# Deploy to Azure con .NET Container Support

## Richiesta Iniziale
L'utente ha richiesto una GitHub Action che:
- Richiami i file Bicep presenti nella cartella [`infra`](infra )
- Crei le immagini Docker dei progetti
- Le pushi sulle istanze di Azure Container Registry definite nei Bicep

## Prima Soluzione Proposta
Ho inizialmente suggerito una GitHub Action completa con Dockerfile tradizionali, includendo:

### GitHub Action completa
```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
  workflow_dispatch:

env:
  AZURE_SUBSCRIPTION_ID: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
  AZURE_TENANT_ID: ${{ secrets.AZURE_TENANT_ID }}
  AZURE_CLIENT_ID: ${{ secrets.AZURE_CLIENT_ID }}
  AZURE_CLIENT_SECRET: ${{ secrets.AZURE_CLIENT_SECRET }}
  RESOURCE_GROUP: "kitt-rg"
  LOCATION: "Italy North"

jobs:
  deploy-infrastructure:
    runs-on: ubuntu-latest
    outputs:
      container-registry-name: ${{ steps.deploy.outputs.containerRegistryName }}
      container-registry-endpoint: ${{ steps.deploy.outputs.containerRegistryEndpoint }}
    steps:
    - uses: actions/checkout@v4

    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Deploy Bicep Infrastructure
      id: deploy
      uses: azure/arm-deploy@v1
      with:
        subscriptionId: ${{ env.AZURE_SUBSCRIPTION_ID }}
        resourceGroupName: ${{ env.RESOURCE_GROUP }}
        template: ./infra/main.bicep
        parameters: >
          resourceGroupName=${{ env.RESOURCE_GROUP }}
          location="${{ env.LOCATION }}"
          principalId=${{ secrets.AZURE_PRINCIPAL_ID }}
          KittAzureSqlResourceGroup=${{ secrets.KITT_SQL_RESOURCE_GROUP }}
          KittAzureSqlName=${{ secrets.KITT_SQL_NAME }}
        deploymentMode: Incremental

  build-and-push-images:
    needs: deploy-infrastructure
    runs-on: ubuntu-latest
    strategy:
      matrix:
        project:
          - name: "kitt-webapp"
            dockerfile: "./src/KITT.Web.App/KITT.Web.App/Dockerfile"
            context: "."
          - name: "kitt-cms-api"
            dockerfile: "./src/KITT.Cms.Web.Api/Dockerfile"
            context: "."
          - name: "kitt-proposals-api"
            dockerfile: "./src/KITT.Proposals.Web.Api/Dockerfile"
            context: "."
    
    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Login to Azure Container Registry
      run: |
        az acr login --name ${{ needs.deploy-infrastructure.outputs.container-registry-name }}

    - name: Build and Push Docker Image
      run: |
        IMAGE_TAG="${{ needs.deploy-infrastructure.outputs.container-registry-endpoint }}/${{ matrix.project.name }}:${{ github.sha }}"
        docker build -f ${{ matrix.project.dockerfile }} -t $IMAGE_TAG ${{ matrix.project.context }}
        docker push $IMAGE_TAG

  deploy-container-apps:
    needs: [deploy-infrastructure, build-and-push-images]
    runs-on: ubuntu-latest
    strategy:
      matrix:
        app:
          - name: "kitt-webapp"
            bicep: "./infra/kitt-webapp/kitt-webapp.bicep"
          - name: "kitt-cms-api"
            bicep: "./infra/kitt-cms-api/kitt-cms-api.bicep"
          - name: "kitt-proposals-api"
            bicep: "./infra/kitt-proposals-api/kitt-proposals-api.bicep"
    
    steps:
    - uses: actions/checkout@v4

    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Deploy Container App
      uses: azure/arm-deploy@v1
      with:
        subscriptionId: ${{ env.AZURE_SUBSCRIPTION_ID }}
        resourceGroupName: ${{ env.RESOURCE_GROUP }}
        template: ${{ matrix.app.bicep }}
        parameters: >
          location="${{ env.LOCATION }}"
          ${{ matrix.app.name }}_containerimage="${{ needs.deploy-infrastructure.outputs.container-registry-endpoint }}/${{ matrix.app.name }}:${{ github.sha }}"
          kitt_env_outputs_azure_container_registry_endpoint="${{ needs.deploy-infrastructure.outputs.container-registry-endpoint }}"
          entraidtenantid_value="${{ secrets.ENTRA_ID_TENANT_ID }}"
          entraiddomainname_value="${{ secrets.ENTRA_ID_DOMAIN_NAME }}"
          webappid_value="${{ secrets.WEB_APP_ID }}"
          webappsecret_value="${{ secrets.WEB_APP_SECRET }}"
          cmsapiappid_value="${{ secrets.CMS_API_APP_ID }}"
          proposalsapiappid_value="${{ secrets.PROPOSALS_API_APP_ID }}"
        deploymentMode: Incremental
```

### Secrets richiesti
```
AZURE_CREDENTIALS
AZURE_SUBSCRIPTION_ID
AZURE_TENANT_ID
AZURE_CLIENT_ID
AZURE_CLIENT_SECRET
AZURE_PRINCIPAL_ID
KITT_SQL_RESOURCE_GROUP
KITT_SQL_NAME
ENTRA_ID_TENANT_ID
ENTRA_ID_DOMAIN_NAME
WEB_APP_ID
WEB_APP_SECRET
CMS_API_APP_ID
PROPOSALS_API_APP_ID
```

## Richiesta di Modifica
L'utente ha richiesto di evitare i Dockerfile e utilizzare le funzionalità native di .NET per la containerizzazione.

## Soluzione Finale con .NET Container Support

### 1. Modifiche ai file .csproj

#### KITT.Web.App.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Container Properties -->
    <EnableSdkContainerSupport>true</EnableSdkContainerSupport>
    <ContainerImageName>kitt-webapp</ContainerImageName>
    <ContainerImageTag>latest</ContainerImageTag>
    <ContainerBaseImage>mcr.microsoft.com/dotnet/aspnet:9.0</ContainerBaseImage>
    <ContainerWorkingDirectory>/app</ContainerWorkingDirectory>
  </PropertyGroup>

  <!-- ...existing code... -->
</Project>
```

#### KITT.Cms.Web.Api.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Container Properties -->
    <EnableSdkContainerSupport>true</EnableSdkContainerSupport>
    <ContainerImageName>kitt-cms-api</ContainerImageName>
    <ContainerImageTag>latest</ContainerImageTag>
    <ContainerBaseImage>mcr.microsoft.com/dotnet/aspnet:9.0</ContainerBaseImage>
    <ContainerWorkingDirectory>/app</ContainerWorkingDirectory>
  </PropertyGroup>

  <!-- ...existing code... -->
</Project>
```

#### KITT.Proposals.Web.Api.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Container Properties -->
    <EnableSdkContainerSupport>true</EnableSdkContainerSupport>
    <ContainerImageName>kitt-proposals-api</ContainerImageName>
    <ContainerImageTag>latest</ContainerImageTag>
    <ContainerBaseImage>mcr.microsoft.com/dotnet/aspnet:9.0</ContainerBaseImage>
    <ContainerWorkingDirectory>/app</ContainerWorkingDirectory>
  </PropertyGroup>

  <!-- ...existing code... -->
</Project>
```

### 2. GitHub Action aggiornata

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
  workflow_dispatch:

env:
  AZURE_SUBSCRIPTION_ID: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
  AZURE_TENANT_ID: ${{ secrets.AZURE_TENANT_ID }}
  AZURE_CLIENT_ID: ${{ secrets.AZURE_CLIENT_ID }}
  AZURE_CLIENT_SECRET: ${{ secrets.AZURE_CLIENT_SECRET }}
  RESOURCE_GROUP: "kitt-rg"
  LOCATION: "Italy North"

jobs:
  deploy-infrastructure:
    runs-on: ubuntu-latest
    outputs:
      container-registry-name: ${{ steps.deploy.outputs.containerRegistryName }}
      container-registry-endpoint: ${{ steps.deploy.outputs.containerRegistryEndpoint }}
    steps:
    - uses: actions/checkout@v4

    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Deploy Bicep Infrastructure
      id: deploy
      uses: azure/arm-deploy@v1
      with:
        subscriptionId: ${{ env.AZURE_SUBSCRIPTION_ID }}
        resourceGroupName: ${{ env.RESOURCE_GROUP }}
        template: ./infra/main.bicep
        parameters: >
          resourceGroupName=${{ env.RESOURCE_GROUP }}
          location="${{ env.LOCATION }}"
          principalId=${{ secrets.AZURE_PRINCIPAL_ID }}
          KittAzureSqlResourceGroup=${{ secrets.KITT_SQL_RESOURCE_GROUP }}
          KittAzureSqlName=${{ secrets.KITT_SQL_NAME }}
        deploymentMode: Incremental

  build-and-push-images:
    needs: deploy-infrastructure
    runs-on: ubuntu-latest
    strategy:
      matrix:
        project:
          - name: "kitt-webapp"
            path: "./src/KITT.Web.App/KITT.Web.App"
            csproj: "KITT.Web.App.csproj"
          - name: "kitt-cms-api"
            path: "./src/KITT.Cms.Web.Api"
            csproj: "KITT.Cms.Web.Api.csproj"
          - name: "kitt-proposals-api"
            path: "./src/KITT.Proposals.Web.Api"
            csproj: "KITT.Proposals.Web.Api.csproj"
    
    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Login to Azure Container Registry
      run: |
        az acr login --name ${{ needs.deploy-infrastructure.outputs.container-registry-name }}

    - name: Build and Push Container using .NET SDK
      run: |
        REGISTRY_ENDPOINT="${{ needs.deploy-infrastructure.outputs.container-registry-endpoint }}"
        IMAGE_TAG="${REGISTRY_ENDPOINT}/${{ matrix.project.name }}:${{ github.sha }}"
        
        cd ${{ matrix.project.path }}
        dotnet publish ${{ matrix.project.csproj }} \
          --os linux \
          --arch x64 \
          -p:PublishProfile=DefaultContainer \
          -p:ContainerImageName=${{ matrix.project.name }} \
          -p:ContainerImageTag=${{ github.sha }} \
          -p:ContainerRegistry=${REGISTRY_ENDPOINT}

  deploy-container-apps:
    needs: [deploy-infrastructure, build-and-push-images]
    runs-on: ubuntu-latest
    strategy:
      matrix:
        app:
          - name: "kitt-webapp"
            bicep: "./infra/kitt-webapp/kitt-webapp.bicep"
          - name: "kitt-cms-api"
            bicep: "./infra/kitt-cms-api/kitt-cms-api.bicep"
          - name: "kitt-proposals-api"
            bicep: "./infra/kitt-proposals-api/kitt-proposals-api.bicep"
    
    steps:
    - uses: actions/checkout@v4

    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Deploy Container App
      uses: azure/arm-deploy@v1
      with:
        subscriptionId: ${{ env.AZURE_SUBSCRIPTION_ID }}
        resourceGroupName: ${{ env.RESOURCE_GROUP }}
        template: ${{ matrix.app.bicep }}
        parameters: >
          location="${{ env.LOCATION }}"
          ${{ matrix.app.name }}_containerimage="${{ needs.deploy-infrastructure.outputs.container-registry-endpoint }}/${{ matrix.app.name }}:${{ github.sha }}"
          kitt_env_outputs_azure_container_registry_endpoint="${{ needs.deploy-infrastructure.outputs.container-registry-endpoint }}"
          entraidtenantid_value="${{ secrets.ENTRA_ID_TENANT_ID }}"
          entraiddomainname_value="${{ secrets.ENTRA_ID_DOMAIN_NAME }}"
          webappid_value="${{ secrets.WEB_APP_ID }}"
          webappsecret_value="${{ secrets.WEB_APP_SECRET }}"
          cmsapiappid_value="${{ secrets.CMS_API_APP_ID }}"
          proposalsapiappid_value="${{ secrets.PROPOSALS_API_APP_ID }}"
        deploymentMode: Incremental
```

### 3. Package NuGet da aggiungere
```xml
<PackageReference Include="Microsoft.NET.Build.Containers" Version="7.0.401" />
```

### 4. Script PowerShell per test locale
```powershell
# build-containers.ps1
Write-Host "Building KITT Web App container..."
dotnet publish src/KITT.Web.App/KITT.Web.App/KITT.Web.App.csproj `
  --os linux --arch x64 `
  -p:PublishProfile=DefaultContainer `
  -p:ContainerImageName=kitt-webapp `
  -p:ContainerImageTag=local

Write-Host "Building KITT CMS API container..."
dotnet publish src/KITT.Cms.Web.Api/KITT.Cms.Web.Api.csproj `
  --os linux --arch x64 `
  -p:PublishProfile=DefaultContainer `
  -p:ContainerImageName=kitt-cms-api `
  -p:ContainerImageTag=local

Write-Host "Building KITT Proposals API container..."
dotnet publish src/KITT.Proposals.Web.Api/KITT.Proposals.Web.Api.csproj `
  --os linux --arch x64 `
  -p:PublishProfile=DefaultContainer `
  -p:ContainerImageName=kitt-proposals-api `
  -p:ContainerImageTag=local

Write-Host "All containers built successfully!"
```

## Vantaggi della soluzione finale

1. **No Dockerfile**: .NET genera automaticamente l'immagine container
2. **Ottimizzazione automatica**: .NET applica le migliori pratiche per le immagini
3. **Consistenza**: Stesso processo per tutti i progetti
4. **Manutenzione ridotta**: Non devi mantenere Dockerfile separati
5. **Sicurezza**: Usa immagini base Microsoft ottimizzate

Il processo è completamente automatizzato: ogni push su main triggera la build dell'infrastruttura, la creazione delle immagini container e il deployment su Azure Container Apps.
