// Production-leaning AKS cluster: private, Workload Identity, OIDC, Azure Monitor, Flux add-on.
targetScope = 'resourceGroup'

@description('Cluster name suffix.')
param clusterName string

@description('Region.')
param location string = resourceGroup().location

@description('Kubernetes version.')
param k8sVersion string = '1.31'

@description('System node pool VM size.')
param systemVmSize string = 'Standard_D4ds_v5'

@description('User node pool VM size.')
param userVmSize string = 'Standard_D8ds_v5'

@description('Log Analytics workspace resource id.')
param logAnalyticsWorkspaceId string

var tags = {
  workload: clusterName
  managedBy: 'bicep'
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-09-01' = {
  name: 'aks-${clusterName}'
  location: location
  tags: tags
  identity: { type: 'SystemAssigned' }
  sku: { name: 'Base', tier: 'Standard' }
  properties: {
    kubernetesVersion: k8sVersion
    dnsPrefix: 'aks-${clusterName}'
    apiServerAccessProfile: {
      enablePrivateCluster: true
      enablePrivateClusterPublicFQDN: false
    }
    oidcIssuerProfile: { enabled: true }
    securityProfile: {
      workloadIdentity: { enabled: true }
      defender: {
        logAnalyticsWorkspaceResourceId: logAnalyticsWorkspaceId
        securityMonitoring: { enabled: true }
      }
    }
    networkProfile: {
      networkPlugin: 'azure'
      networkPluginMode: 'overlay'
      networkPolicy: 'cilium'
      networkDataplane: 'cilium'
      loadBalancerSku: 'standard'
      outboundType: 'managedNATGateway'
    }
    addonProfiles: {
      omsagent: {
        enabled: true
        config: { logAnalyticsWorkspaceResourceID: logAnalyticsWorkspaceId }
      }
      azureKeyvaultSecretsProvider: {
        enabled: true
        config: { enableSecretRotation: 'true' }
      }
    }
    autoUpgradeProfile: {
      upgradeChannel: 'stable'
      nodeOSUpgradeChannel: 'NodeImage'
    }
    agentPoolProfiles: [
      {
        name: 'system'
        mode: 'System'
        count: 3
        vmSize: systemVmSize
        osType: 'Linux'
        osSKU: 'AzureLinux'
        availabilityZones: [ '1', '2', '3' ]
        enableAutoScaling: true
        minCount: 3
        maxCount: 5
        nodeTaints: [ 'CriticalAddonsOnly=true:NoSchedule' ]
      }
    ]
  }
}

resource userPool 'Microsoft.ContainerService/managedClusters/agentPools@2024-09-01' = {
  parent: aks
  name: 'user'
  properties: {
    mode: 'User'
    vmSize: userVmSize
    osType: 'Linux'
    osSKU: 'AzureLinux'
    availabilityZones: [ '1', '2', '3' ]
    enableAutoScaling: true
    minCount: 2
    maxCount: 20
    count: 2
    maxPods: 60
  }
}

resource flux 'Microsoft.KubernetesConfiguration/extensions@2023-05-01' = {
  scope: aks
  name: 'flux'
  properties: {
    extensionType: 'microsoft.flux'
    autoUpgradeMinorVersion: true
    releaseTrain: 'Stable'
    scope: { cluster: { releaseNamespace: 'flux-system' } }
  }
}

output oidcIssuerUrl string = aks.properties.oidcIssuerProfile.issuerURL
