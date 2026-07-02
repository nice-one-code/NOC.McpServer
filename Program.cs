using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using NOC.McpServer.Auth;

var builder = Host.CreateApplicationBuilder(args);

// Route all logs to stderr — stdout is reserved for the MCP protocol stream
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddTransient<NOCAuthHandler>();

// Plain client — used ONLY for the login call itself, no auth handler attached
builder.Services.AddHttpClient("NOCLoginClient", client =>
{
    client.BaseAddress = new Uri("https://www.niceonecode.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("NOCHttpClient", client =>
{
    client.BaseAddress = new Uri("https://www.niceonecode.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<NOCAuthHandler>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();    

var app = builder.Build();
await app.RunAsync();