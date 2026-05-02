# ARM Templates (legacy)

> Microsoft's older JSON-based IaC. **Prefer Bicep for new work** — Bicep transpiles to ARM. Keep this for maintaining existing templates.

## Core Concepts

- **`$schema` + `contentVersion`** at root
- **`parameters` / `variables` / `resources` / `outputs`** — top-level keys
- **`apiVersion`** per resource type — pin a specific date
- **`copy`** loops, **linked / nested** templates for modularization

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| Deploy | `az deployment group create -g <rg> -f template.json -p @parameters.json` |
| What-if | `az deployment group what-if -g <rg> -f template.json -p @parameters.json` |
| Decompile JSON to Bicep | `bicep decompile template.json` |

## When to convert to Bicep

Almost always. Run `bicep decompile main.json` and clean up the result.

## Examples in this folder

- [`template.json`](template.json) · [`parameters.json`](parameters.json)

## See also

- [../Bicep](../Bicep/) — modern replacement
