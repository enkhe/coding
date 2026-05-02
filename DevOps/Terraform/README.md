# Terraform

> Multi-cloud IaC, declarative, with state management. Use when you span clouds (or want HCL across teams).

## Core Concepts

- **Provider** — clouds + services (`azurerm`, `aws`, `google`, `kubernetes`, etc.)
- **State** — store remotely (Azure Storage, S3, Terraform Cloud); state has secrets — protect it.
- **Modules** — reusable building blocks; published to a registry or local
- **Workspaces** — environment isolation; or use directory-per-env (preferred for clarity)
- **`plan` / `apply` / `destroy`** — preview / change / wipe
- **Drift detection** — `terraform plan` shows config vs reality

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| Init | `terraform init` (downloads providers + backend) |
| Plan | `terraform plan -out=tf.plan` |
| Apply | `terraform apply tf.plan` |
| State | `terraform state list / show / mv / rm` |
| Module from registry | `module "x" { source = "Azure/aks/azurerm" version = "..." }` |
| Validate | `terraform fmt -recursive`, `terraform validate` |
| Lint | `tflint`, `tfsec`, `checkov` |

## Quick Reference

```hcl
# main.tf
terraform {
  required_version = ">= 1.9"
  required_providers {
    azurerm = { source = "hashicorp/azurerm", version = "~> 4.0" }
  }
  backend "azurerm" {
    resource_group_name  = "tfstate"
    storage_account_name = "tfstatecontoso"
    container_name       = "tfstate"
    key                  = "orders.tfstate"
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-orders-${var.env}"
  location = var.location
  tags     = var.tags
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "log-orders-${var.env}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = var.tags
}
```

## Common Pitfalls

- Local state on multi-engineer projects → state corruption / lost work
- `apply` without a plan file → drift between what was reviewed and what got applied
- Provider version pins missing → breaking changes silently land
- Secrets in `.tfvars` committed to git → leaked

## Examples in this folder

- [`main.tf`](main.tf), [`variables.tf`](variables.tf), [`outputs.tf`](outputs.tf), [`backend.tf`](backend.tf)

## See also

- [../Bicep](../Bicep/) · [../Pulumi](../Pulumi/) · [../GitHubActions](../GitHubActions/)
