@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param KittAzureSqlName string

resource kitt_sql 'Microsoft.Sql/servers@2021-11-01' existing = {
  name: KittAzureSqlName
}

resource sqlFirewallRule_AllowAllAzureIps 'Microsoft.Sql/servers/firewallRules@2021-11-01' = {
  name: 'AllowAllAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
  parent: kitt_sql
}

resource KittDatabase 'Microsoft.Sql/servers/databases@2023-08-01' = {
  name: 'KittDatabase'
  location: location
  properties: {
    freeLimitExhaustionBehavior: 'AutoPause'
    useFreeLimit: true
  }
  sku: {
    name: 'GP_S_Gen5_2'
  }
  parent: kitt_sql
}

output sqlServerFqdn string = kitt_sql.properties.fullyQualifiedDomainName

output name string = KittAzureSqlName

output sqlServerAdminName string = kitt_sql.properties.administrators.login