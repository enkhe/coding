@description('Storage account name. Must be globally unique, 3-24 chars, lowercase alphanumeric.')
@minLength(3)
@maxLength(24)
param name string

param location string
param tags object = {}

@description('SKU.')
@allowed(['Standard_LRS','Standard_ZRS','Standard_GRS'])
param sku string = 'Standard_LRS'

resource storage 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: name
  location: location
  tags: tags
  sku: { name: sku }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    publicNetworkAccess: 'Disabled'
    networkAcls: { defaultAction: 'Deny', bypass: 'AzureServices' }
  }
}

output endpoint string = storage.properties.primaryEndpoints.blob
output id string = storage.id
