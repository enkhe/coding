"""
Typer CLI — typed, auto-generates `--help`, validates files exist, etc.

Run: uv run python cli-tool.py ingest --src in.csv --dst out.csv
"""
from pathlib import Path
import typer
from rich import print as rprint

app = typer.Typer(help="Sample data CLI.")

@app.command()
def ingest(
    src: Path = typer.Option(..., exists=True, readable=True, help="Input file."),
    dst: Path = typer.Option(..., help="Output file."),
    dry_run: bool = typer.Option(False, "--dry-run", help="Print actions only."),
):
    """Copy src -> dst with rich logging."""
    if dry_run:
        rprint(f"[yellow]DRY[/yellow] would copy {src} -> {dst}")
        raise typer.Exit()
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_bytes(src.read_bytes())
    rprint(f"[green]OK[/green] {src} -> {dst}")


@app.command()
def validate(path: Path = typer.Argument(..., exists=True)):
    """Validate JSON files against a schema."""
    import json
    data = json.loads(path.read_text())
    if "id" not in data:
        rprint("[red]invalid[/red]: missing 'id'")
        raise typer.Exit(code=1)
    rprint("[green]ok[/green]")


if __name__ == "__main__":
    app()
