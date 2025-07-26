@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource kitt_proposals_api_identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: take('kitt_proposals_api_identity-${uniqueString(resourceGroup().id)}', 128)
  location: location
}

output id string = kitt_proposals_api_identity.id

output clientId string = kitt_proposals_api_identity.properties.clientId

output principalId string = kitt_proposals_api_identity.properties.principalId

output principalName string = kitt_proposals_api_identity.name