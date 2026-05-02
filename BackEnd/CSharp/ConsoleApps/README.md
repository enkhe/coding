# Console Apps

> CLIs and one-shot tools. Pair with **System.CommandLine** + **Spectre.Console** for a polished UX.

## "To Be Dangerous" Cheatsheet

| Need | Library |
|---|---|
| Argument parsing + binding | `System.CommandLine` |
| Pretty UI (tables, prompts, progress) | `Spectre.Console` |
| Generic host (DI, config) | `Microsoft.Extensions.Hosting` |
| AOT-friendly | Yes — both libs support trimming |

## Quick Reference

```csharp
// Package: System.CommandLine, Spectre.Console
using System.CommandLine;
using Spectre.Console;

var src = new Option<FileInfo>("--src") { IsRequired = true };
src.AddValidator(r =>
{
    if (!r.GetValueForOption(src)!.Exists) r.ErrorMessage = "src does not exist";
});
var dst = new Option<FileInfo>("--dst") { IsRequired = true };
var verbose = new Option<bool>("--verbose");

var root = new RootCommand("file copier") { src, dst, verbose };
root.SetHandler(async (s, d, v) =>
{
    AnsiConsole.MarkupLine($"[grey]copying[/] {s.FullName} -> [green]{d.FullName}[/]");
    await using var inS = s.OpenRead();
    await using var outS = d.OpenWrite();
    await inS.CopyToAsync(outS);
    AnsiConsole.MarkupLine("[bold green]ok[/]");
}, src, dst, verbose);

return await root.InvokeAsync(args);
```

## Generic host pattern (for DI-rich CLIs)

```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddTransient<MyJob>();

var host = builder.Build();
var job = host.Services.GetRequiredService<MyJob>();
await job.RunAsync();
```

## Common Pitfalls

- Reading `Console.ReadLine()` with stdin redirected from CI → blocks forever
- Forgetting non-zero exit codes on failure → CI thinks success
- Color codes when piped → `Console.IsOutputRedirected` check or AnsiConsole auto-detects
- Hard-coded paths/encodings — use `Path.Combine`, `Encoding.UTF8`

## See also

- [../NativeAOT](../NativeAOT/) · [../../../Tools/CLI](../../../Tools/CLI/)
