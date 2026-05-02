# AWS

> Quick map for someone fluent in Azure who needs to navigate AWS.

## Equivalents

| Azure | AWS | Notes |
|---|---|---|
| App Service | Elastic Beanstalk / App Runner | App Runner is the modern simple option |
| Container Apps (ACA) | App Runner / ECS Fargate | ECS Fargate for finer control |
| AKS | EKS | |
| Functions | Lambda | |
| Service Bus | SQS / SNS / EventBridge | SQS = queue, SNS = pub/sub, EventBridge = event bus + schemas |
| Storage Blob | S3 | |
| Storage Queue | SQS | |
| Storage Table | DynamoDB | DynamoDB is more capable |
| Cosmos DB | DynamoDB | |
| SQL DB | RDS / Aurora | Aurora ~= managed MySQL/Postgres with cloud-native storage |
| Cache for Redis | ElastiCache (Redis / Valkey) | |
| Key Vault | Secrets Manager / KMS / Parameter Store | KMS = keys; Secrets Manager = secrets |
| Entra ID | IAM Identity Center / Cognito | Cognito for CIAM |
| Application Insights | CloudWatch / X-Ray | |
| Log Analytics | CloudWatch Logs | |
| Front Door | CloudFront + Route 53 + WAF | |
| API Management | API Gateway | |
| Bicep | CloudFormation / CDK | CDK is typed (TS/.NET) |
| Application Gateway | ALB | |
| AKS + ingress-nginx | EKS + ALB Controller | |
| Azure DevOps | CodePipeline / CodeBuild / CodeDeploy | Most teams now use GitHub Actions instead |

## Auth from .NET to AWS

```csharp
// Package: AWSSDK.Core, AWSSDK.S3
var s3 = new AmazonS3Client();              // uses AWS SDK credential chain (env, profile, IRSA, IMDS)
await s3.PutObjectAsync(new PutObjectRequest
{
    BucketName = "my-bucket",
    Key = "orders/2026/04/12.json",
    ContentBody = json,
});
```

In EKS pods: prefer **IRSA** (IAM Roles for Service Accounts) over static keys.

## See also

- [../Azure](../Azure/) · [../GCP](../GCP/) · [../FinOps](../FinOps/)
