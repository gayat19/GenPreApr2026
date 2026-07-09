# Playbook: ASP.NET Core → Docker → AKS → Azure DevOps CI/CD → Postgres + Blob Storage + Key Vault

Reproduces this project end-to-end, from an empty folder to a file-upload/download API running on AKS, backed by Postgres and Blob Storage, with connection strings sourced from Key Vault and a CI/CD pipeline that builds, tests, and deploys automatically. Values below match what was actually created — swap in your own names/regions where noted.

Resources created along the way: resource group `helloazure-rg` (eastus), ACR `helloazureacr26957`, AKS `helloazure-aks`, Postgres Flexible Server `helloazure-pg` (eastus2), Storage account `helloazureblob26957`, Key Vault `helloazure-kv26957`, Azure DevOps project `SampleAPI`.

## Prerequisites

- [dotnet SDK](https://dotnet.microsoft.com/download) (used: 9.0.304)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — must be **running** before any `docker build`
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) — logged in via `az login`
- `kubectl` (ships with Docker Desktop, or install separately)
- `dotnet-ef` global tool: `dotnet tool install --global dotnet-ef --version 9.0.9`
- An Azure subscription with permission to create resource groups
- An Azure DevOps organization + project (created in Part 2)

Verify tools:
```bash
dotnet --version
docker version --format '{{.Server.Version}}'
az account show
kubectl version --client
```

---

# Part 1 — Base app, container, and AKS deployment

## 1. Scaffold the app

```bash
dotnet new webapi -n HelloAzureApi -o . --use-controllers false
dotnet build
```

Creates `Program.cs`, `HelloAzureApi.csproj`, and the default `/weatherforecast` minimal API.

## 2. Add a Dockerfile

`Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY HelloAzureApi.csproj .
RUN dotnet restore HelloAzureApi.csproj
COPY . .
RUN dotnet publish HelloAzureApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HelloAzureApi.dll"]
```
(The `dotnet restore/publish HelloAzureApi.csproj` explicit project name — rather than a bare `dotnet publish` — matters once a second `.csproj` exists, e.g. a test project; otherwise the SDK can't tell which project to build.)

`.dockerignore`:
```
bin/
obj/
.claude/
*.md
tests/
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
Takes 5–10 minutes. Cost note: a running AKS cluster + LoadBalancer bills continuously (roughly $70–150+/month) — see Cleanup at the end.

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
The app is now live, deployed manually. Part 2 sets up CI/CD; Part 3 adds the real feature.

---

# Part 2 — CI/CD with Azure DevOps

## 8. Create an Azure DevOps org and project

1. Go to https://dev.azure.com, sign in with the Microsoft account tied to your Azure subscription.
2. Create an organization (or use the auto-created one).
3. Create a project (e.g. `SampleAPI`).

## 9. Initialize git and push the code

Azure DevOps orgs backed by a **Microsoft account** (not Entra ID/work account) require a **Personal Access Token (PAT)** for git operations — an `az account get-access-token` bearer token will not authenticate (returns 302/redirect on API calls, `terminal prompts disabled` on push).

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

`.gitignore`:
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
2. **Azure Resource Manager → Service principal (automatic)**
3. Select your subscription, scope to the resource group (`helloazure-rg`)
4. Name it `AzureServiceConnection`
5. Grant access permission to all pipelines

This one connection is enough for everything in this playbook — pipeline steps use `az acr build`, `az aks get-credentials`, and `az keyvault secret show`, all under the same service principal. No separate Docker registry or Key Vault service connection is needed.

## 11. First pipeline version

Template the image tag in `k8s/deployment.yaml`:
```yaml
image: __ACR_LOGIN_SERVER__/helloazureapi:__IMAGE_TAG__
```

`azure-pipelines.yml` (base version — extended further in Part 3):
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
                az acr build --registry $(acrName) --image $(imageName):$(imageTag) .

  - stage: Deploy
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
                az aks get-credentials --resource-group $(resourceGroup) --name $(aksName) --overwrite-existing
                sed -i -e "s#__ACR_LOGIN_SERVER__#$(acrName).azurecr.io#g" -e "s#__IMAGE_TAG__#$(imageTag)#g" k8s/deployment.yaml
                kubectl apply -f k8s/deployment.yaml -f k8s/service.yaml
                kubectl rollout status deployment/helloazureapi --timeout=120s
```

## 12. Create and run the pipeline

1. **Pipelines → New pipeline → Azure Repos Git →** select your repo
2. **Existing Azure Pipelines YAML file** → branch `master`, path `/azure-pipelines.yml`
3. Save and run

From here, every push to `master` builds a new image and rolls it out to AKS automatically.

---

# Part 3 — Upgrade: Postgres + Blob Storage + Key Vault file upload/download

Feature: `POST /files` uploads a file's bytes to Blob Storage and records filename/content-type/upload-date in Postgres; `GET /files/{id}` downloads it back. Both services' connection strings live in Key Vault. **Architecture decision made for this project**: the deploy pipeline — not the running app — reads the two secrets from Key Vault and injects them into a Kubernetes Secret; the pod only ever sees plain env vars. (The alternative, AKS Workload Identity with the app calling the Key Vault SDK directly at runtime, is more "secretless" but requires enabling OIDC issuer/workload identity on the cluster and a federated credential — more moving parts than needed here.)

## 13. Provision Postgres, Storage Account, Key Vault

```bash
# Postgres flexible server requires this RP; check/register once per subscription
az provider register --namespace Microsoft.DBforPostgreSQL
az provider show -n Microsoft.DBforPostgreSQL --query registrationState -o tsv   # poll until "Registered"

# Generate a strong alphanumeric-only password (avoids connection-string/shell-quoting bugs)
PG_PASSWORD=$(LC_ALL=C tr -dc 'A-Za-z0-9' < /dev/urandom | head -c 24)

# Postgres Flexible Server — note: some regions restrict flexible-server capacity.
# eastus failed with "location is restricted for provisioning of flexible servers"
# on this subscription; eastus2 worked. Cross-region from AKS is fine functionally.
az postgres flexible-server create \
  --resource-group helloazure-rg \
  --name helloazure-pg \
  --location eastus2 \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --storage-size 32 \
  --version 16 \
  --admin-user pgadmin \
  --admin-password "$PG_PASSWORD" \
  --public-access None \
  --yes
# NOTE: omit --database-name here and it creates a default "flexibleserverdb" —
# create your own named database explicitly:
az postgres flexible-server db create --resource-group helloazure-rg --server-name helloazure-pg --database-name helloazureapi

# Firewall: scope to the AKS cluster's OUTBOUND IP specifically (not 0.0.0.0/0).
# This is NOT the same as the app's inbound LoadBalancer IP — find it in the
# node resource group (MC_<rg>_<aks-name>_<region>) as the "-slb-managed-outbound-ip".
az postgres flexible-server firewall-rule create \
  --resource-group helloazure-rg \
  --name helloazure-pg \
  --rule-name AllowAksOutbound \
  --start-ip-address <AKS_OUTBOUND_IP> \
  --end-ip-address <AKS_OUTBOUND_IP>

# Storage account + container
az storage account create --resource-group helloazure-rg --name helloazureblob26957 --location eastus --sku Standard_LRS --kind StorageV2
az storage container create --account-name helloazureblob26957 --name uploads --auth-mode login

# Key Vault (RBAC authorization mode)
az keyvault create --resource-group helloazure-rg --name helloazure-kv26957 --location eastus --enable-rbac-authorization true
```

RBAC role assignments (on Git Bash / MSYS shells, prefix with `export MSYS_NO_PATHCONV=1` or resource-ID arguments starting with `/subscriptions/...` get mangled into a Windows path):
```bash
export MSYS_NO_PATHCONV=1
KV_ID=$(az keyvault show -n helloazure-kv26957 -g helloazure-rg --query id -o tsv)

# You need write access to add secrets
MY_OBJECT_ID=$(az ad signed-in-user show --query id -o tsv)
az role assignment create --assignee-object-id "$MY_OBJECT_ID" --assignee-principal-type User --role "Key Vault Secrets Officer" --scope "$KV_ID"

# The pipeline's service principal needs read access (find its object id via
# `az devops service-endpoint list --project <project> -o json`, field data.spnObjectId)
az role assignment create --assignee-object-id <PIPELINE_SP_OBJECT_ID> --assignee-principal-type ServicePrincipal --role "Key Vault Secrets User" --scope "$KV_ID"
```
RBAC propagation can take a few minutes — if the pipeline's first Key Vault read fails with 403 right after granting the role, just re-run it.

Store the two secrets:
```bash
STORAGE_CONN=$(az storage account show-connection-string -g helloazure-rg -n helloazureblob26957 -o tsv --query connectionString)
PG_CONN="Host=helloazure-pg.postgres.database.azure.com;Database=helloazureapi;Username=pgadmin;Password=${PG_PASSWORD};SSL Mode=Require;Trust Server Certificate=true"

az keyvault secret set --vault-name helloazure-kv26957 --name BlobStorageConnectionString --value "$STORAGE_CONN"
az keyvault secret set --vault-name helloazure-kv26957 --name PostgresConnectionString --value "$PG_CONN"
```
Note `SSL Mode=Require;Trust Server Certificate=true` — Azure Postgres Flexible Server enforces SSL, and these are Npgsql-specific connection string keys.

## 14. Add NuGet packages

```bash
# Pin explicit 9.x versions — the latest published versions of these packages
# target net10.0 and will fail to restore against a net9.0 project.
dotnet add HelloAzureApi.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.4
dotnet add HelloAzureApi.csproj package Microsoft.EntityFrameworkCore.Design --version 9.0.9
dotnet add HelloAzureApi.csproj package Azure.Storage.Blobs
```

## 15. Project structure

```
Data/FileUpload.cs           - entity: Id (Guid), FileName, ContentType, BlobKey, UploadedAtUtc
Data/AppDbContext.cs         - DbContext with DbSet<FileUpload>
Storage/IFileStorageService.cs      - UploadAsync(blobKey, stream, contentType), OpenReadAsync(blobKey)
Storage/BlobFileStorageService.cs   - Azure.Storage.Blobs-backed implementation
Endpoints/FileEndpoints.cs   - MapFileEndpoints() extension; named static handlers (testable, not inline lambdas)
Migrations/                  - dotnet-ef output
```

Key design choices:
- The blob key is a fresh `Guid.NewGuid():N` + original extension — collision-proof regardless of what the user names the file. The *original* filename is preserved only in the Postgres row, never used as the storage key.
- `POST /files` returns a small response DTO (`{ id, fileName, contentType, uploadedAtUtc }`), never the internal blob key.
- `GET /files/{id:guid}` uses `Results.Stream(stream, contentType, fileDownloadName: fileName)`, which sets `Content-Type` and `Content-Disposition: attachment; filename="..."` automatically.

## 16. Wire up Program.cs

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddSingleton(_ =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage")));
builder.Services.AddScoped<IFileStorageService, BlobFileStorageService>();

// ...

app.MapFileEndpoints();

// Safe only because this deployment runs a single replica. If replicas ever
// increase, move migrations to a one-shot Job/pipeline step instead of running
// them from every pod on startup.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (dbContext.Database.IsRelational())   // guards against the InMemory test provider, which doesn't support Migrate()
    {
        dbContext.Database.Migrate();
    }
}
```
`ConnectionStrings:Postgres` / `ConnectionStrings:BlobStorage` config keys map to env vars `ConnectionStrings__Postgres` / `ConnectionStrings__BlobStorage` by ASP.NET Core's double-underscore convention — exactly what the K8s Secret populates in step 19.

Add `public partial class Program { }` at the end of `Program.cs` so `WebApplicationFactory<Program>` can reference it from the test project.

**Gotcha**: minimal APIs that bind `IFormFile`/form data automatically require antiforgery middleware in .NET 8/9 (CSRF protection meant for browser-rendered forms). For a stateless upload API, disable it explicitly:
```csharp
app.MapPost("/files", UploadFileAsync).DisableAntiforgery();
```
Without this, every upload fails with `InvalidOperationException: ... contains anti-forgery metadata, but a middleware was not found`.

## 17. Generate the EF Core migration

```bash
dotnet tool install --global dotnet-ef --version 9.0.9
# Migrations "add" only builds the model snapshot — it doesn't need a live DB
# connection, so a placeholder connection string is fine here:
ConnectionStrings__Postgres="Host=localhost;Database=placeholder;Username=x;Password=x" \
ConnectionStrings__BlobStorage="UseDevelopmentStorage=true" \
dotnet ef migrations add InitialCreate --project HelloAzureApi.csproj
```

## 18. Test project

```bash
dotnet new xunit -n HelloAzureApi.Tests -o tests/HelloAzureApi.Tests
cd tests/HelloAzureApi.Tests
dotnet add reference ../../HelloAzureApi.csproj
dotnet add package Moq
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 9.0.9
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 9.0.9
cd ../..
dotnet new sln -n HelloAzureApi
dotnet sln add HelloAzureApi.csproj
dotnet sln add tests/HelloAzureApi.Tests/HelloAzureApi.Tests.csproj
```

Three test classes:
- **`FileUploadServiceTests`** — calls the named static endpoint handlers directly against an EF Core `UseInMemoryDatabase` context, asserting the metadata row (filename/content-type/date) and the blob upload call (mocked `IFileStorageService`).
- **`BlobFileStorageServiceTests`** — mocks `BlobServiceClient`/`BlobContainerClient`/`BlobClient` directly with Moq (Azure SDK client methods are `virtual` by design specifically to support this).
- **`FileEndpointsIntegrationTests`** — full HTTP round trip via `WebApplicationFactory<Program>`, swapping in EF InMemory and a simple in-memory-dictionary `FakeFileStorageService` via `WithWebHostBuilder`/`ConfigureServices`.

**Gotchas hit and fixed while building this**:
1. **App project accidentally compiling the test files.** SDK-style projects glob `**/*.cs` by default; since `tests/` sits under the app's own directory, its `.cs` files got double-compiled into the main app (which lacks xunit/Moq references). Fix — exclude it explicitly in `HelloAzureApi.csproj`:
   ```xml
   <ItemGroup>
     <Compile Remove="tests/**/*.cs" />
     <Content Remove="tests/**/*" />
     <None Remove="tests/**/*" />
   </ItemGroup>
   ```
2. **"Only a single database provider can be registered" in the integration test host.** Removing just `DbContextOptions<AppDbContext>` before re-registering with `UseInMemoryDatabase` isn't enough on current EF Core — it also registers `IDbContextOptionsConfiguration<AppDbContext>` entries that accumulate across multiple `AddDbContext` calls rather than replacing. Fix — remove both:
   ```csharp
   services.RemoveAll<DbContextOptions<AppDbContext>>();
   services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
   services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(dbName));
   ```
3. **Upload succeeds but the immediate download 404s.** Root cause: `options.UseInMemoryDatabase(Guid.NewGuid().ToString())` had the `Guid.NewGuid()` call *inside* the options-configuration lambda — EF Core re-invokes that lambda every time `DbContextOptions<AppDbContext>` is resolved (i.e., per scope/per request), so every request got a fresh, empty, uniquely-named database. Fix — generate the name once, outside the lambda, and close over it:
   ```csharp
   var databaseName = Guid.NewGuid().ToString();
   services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName));
   ```
4. **NU1202 restoring `Npgsql.EntityFrameworkCore.PostgreSQL`/`Microsoft.EntityFrameworkCore.Design`** — the latest published versions target net10.0; pin explicit 9.x versions (see step 14).

Run everything:
```bash
dotnet build
dotnet test
```

## 19. Docker / k8s / pipeline updates for the new feature

`k8s/deployment.yaml` — add `envFrom` for the secret the pipeline will create:
```yaml
        - name: helloazureapi
          image: __ACR_LOGIN_SERVER__/helloazureapi:__IMAGE_TAG__
          envFrom:
            - secretRef:
                name: helloazureapi-secrets
          ports: [...]
```

`azure-pipelines.yml` — add a test step to the Build stage, and Key-Vault-read + secret-apply steps before the existing `kubectl apply` in the Deploy stage:
```yaml
variables:
  # ...existing variables...
  keyVaultName: 'helloazure-kv26957'

stages:
  - stage: Build
    jobs:
      - job: BuildAndPush
        steps:
          - task: UseDotNet@2
            inputs: { packageType: sdk, version: '9.0.x' }
          - script: dotnet test
            displayName: dotnet test
          - task: AzureCLI@2
            displayName: az acr build
            # ...unchanged...

  - stage: Deploy
    dependsOn: Build
    jobs:
      - job: DeployToAks
        steps:
          - task: AzureCLI@2
            displayName: Read secrets from Key Vault
            inputs:
              azureSubscription: $(azureServiceConnection)
              scriptType: bash
              scriptLocation: inlineScript
              inlineScript: |
                PG_CONN=$(az keyvault secret show --vault-name $(keyVaultName) --name PostgresConnectionString --query value -o tsv)
                BLOB_CONN=$(az keyvault secret show --vault-name $(keyVaultName) --name BlobStorageConnectionString --query value -o tsv)
                echo "##vso[task.setvariable variable=pgConnectionString;issecret=true]$PG_CONN"
                echo "##vso[task.setvariable variable=blobConnectionString;issecret=true]$BLOB_CONN"

          - task: AzureCLI@2
            displayName: kubectl apply
            inputs:
              azureSubscription: $(azureServiceConnection)
              scriptType: bash
              scriptLocation: inlineScript
              inlineScript: |
                az aks get-credentials --resource-group $(resourceGroup) --name $(aksName) --overwrite-existing

                kubectl create secret generic helloazureapi-secrets \
                  --from-literal=ConnectionStrings__Postgres="$(pgConnectionString)" \
                  --from-literal=ConnectionStrings__BlobStorage="$(blobConnectionString)" \
                  --dry-run=client -o yaml | kubectl apply -f -

                sed -i -e "s#__ACR_LOGIN_SERVER__#$(acrName).azurecr.io#g" -e "s#__IMAGE_TAG__#$(imageTag)#g" k8s/deployment.yaml
                kubectl apply -f k8s/deployment.yaml -f k8s/service.yaml
                kubectl rollout status deployment/helloazureapi --timeout=120s
```
Marking the Key Vault values `issecret=true` makes Azure DevOps redact them from logs — don't skip this, or the Postgres password and storage key end up in plaintext build logs.

Also update `Dockerfile`'s `dotnet publish` to name the project explicitly (`dotnet publish HelloAzureApi.csproj ...`, see Part 1 step 2) and add `tests/` to `.dockerignore` — both needed now that a second `.csproj` exists.

## 20. Commit, push, verify

```bash
git add -A
git commit -m "Add Postgres-backed file upload/download with Blob Storage and Key Vault-managed secrets"
git push origin master
```

Pushing to `master` triggers the pipeline automatically. After it succeeds:
```bash
kubectl logs -l app=helloazureapi --tail=50   # confirm the EF migration applied cleanly against real Postgres

curl -F "file=@sample.txt;type=text/plain" http://<EXTERNAL-IP>/files          # upload, note the returned id
curl -sD - -o downloaded.txt "http://<EXTERNAL-IP>/files/<id>"                  # download
diff sample.txt downloaded.txt && echo "byte-for-byte match"

az storage blob list --account-name helloazureblob26957 --container-name uploads --auth-mode key -o table
```
To check the Postgres row directly, temporarily add your own IP to the server's firewall (`az postgres flexible-server firewall-rule create ...`), query, then delete the rule again — don't leave ad-hoc IPs in the firewall.

---

## Cleanup (avoid ongoing charges)

```bash
az group delete --name helloazure-rg --yes --no-wait
```
This removes the AKS cluster, ACR, Postgres server, storage account, and Key Vault together (all in `helloazure-rg`).