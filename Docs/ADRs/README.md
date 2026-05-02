# ADRs — Architecture Decision Records

> Numbered, dated, immutable records of every significant architectural choice and its context.

## Core Concepts

- **What an ADR is.** A short markdown document (typically 1-2 pages) capturing one architectural decision: the context, the options considered, the choice, and the consequences.
- **Two dominant formats.**
  - **Nygard format** (2011, the original): Title, Status, Context, Decision, Consequences. Minimal and battle-tested.
  - **MADR** (Markdown Any Decision Records): adds explicit *Decision Drivers*, *Considered Options*, and *Pros/Cons of the Options*. Better for non-trivial trade-offs. **This repo uses MADR.**
- **Numbering.** Zero-padded sequential: `0001-`, `0002-`, ... Stable forever; do not renumber.
- **Status lifecycle.** `Proposed` -> `Accepted` -> (`Deprecated` | `Superseded by NNNN`). Never edit an Accepted ADR's decision in place — supersede it with a new one and link both ways.
- **Immutability.** An accepted ADR is a historical record. Fix typos, but never silently rewrite the decision. If reality changes, write the next ADR.
- **Scope.** One decision per ADR. "Use PostgreSQL and event sourcing and gRPC" is three ADRs, not one.
- **Granularity heuristic.** If reversing the decision would cost > 1 sprint, it's an ADR. Otherwise it's a code comment.

## "To Be Dangerous" Cheatsheet

| Action | How |
|---|---|
| New ADR | Copy [template-madr.md](./template-madr.md), name it `NNNN-kebab-title.md`, status `Proposed`. |
| Approve | Change status to `Accepted`, add `Date:`, merge the PR. |
| Replace | Write a new ADR; set old one's status to `Superseded by NNNN`; cross-link. |
| Retire | Status `Deprecated` with rationale, but leave the file. |
| Index | This README, plus optionally a tool like `adr-tools` or `log4brains`. |

## Quick Reference / Examples in this folder

- [0001-record-architecture-decisions.md](./0001-record-architecture-decisions.md) — the meta-ADR (we will use ADRs).
- [0002-use-net-10-lts-baseline.md](./0002-use-net-10-lts-baseline.md) — runtime baseline.
- [0003-modular-monolith-default.md](./0003-modular-monolith-default.md) — default architectural style.
- [template-madr.md](./template-madr.md) — blank MADR template, copy this for every new ADR.

## See also

- [adr-template.md](../Templates/adr-template.md) — full Templates copy of the MADR template.
- [Michael Nygard, "Documenting Architecture Decisions" (2011)](https://www.cognitect.com/blog/2011/11/15/documenting-architecture-decisions).
- [MADR project](https://adr.github.io/madr/).
- Roadmap closing principle #1: "Write down your decisions."
