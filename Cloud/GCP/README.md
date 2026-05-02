# GCP

> Quick map for an Azure-native moving to Google Cloud.

## Equivalents

| Azure | GCP | Notes |
|---|---|---|
| App Service / ACA | **Cloud Run** | Best-in-class scale-to-zero containers |
| AKS | **GKE** (Autopilot) | Autopilot is closer to ACA in ergonomics |
| Functions | **Cloud Functions** / Cloud Run jobs | |
| Service Bus | **Pub/Sub** | Topics + subscriptions |
| Storage Blob | **Cloud Storage (GCS)** | |
| Cosmos DB | **Firestore** / **Spanner** | Spanner = global SQL with strong consistency |
| SQL DB | **Cloud SQL** (Postgres/MySQL/SQL Server) | |
| Cache for Redis | **Memorystore** | |
| Key Vault | **Secret Manager** + **KMS** | |
| Entra ID | **Cloud Identity** / **Identity Platform** | |
| Application Insights | **Cloud Trace** + **Cloud Logging** + **Cloud Monitoring** | |
| Bicep | **Deployment Manager** / Terraform / Pulumi | Terraform dominant |
| Front Door | **Cloud CDN** + **Cloud Load Balancer** | |
| API Management | **Apigee** / **API Gateway** | Apigee is the enterprise option |

## .NET on GCP — typical setup

```csharp
// Package: Google.Cloud.Storage.V1
var storage = await StorageClient.CreateAsync();         // ADC: env/gcloud/Workload Identity
await storage.UploadObjectAsync("my-bucket", "orders/12.json", "application/json", stream);
```

In GKE pods: use **Workload Identity** (binds K8s SA to GCP SA), not service account JSON keys.

## See also

- [../Azure](../Azure/) · [../AWS](../AWS/) · [../FinOps](../FinOps/)
