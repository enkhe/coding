output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "log_analytics_workspace_id" {
  value = azurerm_log_analytics_workspace.law.id
}

output "storage_account_blob_endpoint" {
  value = azurerm_storage_account.sa.primary_blob_endpoint
}
