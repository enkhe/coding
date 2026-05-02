terraform {
  backend "azurerm" {
    resource_group_name  = "tfstate"
    storage_account_name = "tfstatecontoso"
    container_name       = "tfstate"
    key                  = "orders/${terraform.workspace}.tfstate"
    use_oidc             = true
  }
}
