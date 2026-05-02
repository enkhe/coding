# GitOps

> Git is the source of truth; an in-cluster operator reconciles toward the desired state. Two leading tools: **Flux** and **ArgoCD**.

## Core Concepts

- **Reconciliation** — operator continuously diffs cluster state vs git; applies changes
- **Pull-based** — cluster pulls from git; no CI pushing kubectl
- **App-of-apps** (Argo) / **Kustomization tree** (Flux) — hierarchical config layout
- **Image automation** — bot bumps image tag in git when a new image lands
- **Promotion** — a PR-driven flow moves config from `dev/` → `staging/` → `prod/` directories

## Repository pattern

```
infra-repo/
├── apps/
│   └── orders/
│       ├── base/                   ← Kustomize base
│       │   ├── deployment.yaml
│       │   ├── service.yaml
│       │   └── kustomization.yaml
│       └── overlays/
│           ├── dev/    kustomization.yaml + patches
│           ├── staging/
│           └── prod/
└── clusters/
    ├── dev-cluster/    flux GitRepository + Kustomization
    ├── staging-cluster/
    └── prod-cluster/
```

## Quick Reference (Flux)

```yaml
# clusters/prod-cluster/orders.yaml
apiVersion: source.toolkit.fluxcd.io/v1
kind: GitRepository
metadata: { name: orders, namespace: flux-system }
spec:
  interval: 1m
  url: https://github.com/contoso/infra
  ref: { branch: main }
---
apiVersion: kustomize.toolkit.fluxcd.io/v1
kind: Kustomization
metadata: { name: orders, namespace: flux-system }
spec:
  interval: 5m
  path: ./apps/orders/overlays/prod
  prune: true
  sourceRef: { kind: GitRepository, name: orders }
  healthChecks:
    - { kind: Deployment, name: orders-api, namespace: orders }
```

## Quick Reference (ArgoCD)

```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: orders-prod
  namespace: argocd
spec:
  project: default
  source:
    repoURL: https://github.com/contoso/infra
    targetRevision: main
    path: apps/orders/overlays/prod
  destination:
    server: https://kubernetes.default.svc
    namespace: orders
  syncPolicy:
    automated: { prune: true, selfHeal: true }
    syncOptions: [CreateNamespace=true]
```

## Common Pitfalls

- Drift via direct `kubectl apply` → operator reverts; lost work
- Putting secrets in plain git → use sealed-secrets / external-secrets-operator
- One Application per cluster, no app-of-apps → cluster bootstrap is brittle
- Image automation without immutable tags → race conditions

## Examples in this folder

- [`flux-orders.yaml`](flux-orders.yaml) — Flux Kustomization
- [`argocd-app.yaml`](argocd-app.yaml) — ArgoCD Application

## See also

- [../Kubernetes](../Kubernetes/) · [../Helm](../Helm/)
