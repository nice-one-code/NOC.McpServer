using global::NOC.McpServer.Models;
using Microsoft.Extensions.Logging;
using NOC.McpServer.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace NOC.McpServer.Auth
{

    /// <summary>
    /// Logs in to NOC on first use, caches the access token in memory,
    /// attaches it as a Bearer header on every outgoing request, and transparently
    /// re-authenticates + retries once if a call comes back 401.
    /// </summary>
    public class NOCAuthHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<NOCAuthHandler> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private string? _accessToken;
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

        public NOCAuthHandler(IHttpClientFactory factory, ILogger<NOCAuthHandler> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            await EnsureTokenAsync(ct);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await base.SendAsync(request, ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Got 401, forcing re-login and retrying once.");
                await LoginAsync(ct);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                response = await base.SendAsync(request, ct);
            }

            return response;
        }

        private async Task EnsureTokenAsync(CancellationToken ct)
        {
            if (_accessToken != null && DateTimeOffset.UtcNow < _expiresAt)
                return;

            await _lock.WaitAsync(ct);
            try
            {
                if (_accessToken != null && DateTimeOffset.UtcNow < _expiresAt)
                    return; // someone else refreshed while we waited

                await LoginAsync(ct);
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task LoginAsync(CancellationToken ct)
        {
            var userId = Environment.GetEnvironmentVariable("NOC_USERID")
                ?? throw new InvalidOperationException("NOC_USERID env var not set");
            var password = Environment.GetEnvironmentVariable("NOC_PASSWORD")
                ?? throw new InvalidOperationException("NOC_PASSWORD env var not set");

            var authClient = _factory.CreateClient("NOCLoginClient");

            var loginBody = new LoginRequest { UserId = userId, Password = password };

            using var response = await authClient.PostAsJsonAsync("api/nc-login/Login", loginBody, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Login response was empty or malformed.");

            _accessToken = result.AccessToken;
            // Refresh a bit early (5 min buffer) rather than exactly at expiry
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(result.ExpiresIn) - TimeSpan.FromMinutes(5);

            _logger.LogInformation("Logged in to NOC as {Email}, token valid until {ExpiresAt}",
                result.Email, _expiresAt);
        }
    }
}
