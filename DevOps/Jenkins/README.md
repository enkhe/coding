# Jenkins

> The legacy CI workhorse. Declarative `Jenkinsfile`s, agents, shared libraries, credentials.

## Core Concepts

- **Pipeline** — DSL in a `Jenkinsfile` (Groovy). Two flavors: declarative (preferred) and scripted.
- **Agent** — node executing the pipeline; `agent any`, `agent { label 'linux' }`, or `agent { docker { image 'mcr.microsoft.com/dotnet/sdk:10.0' } }`
- **Stages → Steps** — like other CI systems
- **Shared libraries** — reusable Groovy under `vars/` or `src/`
- **Credentials** — bound via `withCredentials` or `credentialsId`
- **Multibranch pipeline** — auto-discover branches/PRs

## "To Be Dangerous" Cheatsheet

| Need | Snippet |
|---|---|
| Run on docker agent | `agent { docker { image 'mcr.microsoft.com/dotnet/sdk:10.0' } }` |
| Bind a secret | `withCredentials([string(credentialsId: 'azure-sp', variable: 'AZ_SP')])` |
| Parallel | `parallel(unit: { ... }, integration: { ... })` |
| Stash artifacts | `stash includes: 'artifacts/**', name: 'art'` |
| Post hooks | `post { success { ... } failure { ... } always { ... } }` |

## Quick Reference

See [`Jenkinsfile`](Jenkinsfile).

## Common Pitfalls

- Using scripted (CPS) pipelines for new work → declarative is more constrained, more reliable
- Plain Groovy in pipeline (untrusted) → sandbox restrictions; prefer shared libs
- Agents without disk space cleanup → builds get slower over time
- No retention policy → master grows unbounded

## See also

- [../GitHubActions](../GitHubActions/) · [../AzureDevOps](../AzureDevOps/)
