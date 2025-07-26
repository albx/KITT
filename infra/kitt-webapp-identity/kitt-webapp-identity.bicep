@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource kitt_webapp_identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: take('kitt_webapp_identity-${uniqueString(resourceGroup().id)}', 128)
  location: location
}

output id string = kitt_webapp_identity.id

output clientId string = kitt_webapp_identity.properties.clientId

output principalId string = kitt_webapp_identity.properties.principalId

output principalName string = kitt_webapp_identity.name