@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param kitt_env_outputs_azure_container_apps_environment_default_domain string

param kitt_env_outputs_azure_container_apps_environment_id string

param kitt_env_outputs_azure_container_registry_endpoint string

param kitt_env_outputs_azure_container_registry_managed_identity_id string

param kitt_cms_api_containerimage string

param kitt_cms_api_identity_outputs_id string

param kitt_cms_api_containerport string

param kitt_sql_outputs_sqlserverfqdn string

@secure()
param entraidtenantid_value string

@secure()
param cmsapiappid_value string

param kitt_cms_api_identity_outputs_clientid string

resource kitt_cms_api 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'kitt-cms-api'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'identity--tenantid'
          value: entraidtenantid_value
        }
        {
          name: 'identity--cms--appid'
          value: cmsapiappid_value
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: false
        targetPort: int(kitt_cms_api_containerport)
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
          image: kitt_cms_api_containerimage
          name: 'kitt-cms-api'
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
              value: kitt_cms_api_containerport
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
              name: 'Identity__Cms__AppId'
              secretRef: 'identity--cms--appid'
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: kitt_cms_api_identity_outputs_clientid
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
      '${kitt_cms_api_identity_outputs_id}': { }
      '${kitt_env_outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}