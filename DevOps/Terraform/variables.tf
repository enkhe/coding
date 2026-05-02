variable "env" {
  type        = string
  description = "Environment (prod/staging/dev)"
  validation {
    condition     = contains(["prod", "staging", "dev"], var.env)
    error_message = "env must be prod, staging, or dev"
  }
}

variable "location" {
  type    = string
  default = "eastus"
}

variable "tags" {
  type = map(string)
  default = {
    app           = "orders"
    "cost-center" = "platform"
    owner         = "platform@contoso.com"
  }
}
