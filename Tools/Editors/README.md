# Editors / IDEs

> Visual Studio 2026, VS Code, JetBrains Rider — pick by team and workload.

## When to use which

| Editor | Strengths | Notes |
|---|---|---|
| **Visual Studio 2026** | Best .NET debug, profilers, designers; Windows | Best for Blazor/MAUI debug, large solutions |
| **VS Code + C# Dev Kit** | Cross-platform, fast, great for monorepos | Multi-language teams, devcontainers |
| **JetBrains Rider** | Cross-platform, ReSharper-grade refactoring | Linux/macOS .NET, large refactors |

## VS Code essentials

`.vscode/settings.json` — repo-level overrides:

```jsonc
{
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": { "source.fixAll": "explicit" },
  "editor.tabSize": 4,
  "files.trimTrailingWhitespace": true,
  "files.insertFinalNewline": true,
  "[csharp]": { "editor.defaultFormatter": "ms-dotnettools.csharp" },
  "[typescript]": { "editor.defaultFormatter": "esbenp.prettier-vscode" },
  "dotnet.defaultSolution": "Orders.sln"
}
```

`.vscode/launch.json` — F5 launch profile:

```jsonc
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Run",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/Orders.Api/bin/Debug/net10.0/Orders.Api.dll",
      "preLaunchTask": "build",
      "console": "internalConsole",
      "stopAtEntry": false,
      "env": { "ASPNETCORE_ENVIRONMENT": "Development" }
    }
  ]
}
```

`.vscode/tasks.json` — common tasks:

```jsonc
{
  "version": "2.0.0",
  "tasks": [
    { "label": "build", "type": "shell", "command": "dotnet build --no-restore -c Debug",
      "problemMatcher": "$msCompile", "group": "build" },
    { "label": "test",  "type": "shell", "command": "dotnet test --no-build", "group": "test" }
  ]
}
```

## Devcontainers

`.devcontainer/devcontainer.json` for reproducible dev:

```jsonc
{
  "name": "dotnet-10",
  "image": "mcr.microsoft.com/devcontainers/dotnet:10.0-noble",
  "features": {
    "ghcr.io/devcontainers/features/azure-cli:1": {},
    "ghcr.io/devcontainers/features/docker-in-docker:2": {}
  },
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit",
        "ms-azuretools.vscode-bicep",
        "esbenp.prettier-vscode",
        "streetsidesoftware.code-spell-checker",
        "GitHub.copilot",
        "humao.rest-client"
      ]
    }
  }
}
```

## Rider tips

- Keep `.editorconfig` in the repo — Rider respects it.
- Use **Solution-wide analysis** (toggle bottom-right) for big-picture issues.
- Plugins worth installing: **Bicep**, **Azure Toolkit**, **GitToolBox**.

## VS 2026 productivity

- Inline AI completions + chat side panel (Copilot)
- Hot Reload for Blazor/ASP.NET Core
- IntelliTrace for time-travel debug (Enterprise)
- Memory & PerfView profilers built in

## See also

- [../VsCodeExtensions](../VsCodeExtensions/) · [../Git](../Git/)
