@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param kitt_env_outputs_azure_container_apps_environment_default_domain string

param kitt_env_outputs_azure_container_apps_environment_id string

param kitt_env_outputs_azure_container_registry_endpoint string

param kitt_env_outputs_azure_container_registry_managed_identity_id string

param kitt_proposals_api_containerimage string

param kitt_proposals_api_identity_outputs_id string

param kitt_proposals_api_containerport string

param kitt_sql_outputs_sqlserverfqdn string

@secure()
param entraidtenantid_value string

@secure()
param proposalsapiappid_value string

param kitt_proposals_api_identity_outputs_clientid string

resource kitt_proposals_api 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'kitt-proposals-api'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'identity--tenantid'
          value: entraidtenantid_value
        }
        {
          name: 'identity--proposals--appid'
          value: proposalsapiappid_value
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: false
        targetPort: int(kitt_proposals_api_containerport)
        transport: 'http'
      }
      registries: [
        {
          server: kitt_env_outputs_azure_container_registry_endpoint
          identity: kitt_env_outputs_azure_container_registry_managed_identity_id
        }
      ]
    }
    environmentId: kitt_env_outputs_azure_container_apps_environment_id
    template: {
      containers: [
        {
          image: kitt_proposals_api_containerimage
          name: 'kitt-proposals-api'
          env: [
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY'
              value: 'in_memory'
            }
            {
              name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
              value: 'true'
            }
            {
              name: 'HTTP_PORTS'
              value: kitt_proposals_api_containerport
            }
            {
              name: 'ConnectionStrings__KittDatabase'
              value: 'Server=tcp:${kitt_sql_outputs_sqlserverfqdn},1433;Encrypt=True;Authentication="Active Directory Default";Database=KittDatabase'
            }
            {
              name: 'Identity__TenantId'
              secretRef: 'identity--tenantid'
            }
            {
              name: 'Identity__Proposals__AppId'
              secretRef: 'identity--proposals--appid'
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: kitt_proposals_api_identity_outputs_clientid
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${kitt_proposals_api_identity_outputs_id}': { }
      '${kitt_env_outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}