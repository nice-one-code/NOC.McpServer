# NOC.McpServer

<!-- mcp-name: io.github.mr-rahulmaurya/NOC.McpServer -->

An MCP server exposing [NiceOneCode](https://www.niceonecode.com)'s JSON → C# class converter as a tool for AI assistants.

## Install

```bash
dotnet tool install -g NOC.McpServer
```

## Configuration

Requires a NiceOneCode account. Set these environment variables in your MCP client's config:

| Variable | Description |
|---|---|
| `NOC_USERID` | Your NiceOneCode userid |
| `NOC_PASSWORD` | Your NiceOneCode password |

## Usage with Claude Desktop

```json
{
  "mcpServers": {
    "niceonecode": {
      "command": "noc-mcp",
      "env": {
        "NOC_USERID": "your-userid",
        "NOC_PASSWORD": "your-password"
      }
    }
  }
}
```

## Source

Full source, contribution guide, and local development setup: [github.com/nice-one-code/NOC.McpServer](https://github.com/nice-one-code/NOC.McpServer)

## License

MIT