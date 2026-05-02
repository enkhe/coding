// Skeleton Bicep generated/customized by `azd`. Real `azd up` will produce more detail.
targetScope = 'subscription'

@description('Resource prefix for all resources.')
param environmentName string

@description('Primary location.')
param location string = deployment().location

var rgName = 'rg-${environmentName}'

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: rgName
  location: location
  tags: {
    'azd-env-name': environmentName
    'cost-center': 'platform'
  }
}

module aca 'modules/container-apps.bicep' = {
  scope: rg
  name: 'aca'
  params: {
    environmentName: environmentName
    location: location
  }
}

output AZURE_LOCATION string = location
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = aca.outputs.envId
