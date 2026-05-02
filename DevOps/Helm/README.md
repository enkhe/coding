# Helm

> Templated K8s manifests + a release lifecycle (install / upgrade / rollback).

## Core Concepts

- **Chart** — package: `Chart.yaml`, `values.yaml`, `templates/`
- **Release** — installed instance of a chart in a namespace
- **Repository** — a place that hosts charts (OCI registries supported)
- **Dependencies** — `Chart.yaml`'s `dependencies` + `Chart.lock`
- **Hooks** — pre/post install/upgrade/delete jobs

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| Install | `helm install orders ./chart -n orders --create-namespace -f values.prod.yaml` |
| Upgrade | `helm upgrade orders ./chart -n orders -f values.prod.yaml` |
| Rollback | `helm rollback orders 3 -n orders` |
| Diff | `helm diff upgrade ...` (plugin) |
| OCI push | `helm push orders-1.0.0.tgz oci://my.registry/charts` |
| Lint | `helm lint ./chart` |

## Quick Reference

```yaml
# Chart.yaml
apiVersion: v2
name: orders-api
version: 1.0.0
appVersion: "1.4.2"
dependencies:
  - name: postgresql
    version: 15.x
    repository: https://charts.bitnami.com/bitnami
```

```yaml
# templates/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata: { name: {{ include "orders-api.fullname" . }} }
spec:
  replicas: {{ .Values.replicaCount }}
  selector: { matchLabels: { app: {{ .Release.Name }} } }
  template:
    metadata: { labels: { app: {{ .Release.Name }} } }
    spec:
      containers:
        - name: api
          image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
          ports: [{ containerPort: 8080 }]
```

## Common Pitfalls

- Untemplated namespace → installs into `default` accidentally
- No `_helpers.tpl` → label/name drift across templates
- Putting secrets in `values.yaml` → use sealed-secrets or external-secrets-operator
- Tracking installed values without source control → drift

## Examples in this folder

- [`Chart.yaml`](Chart.yaml) · [`values.yaml`](values.yaml) · [`templates/deployment.yaml`](templates/deployment.yaml)

## See also

- [../Kubernetes](../Kubernetes/) · [../GitOps](../GitOps/)
