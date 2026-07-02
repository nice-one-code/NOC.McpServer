using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Server;

namespace NOC.McpServer.Tools
{
    [McpServerToolType]
    public static class JsonToCSharpTool
    {
        [McpServerTool, Description("Converts a JSON sample into C# class definitions using the NiceOneCode json-to-cSharp converter.")]
        public static async Task<string> JsonToCSharp(
        IHttpClientFactory httpClientFactory,
        [Description("The JSON sample to convert into C# classes")] string json)
        {
            try
            {
                var client = httpClientFactory.CreateClient("NOCHttpClient");

                var payload = new { Code = json };

                using var response = await client.PostAsJsonAsync("api/nc-converter/json-to-cSharp", payload);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return $"Conversion failed ({(int)response.StatusCode} {response.StatusCode}): {errorBody}";
                }

                var raw = await response.Content.ReadAsStringAsync();
                return TryExtractResultField(raw) ?? raw;
            }
            catch (Exception ex)
            {
                // TEMPORARY: surface the real exception for debugging.
                // Remove/replace with proper logging once diagnosed.
                return $"EXCEPTION: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
            }
        }

        private static string? TryExtractResultField(string raw)
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.ValueKind == JsonValueKind.String)
                    return doc.RootElement.GetString(); // response is just a JSON-encoded string
                if (doc.RootElement.TryGetProperty("code", out var code))
                    return code.GetString();
                if (doc.RootElement.TryGetProperty("result", out var result))
                    return result.GetString();
            }
            catch (JsonException ex)
            {
                return $"TryExtractResultField EXCEPTION: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
            }
            return null;
        }
    }
}
