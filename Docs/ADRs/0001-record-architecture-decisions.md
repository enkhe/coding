# 0001. Record architecture decisions

- **Status:** Accepted
- **Date:** 2026-01-15
- **Deciders:** Architecture guild
- **Tags:** process, governance

## Context and Problem Statement

We need a durable, low-friction way to capture architecturally significant decisions so that future engineers can understand *why* the system looks the way it does, not just *what* it looks like. Wikis rot, slide decks vanish, and tribal knowledge walks out the door at every reorg.

How should we record architecture decisions across all repos in the .NET 2026 stack?

## Decision Drivers

- Decisions must be discoverable by anyone with repo access, including new hires on day one.
- The format must be reviewable in a pull request.
- The cost of writing one must be low enough that engineers actually do it.
- The record must be immutable (auditable history) but supersedable.
- Should integrate with existing markdown / git tooling — no special hosting.

## Considered Options

1. **Architecture Decision Records (ADRs) in-repo, MADR format.**
2. **Confluence / SharePoint pages.**
3. **A separate "architecture" repo with free-form design docs.**
4. **Do nothing; rely on commit messages and code comments.**

## Decision Outcome

**Chosen option:** ADRs in-repo using the MADR template, stored under `Docs/ADRs/` and numbered sequentially from `0001`.

### Positive Consequences

- Decisions diff in PRs alongside the code they affect.
- Onboarding artifact: reading ADRs in order tells the story of the system.
- Format is constrained enough to be skimmable, flexible enough for any decision.
- No new tooling required; works in any markdown viewer.

### Negative Consequences

- Engineers must learn the format and maintain discipline (status updates, supersession links).
- Cross-repo decisions need to live somewhere — we will keep org-wide ADRs in this repo and per-service ADRs in each service repo.

## Pros and Cons of the Options

### Option 1 — ADRs in-repo (MADR)

- Good, version-controlled, reviewable, discoverable.
- Good, well-known industry pattern (Nygard, ThoughtWorks Tech Radar).
- Bad, requires writing discipline.

### Option 2 — Confluence / SharePoint

- Good, familiar to non-engineers.
- Bad, page rot, broken links, no PR review, no diff history that engineers can read.

### Option 3 — Separate architecture repo

- Good, central catalog.
- Bad, decisions drift from the code; reviewers context-switch.

### Option 4 — Do nothing

- Good, zero cost up front.
- Bad, every new hire re-discovers the same context, and nobody remembers why.

## Links

- [MADR template](./template-madr.md)
- [Nygard, "Documenting Architecture Decisions"](https://www.cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
- Roadmap closing principle #1: "Write down your decisions."
