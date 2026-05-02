# AKS (Azure Kubernetes Service)

> Managed Kubernetes for full control of cluster, networking, and operators.

## Core Concepts

- **Node pools**: system (CoreDNS, metrics-server) + user pools (workload, optional spot).
- **Workload Identity**: federated credentials map K8s ServiceAccount -> Entra app -> Azure RBAC. Replaces Pod Identity.
- **Networking**: Azure CNI Overlay or CNI Powered by Cilium; private cluster for hardened envs.
- **Ingress**: AGIC (App Gateway), Ingress for Container Apps, NGINX, Istio (managed addon).
- **GitOps**: Flux v2 add-on (managed); ArgoCD as DIY.
- **Tiering**: Free (dev), Standard (prod with SLA), Premium (long-term LTS support).

## "To Be Dangerous" Cheatsheet

- Pin **Kubernetes version**; opt into auto-upgrade channel `stable`.
- Enable: **Workload Identity**, **OIDC issuer**, **Azure Monitor for containers**, **Defender for Containers**, **Key Vault Secrets Provider** add-on.
- Use **system-assigned MI** for kubelet pulls from ACR (`az aks update --attach-acr`).
- Set **PDBs** + HPAs + topology spread constraints for HA.
- Use **taints** to isolate workloads onto spot/GPU pools.
- **Private cluster** + jump-box / Bastion / Dev Box.

## When AKS over ACA

| AKS wins | Container Apps wins |
|---|---|
| Custom CRDs / operators | Pure HTTP / queue microservices |
| Daemonsets, sidecars, mTLS mesh | Scale-to-zero out of the box |
| Strict networking (egress lockdown) | Small platform team |
| Multi-tenant on shared cluster | Managed Dapr / KEDA without ops |

## Quick Reference

- Workload Identity: annotate ServiceAccount with `azure.workload.identity/client-id`; pod label `azure.workload.identity/use: "true"`.
- AGIC: managed App Gateway terminates TLS, WAF on; ingress class `azure-application-gateway`.
- Flux add-on: `az k8s-configuration flux create ...`.

## Common Pitfalls

- One huge node pool -> can't isolate noisy neighbors.
- Forgetting **maxSurge** on node pool upgrades -> capacity gaps.
- Skipping **PDB** -> rolling upgrade outage.
- Using kubelet identity for app secrets (anti-pattern; use Workload Identity).

## Examples in this folder

- [aks.bicep](./aks.bicep) - private cluster, Workload Identity, Flux, Azure Monitor.

## See also

- [Container Apps](../ContainerApps/README.md)
- [DevOps/Kubernetes](../../../DevOps/Kubernetes/README.md)
- [DevOps/Helm](../../../DevOps/Helm/README.md)
- [DevOps/GitOps](../../../DevOps/GitOps/README.md)
