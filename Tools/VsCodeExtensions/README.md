# VS Code Extensions

> Curated picks for a senior .NET / cloud / AI dev in 2026.

## Must-have

| Extension | Why |
|---|---|
| **C# Dev Kit** (`ms-dotnettools.csdevkit`) | Solution explorer, test runner, debugger |
| **C#** (`ms-dotnettools.csharp`) | Roslyn LSP |
| **REST Client** (`humao.rest-client`) | `*.http` files, runs requests inline |
| **Bicep** (`ms-azuretools.vscode-bicep`) | Bicep IntelliSense |
| **Docker** (`ms-azuretools.vscode-docker`) | Dockerfile, compose, registries |
| **Kubernetes** (`ms-kubernetes-tools.vscode-kubernetes-tools`) | kubectl, manifests, contexts |
| **GitHub Actions** (`github.vscode-github-actions`) | Workflow lint + run |
| **GitLens** (`eamodio.gitlens`) | Inline blame, file history |
| **ErrorLens** (`usernamehw.errorlens`) | Diagnostics inline |
| **Mermaid Preview** (`bierner.markdown-mermaid`) | Diagrams in markdown |
| **Edit CSV** (`janisdd.vscode-edit-csv`) | View/edit CSVs |
| **Rainbow CSV** (`mechatroner.rainbow-csv`) | Colorize CSVs |
| **Prettier** (`esbenp.prettier-vscode`) | JS/TS/JSON formatter |
| **ESLint** (`dbaeumer.vscode-eslint`) | JS/TS lint |
| **EditorConfig** (`editorconfig.editorconfig`) | Respect repo style |

## AI / .NET 2026

| Extension | Why |
|---|---|
| **GitHub Copilot** (`github.copilot`) | Inline + chat |
| **Polyglot Notebooks** (`ms-dotnettools.dotnet-interactive-vscode`) | C# / F# / Python in notebooks |

## Frontend

| Extension | Why |
|---|---|
| **Tailwind CSS IntelliSense** | Class autocompletion + linting |
| **Vue – Official** (`Vue.volar`) | Vue 3 |
| **Svelte for VS Code** (`svelte.svelte-vscode`) | Svelte 5 |
| **Angular Language Service** | Angular |

## Cloud

| Extension | Why |
|---|---|
| **Azure Tools** (`ms-vscode.vscode-node-azure-pack`) | Azure resource explorer |
| **AWS Toolkit** | AWS resources |
| **Terraform** (`hashicorp.terraform`) | HCL |

## Drop-in `.vscode/extensions.json`

```jsonc
{
  "recommendations": [
    "ms-dotnettools.csdevkit",
    "ms-dotnettools.csharp",
    "ms-azuretools.vscode-bicep",
    "ms-azuretools.vscode-docker",
    "ms-kubernetes-tools.vscode-kubernetes-tools",
    "github.vscode-github-actions",
    "humao.rest-client",
    "eamodio.gitlens",
    "usernamehw.errorlens",
    "bierner.markdown-mermaid",
    "esbenp.prettier-vscode",
    "dbaeumer.vscode-eslint",
    "editorconfig.editorconfig",
    "github.copilot",
    "ms-vscode.vscode-node-azure-pack"
  ]
}
```

## See also

- [../Editors](../Editors/) · [../Git](../Git/)
