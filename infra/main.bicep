targetScope = 'subscription'

param resourceGroupName string

param location string

param principalId string

param KittAzureSqlResourceGroup string

param KittAzureSqlName string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

module kitt_env 'kitt-env/kitt-env.bicep' = {
  name: 'kitt-env'
  scope: rg
  params: {
    location: location
    userPrincipalId: principalId
  }
}

module kitt_sql 'kitt-sql/kitt-sql.bicep' = {
  name: 'kitt-sql'
  scope: resourceGroup(KittAzureSqlResourceGroup)
  params: {
    location: location
    KittAzureSqlName: KittAzureSqlName
  }
}

module kitt_cms_api_identity 'kitt-cms-api-identity/kitt-cms-api-identity.bicep' = {
  name: 'kitt-cms-api-identity'
  scope: rg
  params: {
    location: location
  }
}

module kitt_cms_api_roles_kitt_sql 'kitt-cms-api-roles-kitt-sql/kitt-cms-api-roles-kitt-sql.bicep' = {
  name: 'kitt-cms-api-roles-kitt-sql'
  scope: resourceGroup(KittAzureSqlResourceGroup)
  params: {
    location: location
  }
}

module kitt_proposals_api_identity 'kitt-proposals-api-identity/kitt-proposals-api-identity.bicep' = {
  name: 'kitt-proposals-api-identity'
  scope: rg
  params: {
    location: location
  }
}

module kitt_proposals_api_roles_kitt_sql 'kitt-proposals-api-roles-kitt-sql/kitt-proposals-api-roles-kitt-sql.bicep' = {
  name: 'kitt-proposals-api-roles-kitt-sql'
  scope: resourceGroup(KittAzureSqlResourceGroup)
  params: {
    location: location
  }
}

module kitt_webapp_identity 'kitt-webapp-identity/kitt-webapp-identity.bicep' = {
  name: 'kitt-webapp-identity'
  scope: rg
  params: {
    location: location
  }
}

module kitt_webapp_roles_kitt_sql 'kitt-webapp-roles-kitt-sql/kitt-webapp-roles-kitt-sql.bicep' = {
  name: 'kitt-webapp-roles-kitt-sql'
  scope: resourceGroup(KittAzureSqlResourceGroup)
  params: {
    location: location
  }
}

output kitt_env_AZURE_CONTAINER_REGISTRY_NAME string = kitt_env.outputs.AZURE_CONTAINER_REGISTRY_NAME

output kitt_env_AZURE_CONTAINER_REGISTRY_ENDPOINT string = kitt_env.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT

output kitt_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = kitt_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID

output kitt_env_AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = kitt_env.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN

output kitt_env_AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = kitt_env.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID

output kitt_cms_api_identity_id string = kitt_cms_api_identity.outputs.id

output kitt_sql_sqlServerFqdn string = kitt_sql.outputs.sqlServerFqdn

output kitt_cms_api_identity_clientId string = kitt_cms_api_identity.outputs.clientId

output kitt_proposals_api_identity_id string = kitt_proposals_api_identity.outputs.id

output kitt_proposals_api_identity_clientId string = kitt_proposals_api_identity.outputs.clientId

output kitt_webapp_identity_id string = kitt_webapp_identity.outputs.id

output kitt_webapp_identity_clientId string = kitt_webapp_identity.outputs.clientId