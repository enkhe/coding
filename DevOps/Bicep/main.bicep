targetScope = 'subscription'

@description('Environment name.')
@allowed(['prod','staging','dev'])
param env string

@description('Primary location.')
param location string = deployment().location

var tags = {
  app: 'orders'
  env: env
  'cost-center': 'platform'
  owner: 'platform@contoso.com'
}

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'rg-orders-${env}'
  location: location
  tags: tags
}

module storage 'modules/storage.bicep' = {
  scope: rg
  name: 'storage'
  params: {
    name: 'storders${env}${uniqueString(rg.id)}'
    location: location
    tags: tags
  }
}

module log 'modules/log-analytics.bicep' = {
  scope: rg
  name: 'logs'
  params: { name: 'log-orders-${env}', location: location, tags: tags }
}

output storageEndpoint string = storage.outputs.endpoint
output workspaceId string = log.outputs.id
