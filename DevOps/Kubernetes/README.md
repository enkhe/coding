# Kubernetes

> Pods, Deployments, Services, Ingress, ConfigMaps, Secrets, HPA, PDB, NetworkPolicy, RBAC.

## Core Concepts

- **Pod** — one or more containers + shared volumes/network. Smallest schedulable unit.
- **Deployment** — manages replica sets; rolling updates, rollback.
- **Service** — stable virtual IP and DNS for a pod set (`ClusterIP`, `NodePort`, `LoadBalancer`).
- **Ingress** — L7 routing (host/path) into Services. Controllers: NGINX, Traefik, AGIC.
- **ConfigMap / Secret** — non-secret/secret config; mount as env or volume.
- **HPA** — autoscale replicas on CPU/memory/custom metrics.
- **PDB** — minimum replicas during voluntary disruptions (drains).
- **NetworkPolicy** — pod-to-pod firewalling (default-deny is the goal).
- **Probes** — liveness (restart), readiness (LB), startup (slow boot).

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| List pods | `kubectl get pods -A` |
| Logs | `kubectl logs -f -n orders deploy/api` |
| Exec | `kubectl exec -it -n orders deploy/api -- sh` |
| Port-forward | `kubectl port-forward -n orders svc/api 8080:80` |
| Apply manifest | `kubectl apply -f deployment.yaml` |
| Diff before apply | `kubectl diff -f deployment.yaml` |
| Top resources | `kubectl top pod -A` |
| Events | `kubectl get events -A --sort-by=.lastTimestamp` |

## Common Pitfalls

- Liveness probe checks DB → DB blip restarts pods needlessly. Liveness is process-level only.
- No resource `requests` → scheduler can't bin-pack and pods can OOM their neighbors.
- No `PodDisruptionBudget` → cluster maintenance evicts everything at once.
- Default `imagePullPolicy: Always` for tagged images → unnecessary pulls. Use specific tags + `IfNotPresent`.
- Secrets as env vars → leak in logs. Mount as files where possible, redact in logging.

## Examples in this folder

- [`deployment.yaml`](deployment.yaml)
- [`service.yaml`](service.yaml)
- [`ingress.yaml`](ingress.yaml)
- [`hpa.yaml`](hpa.yaml)
- [`networkpolicy.yaml`](networkpolicy.yaml)
- [`pdb.yaml`](pdb.yaml)

## See also

- [../Helm](../Helm/) · [../GitOps](../GitOps/) · [../../Cloud/Azure/AKS](../../Cloud/Azure/AKS/) · [../../Observability/HealthChecks](../../Observability/HealthChecks/)
