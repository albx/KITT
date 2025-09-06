@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param kitt_env_outputs_azure_container_apps_environment_default_domain string

param kitt_env_outputs_azure_container_apps_environment_id string

param kitt_env_outputs_azure_container_registry_endpoint string

param kitt_env_outputs_azure_container_registry_managed_identity_id string

param kitt_webapp_containerimage string

param kitt_webapp_containerport string

@secure()
param kittdatabase_connectionstring string

@secure()
param entraidtenantid_value string

@secure()
param entraiddomainname_value string

@secure()
param webappid_value string

@secure()
param webappsecret_value string

@secure()
param cmsapiappid_value string

@secure()
param proposalsapiappid_value string

resource kitt_webapp 'Microsoft.App/containerApps@2025-02-02-preview' = {
  name: 'kitt-webapp'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'connectionstrings--kittdatabase'
          value: kittdatabase_connectionstring
        }
        {
          name: 'identity--tenantid'
          value: entraidtenantid_value
        }
        {
          name: 'identity--domainname'
          value: entraiddomainname_value
        }
        {
          name: 'identity--webapp--appid'
          value: webappid_value
        }
        {
          name: 'identity--webapp--appsecret'
          value: webappsecret_value
        }
        {
          name: 'identity--cms--appid'
          value: cmsapiappid_value
        }
        {
          name: 'identity--proposals--appid'
          value: proposalsapiappid_value
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: int(kitt_webapp_containerport)
        transport: 'http'
      }
      registries: [
        {
          server: kitt_env_outputs_azure_container_registry_endpoint
          identity: kitt_env_outputs_azure_container_registry_managed_identity_id
        }
      ]
      runtime: {
        dotnet: {
          autoConfigureDataProtection: true
        }
      }
    }
    environmentId: kitt_env_outputs_azure_container_apps_environment_id
    template: {
      containers: [
        {
          image: kitt_webapp_containerimage
          name: 'kitt-webapp'
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
              value: kitt_webapp_containerport
            }
            {
              name: 'services__kitt-cms-api__http__0'
              value: 'http://kitt-cms-api.internal.${kitt_env_outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'services__kitt-cms-api__https__0'
              value: 'https://kitt-cms-api.internal.${kitt_env_outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'services__kitt-proposals-api__http__0'
              value: 'http://kitt-proposals-api.internal.${kitt_env_outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'services__kitt-proposals-api__https__0'
              value: 'https://kitt-proposals-api.internal.${kitt_env_outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'ConnectionStrings__KittDatabase'
              secretRef: 'connectionstrings--kittdatabase'
            }
            {
              name: 'Identity__TenantId'
              secretRef: 'identity--tenantid'
            }
            {
              name: 'Identity__DomainName'
              secretRef: 'identity--domainname'
            }
            {
              name: 'Identity__WebApp__AppId'
              secretRef: 'identity--webapp--appid'
            }
            {
              name: 'Identity__WebApp__AppSecret'
              secretRef: 'identity--webapp--appsecret'
            }
            {
              name: 'Identity__Cms__AppId'
              secretRef: 'identity--cms--appid'
            }
            {
              name: 'Identity__Proposals__AppId'
              secretRef: 'identity--proposals--appid'
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
      '${kitt_env_outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}