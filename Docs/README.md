# Docs

> The "documentation as code" hub for the .NET 2026 reference repo: ADRs, diagrams, runbooks, templates, and cheatsheets — all versioned next to the code they describe.

## Core Concepts

- **Documentation as code.** Markdown, mermaid, and DSL files live in git. They review through PRs, ship through CI, and rot less than wikis.
- **Decision provenance > prose.** Architecture is captured in [ADRs](./ADRs/README.md), not in slide decks. Each significant decision is dated, numbered, and immutable once accepted.
- **Diagrams in source.** Use [Mermaid](./Diagrams/README.md) and [C4](./C4/README.md) so diagrams diff in PRs. Reach for draw.io / Excalidraw only for one-off whiteboards.
- **Runbook every alert.** Every paging alert should link to a [runbook](./Runbooks/README.md). No runbook, no alert.
- **Templates scale judgment.** [Templates](./Templates/README.md) encode the "shape" of good artifacts so juniors and seniors produce comparable output.
- **Cheatsheets reduce context-switch cost.** [CheatSheets](./CheatSheets/README.md) are deliberately terse — they are not tutorials.

## "To Be Dangerous" Cheatsheet

| Need | Go to |
|---|---|
| Record an architecture decision | [ADRs](./ADRs/README.md) + [adr-template.md](./Templates/adr-template.md) |
| Sketch a system at C4 levels 1-3 | [C4](./C4/README.md) |
| Write a mermaid diagram | [Diagrams/mermaid-cheatsheet.md](./Diagrams/mermaid-cheatsheet.md) |
| Start a new microservice README | [service-readme-template.md](./Templates/service-readme-template.md) |
| Write a design RFC | [rfc-template.md](./Templates/rfc-template.md) |
| Threat-model a feature | [threat-model-template.md](./Templates/threat-model-template.md) |
| Run an incident postmortem | [incident-postmortem-template.md](./Templates/incident-postmortem-template.md) |
| Page-on-call runbook | [Runbooks](./Runbooks/README.md) |
| Look up a .NET 10 / regex / git / HTTP fact | [CheatSheets](./CheatSheets/README.md) |

## Quick Reference / Examples in this folder

- [ADRs/](./ADRs/README.md) — Architecture Decision Records (MADR format).
- [C4/](./C4/README.md) — C4 model diagrams (System Context, Container, Component).
- [Diagrams/](./Diagrams/README.md) — Mermaid + draw.io guidance.
- [Roadmaps/](./Roadmaps/dotnet-2026-roadmap-senior-architect.md) — the .NET 2026 senior/architect roadmap.
- [Runbooks/](./Runbooks/README.md) — operational playbooks.
- [Templates/](./Templates/README.md) — reusable doc skeletons (ADR, RFC, threat model, postmortem, runbook, PR, service README).
- [CheatSheets/](./CheatSheets/README.md) — keystone quick-references.

## See also

- [.NET 2026 Roadmap](./Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [FullStack capstone work](../FullStack/README.md)
- Closing principle #1 of the roadmap: *"Write down your decisions. ADRs are the difference between an opinion and an architecture."*
