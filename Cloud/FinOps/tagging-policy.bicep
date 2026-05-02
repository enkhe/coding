targetScope = 'subscription'

@description('Tags whose presence is required on every resource group.')
param requiredTags array = [
  'cost-center'
  'app'
  'env'
  'owner'
]

resource initiative 'Microsoft.Authorization/policySetDefinitions@2023-04-01' = {
  name: 'require-cost-tags-initiative'
  properties: {
    displayName: 'Require cost-allocation tags'
    description: 'Block creation of resource groups without required tags.'
    policyDefinitions: [for tag in requiredTags: {
      policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/96670d01-0a4d-4649-9c89-2d3abc0a5025' // 'Require a tag on resource groups'
      parameters: {
        tagName: { value: tag }
      }
    }]
  }
}

resource assignment 'Microsoft.Authorization/policyAssignments@2024-04-01' = {
  name: 'require-cost-tags'
  properties: {
    displayName: 'Require cost-allocation tags'
    policyDefinitionId: initiative.id
    enforcementMode: 'Default'
  }
}
