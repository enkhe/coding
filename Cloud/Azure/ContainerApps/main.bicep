// Container Apps environment + app with KEDA Service Bus scaler and managed identity.
targetScope = 'resourceGroup'

@description('Workload name (lowercase, used in resource names).')
param workloadName string = 'demo'

@description('Azure region.')
param location string = resourceGroup().location

@description('ACR login server, e.g. myacr.azurecr.io.')
param acrLoginServer string

@description('Container image with tag.')
param image string

@description('Service Bus namespace FQDN for KEDA scaler.')
param serviceBusNamespace string

@description('Service Bus queue name.')
param queueName string = 'work'

var tags = {
  workload: workloadName
  env: 'prod'
  managedBy: 'bicep'
}

resource law 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-${workloadName}'
  location: location
  tags: tags
  properties: {
    sku: { name: 'PerGB2018' }
    retentionInDays: 30
  }
}

resource uami 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'id-${workloadName}'
  location: location
  tags: tags
}

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' = {
  name: 'cae-${workloadName}'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: law.properties.customerId
        sharedKey: law.listKeys().primarySharedKey
      }
    }
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
    zoneRedundant: true
  }
}

resource app 'Microsoft.App/containerApps@2024-10-02-preview' = {
  name: 'ca-${workloadName}-api'
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { '${uami.id}': {} }
  }
  properties: {
    environmentId: env.id
    workloadProfileName: 'Consumption'
    configuration: {
      activeRevisionsMode: 'Multiple'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        traffic: [
          { latestRevision: true, weight: 100 }
        ]
      }
      registries: [
        {
          server: acrLoginServer
          identity: uami.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'api'
          image: image
          resources: { cpu: json('0.5'), memory: '1Gi' }
          probes: [
            { type: 'Liveness',  httpGet: { path: '/healthz/live',  port: 8080 } }
            { type: 'Readiness', httpGet: { path: '/healthz/ready', port: 8080 } }
            { type: 'Startup',   httpGet: { path: '/healthz/start', port: 8080 } }
          ]
          env: [
            { name: 'AZURE_CLIENT_ID', value: uami.properties.clientId }
            { name: 'ServiceBus__Namespace', value: serviceBusNamespace }
          ]
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 20
        rules: [
          {
            name: 'http'
            http: { metadata: { concurrentRequests: '50' } }
          }
          {
            name: 'sbq'
            custom: {
              type: 'azure-servicebus'
              metadata: {
                namespace: serviceBusNamespace
                queueName: queueName
                messageCount: '5'
              }
              identity: uami.id
            }
          }
        ]
      }
    }
  }
}

output appFqdn string = app.properties.configuration.ingress.fqdn
