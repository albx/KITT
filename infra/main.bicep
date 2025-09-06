targetScope = 'subscription'

param resourceGroupName string

param location string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

module kitt_env 'kitt-env/kitt-env.bicep' = {
  name: 'kitt-env'
  scope: rg
  params: {
    location: location
  }
}

output kitt_env_AZURE_CONTAINER_REGISTRY_NAME string = kitt_env.outputs.AZURE_CONTAINER_REGISTRY_NAME

output kitt_env_AZURE_CONTAINER_REGISTRY_ENDPOINT string = kitt_env.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT

output kitt_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = kitt_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID

output kitt_env_AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = kitt_env.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN

output kitt_env_AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = kitt_env.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID
