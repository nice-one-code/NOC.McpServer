# NOC.McpServer

<!-- mcp-name: io.github.mr-rahulmaurya/NOC.McpServer -->

An MCP server exposing [NiceOneCode](https://www.niceonecode.com)'s JSON → C# class converter as a tool for AI assistants.

## Creating an Account

If you don't already have a NiceOneCode account, register one via API:

```bash
curl -X POST \
  --header 'Content-Type: application/json' \
  --header 'Accept: application/json' \
  -d '{
    "UserName": "your-username",
    "Password": "your-password",
    "Email": "your-email@example.com",
    "GenderID": 1
  }' \
  'https://www.niceonecode.com/api/nc-register'
```

| Field | Description |
|---|---|
| `UserName` | Desired username |
| `Password` | Desired password — use this as `NOC_PASSWORD` |
| `Email` | A valid email address |
| `GenderID` | `1` = Male, `2` = Female |

**Important:** the response body is a plain string, not a JSON object — for example:
```json
"your-userid"
```

Use this returned value as `NOC_USERID`. Don't assume it will always match the `UserName` you submitted — always use the value the API actually returns.

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

Edit `claude_desktop_config.json` (Windows: `%APPDATA%\Claude\claude_desktop_config.json`, macOS: `~/Library/Application Support/Claude/claude_desktop_config.json`):

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

Fully quit and reopen Claude Desktop after editing.

## Usage with Claude Code

```bash
claude mcp add-json niceonecode '{"command":"noc-mcp","env":{"NOC_USERID":"your-userid","NOC_PASSWORD":"your-password"}}'
```

Verify with `claude mcp list`.

## Usage with Codex

```bash
codex mcp add niceonecode --env NOC_USERID=your-userid --env NOC_PASSWORD=your-password -- noc-mcp
```

Verify inside a session with `/mcp`.

## Usage with VS Code

Add to `.vscode/mcp.json` in your workspace, or your global VS Code MCP settings:

```json
{
  "inputs": [
    {
      "type": "promptString",
      "id": "NOC_USERID",
      "description": "Your NiceOneCode account userid"
    },
    {
      "type": "promptString",
      "id": "NOC_PASSWORD",
      "description": "Your NiceOneCode account password",
      "password": true
    }
  ],
  "servers": {
    "NOC.McpServer": {
      "type": "stdio",
      "command": "dnx",
      "args": ["NOC.McpServer", "--yes"],
      "env": {
        "NOC_USERID": "${input:NOC_USERID}",
        "NOC_PASSWORD": "${input:NOC_PASSWORD}"
      }
    }
  }
}
```

VS Code will prompt for your userid and password the first time the server starts, rather than requiring them hardcoded in the file. Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) for `dnx`, which downloads and runs the package on demand — no separate `dotnet tool install` step needed.

**Note:** if your working directory has a `global.json` pinning an SDK below .NET 10, `dnx` will fail with a confusing "unrecognized argument" error rather than a clear version-mismatch message — worth checking for a stray `global.json` if this happens.

## Usage with Gemini CLI

Edit `~/.gemini/settings.json` (global) or `.gemini/settings.json` (project-specific):

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

Restart Gemini CLI, then run `/mcp` to confirm it's connected.

**Windows note:** if the server fails to start directly, wrap it through `cmd`:
```json
{
  "mcpServers": {
    "niceonecode": {
      "command": "cmd",
      "args": ["/c", "noc-mcp"],
      "env": { "NOC_USERID": "your-userid", "NOC_PASSWORD": "your-password" }
    }
  }
}
```

## Usage with Windsurf

Edit `~/.codeium/windsurf/mcp_config.json` (macOS/Linux) or `%USERPROFILE%\.codeium\windsurf\mcp_config.json` (Windows), or open it via the MCPs icon in the Cascade panel → **Configure**:

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

Fully quit and reopen Windsurf after saving.

## Usage with Devin

**Devin (cloud):** Settings → Connections → MCP servers → **Add a custom MCP**:

```json
{
  "transport": "STDIO",
  "command": "noc-mcp",
  "args": [],
  "env_variables": {
    "NOC_USERID": "your-userid",
    "NOC_PASSWORD": "your-password"
  }
}
```

**Devin for Terminal:** add to `.devin/config.json`:

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

**Devin Desktop** shares Windsurf's `mcp_config.json` — no separate setup needed if already configured above.

## Source

Full source, contribution guide, and local development setup: [github.com/nice-one-code/NOC.McpServer](https://github.com/nice-one-code/NOC.McpServer)

## License

MIT