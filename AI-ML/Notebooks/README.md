# Notebooks

> Jupyter for Python; Polyglot Notebooks for .NET (and mixed). Use for **exploration**, not production.

## Conventions

- One notebook = one question or one experiment
- Top cell: title + date + author + dataset version
- Second cell: imports + seed
- **Restart and Run All** before committing — make sure it actually runs end-to-end
- **`nbstripout`** as a git filter — strip outputs to keep diffs sane

## Polyglot Notebooks (.NET)

Polyglot Notebooks let you run C#, F#, PowerShell, JavaScript, SQL, KQL in one notebook (`.dib` or `.ipynb`).

Install in VS Code: extension `ms-dotnettools.dotnet-interactive-vscode`.

```csharp
// First C# cell
#r "nuget: Microsoft.Data.Sqlite, 9.0.0"
using Microsoft.Data.Sqlite;

var conn = new SqliteConnection("Data Source=:memory:");
conn.Open();
```

## Anti-patterns

- Notebooks committed with cell outputs (huge diffs)
- "Production code lives in notebooks" — refactor to .py / .cs files; import into notebook
- Hidden state from non-linear execution (run cell 5, then 3, then 7) → Restart-and-Run-All often
- Globals everywhere — wrap in functions for testability

## See also

- [../MachineLearning](../MachineLearning/) · [../DeepLearning](../DeepLearning/) · [../MLOps](../MLOps/)
