terraform {
  required_version = ">= 1.9"
  required_providers {
    azurerm = { source = "hashicorp/azurerm", version = "~> 4.0" }
  }
}

provider "azurerm" {
  features {}
  use_oidc = true   # GitHub OIDC; no secrets in pipelines
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

resource "azurerm_storage_account" "sa" {
  name                              = "storders${var.env}${substr(replace(uuid(), "-", ""), 0, 8)}"
  resource_group_name               = azurerm_resource_group.rg.name
  location                          = azurerm_resource_group.rg.location
  account_tier                      = "Standard"
  account_replication_type          = "LRS"
  min_tls_version                   = "TLS1_2"
  allow_nested_items_to_be_public   = false
  public_network_access_enabled     = false
  tags                              = var.tags

  lifecycle { ignore_changes = [name] }
}
