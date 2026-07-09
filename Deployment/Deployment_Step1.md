# Playbook: ASP.NET Core → Docker → AKS → Azure DevOps CI/CD



## Prerequisites

- [dotnet SDK](https://dotnet.microsoft.com/download) (used: 9.0.304)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — must be **running** before any `docker build`
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) — logged in via `az login`
- `kubectl` (ships with Docker Desktop, or install separately)
- An Azure subscription with permission to create resource groups
- An Azure DevOps organization + project (see step 8)

Verify tools:
```bash
dotnet --version
docker version --format '{{.Server.Version}}'
az account show
kubectl version --client
```

## 1. Scaffold the app

```bash
dotnet new webapi -n HelloAzureApi -o . --use-controllers false
dotnet build
```

This creates `Program.cs`, `HelloAzureApi.csproj`, and the default `/weatherforecast` minimal API.

## 2. Add a Dockerfile

`Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY HelloAzureApi.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HelloAzureApi.dll"]
```

`.dockerignore`:
```
bin/
obj/
.claude/
*.md
```

## 3. Build and test the container locally

```bash
docker build -t helloazureapi:local .
docker run -d --name helloazure-test -p 18080:8080 helloazureapi:local
curl http://localhost:18080/weatherforecast
docker rm -f helloazure-test
```

Use a host port other than 8080 if something else already owns it locally.

## 4. Provision Azure resources

Pick a region and a **globally unique** ACR name.

```bash
az group create --name helloazure-rg --location eastus

# ACR name must be globally unique, alphanumeric only
az acr create --resource-group helloazure-rg --name helloazureacr26957 --sku Basic

# Minimal, cost-conscious cluster: 1 node, burstable VM, ACR pre-attached
az aks create \
  --resource-group helloazure-rg \
  --name helloazure-aks \
  --node-count 1 \
  --node-vm-size Standard_B2s \
  --attach-acr helloazureacr26957 \
  --generate-ssh-keys \
  --tier free
```

`az aks create` takes 5–10 minutes. Cost note: a running AKS cluster + LoadBalancer bills continuously (roughly $70–150+/month depending on size) — delete resources when done experimenting (`az group delete --name helloazure-rg`).

## 5. Push the image to ACR

```bash
az acr login --name helloazureacr26957
docker tag helloazureapi:local helloazureacr26957.azurecr.io/helloazureapi:v1
docker push helloazureacr26957.azurecr.io/helloazureapi:v1
```

## 6. Kubernetes manifests

`k8s/deployment.yaml`:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: helloazureapi
  labels:
    app: helloazureapi
spec:
  replicas: 1
  selector:
    matchLabels:
      app: helloazureapi
  template:
    metadata:
      labels:
        app: helloazureapi
    spec:
      containers:
        - name: helloazureapi
          image: helloazureacr26957.azurecr.io/helloazureapi:v1
          ports:
            - containerPort: 8080
          resources:
            requests:
              cpu: 100m
              memory: 128Mi
            limits:
              cpu: 250m
              memory: 256Mi
```

`k8s/service.yaml`:
```yaml
apiVersion: v1
kind: Service
metadata:
  name: helloazureapi
spec:
  type: LoadBalancer
  selector:
    app: helloazureapi
  ports:
    - protocol: TCP
      port: 80
      targetPort: 8080
```

## 7. Deploy and verify

```bash
az aks get-credentials --resource-group helloazure-rg --name helloazure-aks --overwrite-existing
kubectl apply -f k8s/deployment.yaml -f k8s/service.yaml
kubectl wait --for=condition=Ready pod -l app=helloazureapi --timeout=90s
kubectl get svc helloazureapi   # note the EXTERNAL-IP
curl http://<EXTERNAL-IP>/weatherforecast
```

At this point the app is live, deployed manually. Everything after this sets up CI/CD so future pushes deploy automatically.

## 8. Create an Azure DevOps org and project

1. Go to https://dev.azure.com, sign in with the Microsoft account tied to your Azure subscription.
2. Create an organization (or use the auto-created one) — pick a unique name and a region.
3. Create a project (e.g. `SampleAPI`).

## 9. Initialize git and push the code

Azure DevOps orgs backed by a **Microsoft account** (not Entra ID/work account) require a **Personal Access Token (PAT)** for git operations — an `az account get-access-token` bearer token will not authenticate.

1. Create a PAT: **User settings → Personal access tokens → New Token**, scopes: `Code (Read & Write)`, `Build (Read & execute)`, `Service Connections (Read, query & manage)`.
2. Initialize and push:

```bash
git init
git add -A
git commit -m "Initial commit"
git remote add origin https://dev.azure.com/<your-org>/<your-project>/_git/<your-repo>
git push -u origin master
# username: your Azure DevOps email, password: the PAT
```

`.gitignore` used:
```
bin/
obj/
.vs/
*.user
.claude/
```

## 10. Create the Azure service connection

In the Azure DevOps project:
1. **Project Settings → Service connections → New service connection**
2. Choose **Azure Resource Manager → Service principal (automatic)**
3. Select your subscription, scope to the resource group (`helloazure-rg`)
4. Name it `AzureServiceConnection` (or update the pipeline variable to match)
5. Grant access permission to all pipelines

This is the only service connection needed — the pipeline uses `az acr build` and `az aks get-credentials`, so no separate Docker registry connection is required.

## 11. Pipeline YAML

Template the image tag in `k8s/deployment.yaml` so the pipeline controls versioning:
```yaml
image: __ACR_LOGIN_SERVER__/helloazureapi:__IMAGE_TAG__
```

`azure-pipelines.yml`:
```yaml
trigger:
  branches:
    include:
      - master

pool:
  vmImage: ubuntu-latest

variables:
  azureServiceConnection: 'AzureServiceConnection'
  resourceGroup: 'helloazure-rg'
  acrName: 'helloazureacr26957'
  aksName: 'helloazure-aks'
  imageName: 'helloazureapi'
  imageTag: '$(Build.BuildId)'

stages:
  - stage: Build
    displayName: Build and push image to ACR
    jobs:
      - job: BuildAndPush
        steps:
          - task: AzureCLI@2
            displayName: az acr build
            inputs:
              azureSubscription: $(azureServiceConnection)
              scriptType: bash
              scriptLocation: inlineScript
              inlineScript: |
                az acr build \
                  --registry $(acrName) \
                  --image $(imageName):$(imageTag) \
                  .

  - stage: Deploy
    displayName: Deploy to AKS
    dependsOn: Build
    jobs:
      - job: DeployToAks
        steps:
          - task: AzureCLI@2
            displayName: kubectl apply
            inputs:
              azureSubscription: $(azureServiceConnection)
              scriptType: bash
              scriptLocation: inlineScript
              inlineScript: |
                az aks get-credentials \
                  --resource-group $(resourceGroup) \
                  --name $(aksName) \
                  --overwrite-existing

                sed -i \
                  -e "s#__ACR_LOGIN_SERVER__#$(acrName).azurecr.io#g" \
                  -e "s#__IMAGE_TAG__#$(imageTag)#g" \
                  k8s/deployment.yaml

                kubectl apply -f k8s/deployment.yaml -f k8s/service.yaml
                kubectl rollout status deployment/helloazureapi --timeout=120s
```

Commit and push this file.

## 12. Create and run the pipeline

1. **Pipelines → New pipeline → Azure Repos Git →** select your repo
2. **Existing Azure Pipelines YAML file** → branch `master`, path `/azure-pipelines.yml`
3. Save and run

From then on, every push to `master` builds a new image tagged with the DevOps build number and rolls it out to AKS automatically.

## Cleanup (avoid ongoing charges)

```bash
az group delete --name helloazure-rg --yes --no-wait
```