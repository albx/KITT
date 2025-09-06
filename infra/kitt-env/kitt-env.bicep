@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param tags object = { }

resource kitt_env_mi 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' = {
  name: take('kitt_env_mi-${uniqueString(resourceGroup().id)}', 128)
  location: location
  tags: tags
}

resource kitt_env_acr 'Microsoft.ContainerRegistry/registries@2025-04-01' = {
  name: take('kittenvacr${uniqueString(resourceGroup().id)}', 50)
  location: location
  sku: {
    name: 'Basic'
  }
  tags: tags
}

resource kitt_env_acr_kitt_env_mi_AcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(kitt_env_acr.id, kitt_env_mi.id, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d'))
  properties: {
    principalId: kitt_env_mi.properties.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
    principalType: 'ServicePrincipal'
  }
  scope: kitt_env_acr
}

resource kitt_env_law 'Microsoft.OperationalInsights/workspaces@2025-02-01' = {
  name: take('kittenvlaw-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
  tags: tags
}

resource kitt_env 'Microsoft.App/managedEnvironments@2025-01-01' = {
  name: take('kittenv${uniqueString(resourceGroup().id)}', 24)
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: kitt_env_law.properties.customerId
        sharedKey: kitt_env_law.listKeys().primarySharedKey
      }
    }
    workloadProfiles: [
      {
        name: 'consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
  tags: tags
}

resource aspireDashboard 'Microsoft.App/managedEnvironments/dotNetComponents@2024-10-02-preview' = {
  name: 'aspire-dashboard'
  properties: {
    componentType: 'AspireDashboard'
  }
  parent: kitt_env
}

output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = kitt_env_law.name

output AZURE_LOG_ANALYTICS_WORKSPACE_ID string = kitt_env_law.id

output AZURE_CONTAINER_REGISTRY_NAME string = kitt_env_acr.name

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = kitt_env_acr.properties.loginServer

output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = kitt_env_mi.id

output AZURE_CONTAINER_APPS_ENVIRONMENT_NAME string = kitt_env.name

output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = kitt_env.id

output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = kitt_env.properties.defaultDomain
