# Playbook: Enterprise Event Grid demo — API + Blob + Event Grid + Functions + Cosmos DB + Key Vault

Reproduces this project end-to-end: an order-intake pipeline where an API writes to Blob Storage, Blob Storage's native Event Grid integration fans out to a Function that validates/upserts into Cosmos DB (flagging duplicates on the same document rather than creating new ones), and a second Function demonstrates fan-out to downstream consumers. No CI/CD — deployment is manual CLI/`func` commands, with Key Vault consumed via Key Vault references + managed identity instead of a pipeline injecting secrets.

Resources created (region shown is where quota was actually available on this subscription — see the regional-quota gotcha below before assuming your subscription behaves the same way): resource group `evtgriddemo-rg`, Storage account `evtgriddemosa26957` (eastus) + container `orders`, Event Grid System Topic `evtgriddemo-blob-systopic` + Custom Topic `evtgriddemo-orderprocessed-topic` (eastus), Cosmos DB `evtgriddemo-cosmos27156` serverless (eastus), Key Vault `evtgriddemokv27156` (eastus), App Service Plan `evtgriddemo-asp` + Web App `evtgriddemo-api27156` (**centralus**), Function App `evtgriddemo-func27156` + its storage account `evtgriddemofuncsa27156` (**westus2**), Application Insights `evtgriddemo-appinsights` (eastus).

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli), logged in via `az login`
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) (`func --version` should show 4.x)
- PowerShell available for `Compress-Archive` (or any zip tool that produces forward-slash paths — see gotcha below)
- An Azure subscription with Contributor access

## 1. Decide the scenario and the Key-Vault access pattern up front

This demo is an order-intake pipeline: `POST /orders` writes `{orderId}.json` to Blob Storage → Blob's native `Microsoft.Storage.BlobCreated` Event Grid System Topic fires → an Event Grid subscription delivers it to a Function → the Function looks up Cosmos DB by `orderId`: not found → insert; found → **patch the same document** (`IsDuplicate: true`, increment `DuplicateAttemptCount`) rather than creating a duplicate. The Function then publishes a custom `Enterprise.OrderProcessed` event to a second Event Grid Custom Topic, which a second Function subscribes to purely to demonstrate fan-out (logs only).

Because there's no CI/CD pipeline, secrets are consumed via **Key Vault references in App Settings** (`@Microsoft.KeyVault(SecretUri=...)`), resolved automatically by each app's **system-assigned managed identity** granted `Key Vault Secrets User` RBAC on the vault. This is the PaaS-native equivalent of a pipeline injecting a Kubernetes Secret.

## 2. Provision Azure resources

```bash
RG=evtgriddemo-rg
LOCATION=eastus
SUFFIX=$RANDOM   # reused across resource names for global uniqueness

az group create -n $RG -l $LOCATION
```

### Storage account + container + Event Grid System Topic

```bash
az storage account create -n evtgriddemosa$SUFFIX -g $RG -l $LOCATION --sku Standard_LRS --kind StorageV2
CONN=$(az storage account show-connection-string -n evtgriddemosa$SUFFIX -g $RG -o tsv --query connectionString)
az storage container create --name orders --connection-string "$CONN" --public-access off

# Event Grid System Topic — created explicitly (not via implicit portal auto-create) for a predictable lifecycle
STORAGE_ID=$(az storage account show -n evtgriddemosa$SUFFIX -g $RG --query id -o tsv)
az eventgrid system-topic create -g $RG -n evtgriddemo-blob-systopic \
  --source "$STORAGE_ID" --topic-type Microsoft.Storage.StorageAccounts -l $LOCATION
```

### Custom Event Grid topic (fan-out)

```bash
az eventgrid topic create -g $RG -n evtgriddemo-orderprocessed-topic -l $LOCATION
```

### Cosmos DB (serverless)

```bash
az cosmosdb create -n evtgriddemo-cosmos$SUFFIX -g $RG --locations regionName=$LOCATION --capabilities EnableServerless --default-consistency-level Session
az cosmosdb sql database create -a evtgriddemo-cosmos$SUFFIX -g $RG -n OrdersDb
az cosmosdb sql container create -a evtgriddemo-cosmos$SUFFIX -g $RG -d OrdersDb -n OrderStatus --partition-key-path "/orderId"
```
Cosmos DB serverless accounts are single-region by design — don't try to add a failover region later without migrating capacity mode.

### Key Vault (RBAC mode) + Application Insights

```bash
az keyvault create -n evtgriddemokv$SUFFIX -g $RG -l $LOCATION --enable-rbac-authorization true

az extension add -n application-insights -y   # avoids an interactive install prompt that breaks non-interactive shells
az monitor app-insights component create -g $RG -a evtgriddemo-appinsights -l $LOCATION --application-type web
```

### App Service Plan + Web App, Function App + its own storage account

```bash
az appservice plan create -g $RG -n evtgriddemo-asp --is-linux --sku B1
az webapp create -g $RG -p evtgriddemo-asp -n evtgriddemo-api$SUFFIX --runtime "DOTNETCORE:9.0"

az storage account create -n evtgriddemofuncsa$SUFFIX -g $RG -l $LOCATION --sku Standard_LRS --kind StorageV2
az functionapp create -g $RG -n evtgriddemo-func$SUFFIX --storage-account evtgriddemofuncsa$SUFFIX \
  --consumption-plan-location $LOCATION --runtime dotnet-isolated --runtime-version 9 --functions-version 4 --os-type Linux
```

**Gotcha — regional VM/Consumption quota can be zero on dev/test subscriptions.** Both the App Service Plan (even F1 free tier) and the Function App Consumption plan failed in `eastus` with `Operation cannot be completed without additional quota... Current Limit (Total VMs): 0`. This isn't a Postgres-style regional *feature* restriction — it's an actual subscription-level VM quota of zero in that region for that plan type. Fix: try other regions until one has quota (`centralus` worked for the App Service Plan; `westus2` worked for the Function Consumption plan — a real subscription may have quota available in different regions than this one did, so don't assume these specific regions). Deploying the API and Functions to different regions than your storage/Cosmos/Key Vault is functionally fine, just adds a little cross-region latency.

If `az functionapp create` also fails with `Linux dynamic workers are not available in resource group .`, that's the same zero-quota issue for Consumption/Dynamic plans specifically — try more regions.

### Managed identities + Key Vault RBAC

```bash
az webapp identity assign -g $RG -n evtgriddemo-api$SUFFIX
az functionapp identity assign -g $RG -n evtgriddemo-func$SUFFIX

WEBAPP_PID=$(az webapp identity show -g $RG -n evtgriddemo-api$SUFFIX --query principalId -o tsv)
FUNC_PID=$(az functionapp identity show -g $RG -n evtgriddemo-func$SUFFIX --query principalId -o tsv)
KV_ID=$(az keyvault show -n evtgriddemokv$SUFFIX -g $RG --query id -o tsv)
MY_OBJECT_ID=$(az ad signed-in-user show --query id -o tsv)

az role assignment create --assignee-object-id "$MY_OBJECT_ID" --assignee-principal-type User --role "Key Vault Secrets Officer" --scope "$KV_ID"
az role assignment create --assignee-object-id "$WEBAPP_PID" --assignee-principal-type ServicePrincipal --role "Key Vault Secrets User" --scope "$KV_ID"
az role assignment create --assignee-object-id "$FUNC_PID" --assignee-principal-type ServicePrincipal --role "Key Vault Secrets User" --scope "$KV_ID"
```
**Gotcha — Git Bash / MSYS mangles resource IDs.** Any argument starting with `/subscriptions/...` (or `/orderId` as a partition-key-path value) gets silently rewritten into a Windows path by MSYS's automatic path conversion, producing baffling errors like `MissingSubscription` or a Cosmos error about a path containing `C:/Program Files/Git/...`. Fix: `export MSYS_NO_PATHCONV=1` before any `az` command with a leading-slash argument.

**Gotcha — RBAC propagation delay.** Role assignments can take a couple of minutes to propagate. If Key Vault references show as unresolved right after granting the role, wait and retry rather than assuming misconfiguration.

### Write secrets, wire Key Vault references

```bash
STORAGE_CONN=$(az storage account show-connection-string -n evtgriddemosa$SUFFIX -g $RG -o tsv --query connectionString)
COSMOS_CONN=$(az cosmosdb keys list -n evtgriddemo-cosmos$SUFFIX -g $RG --type connection-strings --query "connectionStrings[0].connectionString" -o tsv)
TOPIC_ENDPOINT=$(az eventgrid topic show -g $RG -n evtgriddemo-orderprocessed-topic --query endpoint -o tsv)
TOPIC_KEY=$(az eventgrid topic key list -g $RG -n evtgriddemo-orderprocessed-topic --query key1 -o tsv)

az keyvault secret set --vault-name evtgriddemokv$SUFFIX --name StorageConnectionString --value "$STORAGE_CONN"
az keyvault secret set --vault-name evtgriddemokv$SUFFIX --name CosmosConnectionString --value "$COSMOS_CONN"
az keyvault secret set --vault-name evtgriddemokv$SUFFIX --name EventGridTopicEndpoint --value "$TOPIC_ENDPOINT"
az keyvault secret set --vault-name evtgriddemokv$SUFFIX --name EventGridTopicKey --value "$TOPIC_KEY"

az webapp config appsettings set -g $RG -n evtgriddemo-api$SUFFIX --settings \
  "ConnectionStrings__BlobStorage=@Microsoft.KeyVault(SecretUri=https://evtgriddemokv$SUFFIX.vault.azure.net/secrets/StorageConnectionString/)" \
  "ConnectionStrings__Cosmos=@Microsoft.KeyVault(SecretUri=https://evtgriddemokv$SUFFIX.vault.azure.net/secrets/CosmosConnectionString/)"

az functionapp config appsettings set -g $RG -n evtgriddemo-func$SUFFIX --settings \
  "ConnectionStrings__BlobStorage=@Microsoft.KeyVault(SecretUri=https://evtgriddemokv$SUFFIX.vault.azure.net/secrets/StorageConnectionString/)" \
  "ConnectionStrings__Cosmos=@Microsoft.KeyVault(SecretUri=https://evtgriddemokv$SUFFIX.vault.azure.net/secrets/CosmosConnectionString/)" \
  "EventGrid__TopicEndpoint=@Microsoft.KeyVault(SecretUri=https://evtgriddemokv$SUFFIX.vault.azure.net/secrets/EventGridTopicEndpoint/)" \
  "EventGrid__TopicKey=@Microsoft.KeyVault(SecretUri=https://evtgriddemokv$SUFFIX.vault.azure.net/secrets/EventGridTopicKey/)"
```

Link Application Insights manually if `az functionapp create` couldn't auto-provision it (common when the region differs from where App Insights was created):
```bash
CONN_STRING=$(az monitor app-insights component show -g $RG -a evtgriddemo-appinsights --query connectionString -o tsv)
az functionapp config appsettings set -g $RG -n evtgriddemo-func$SUFFIX --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$CONN_STRING"
az webapp config appsettings set -g $RG -n evtgriddemo-api$SUFFIX --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$CONN_STRING"
```

## 3. Scaffold the two .NET projects

```bash
mkdir EventGridDemo && cd EventGridDemo
dotnet new webapi -n OrdersApi -o OrdersApi --use-controllers false
func init OrderProcessingFunctions --worker-runtime dotnet-isolated --target-framework net9.0
dotnet new sln -n EventGridDemo
dotnet sln add OrdersApi/OrdersApi.csproj
dotnet sln add OrderProcessingFunctions/OrderProcessingFunctions.csproj
```

### NuGet packages — add one at a time, `dotnet build` after each

Both projects need `Microsoft.Azure.Cosmos` (SDK v3 — not the EF Core Cosmos provider, which risks the same "latest version targets a newer TFM than your project" trap that bit an earlier project). **Gotcha:** `Microsoft.Azure.Cosmos` requires `Newtonsoft.Json >= 10.0.2` to be **explicitly** referenced in the consuming project, or the build fails with `The Newtonsoft.Json package must be explicitly referenced`. Add it right after Cosmos:
```bash
dotnet add package Microsoft.Azure.Cosmos
dotnet add package Newtonsoft.Json --version 13.0.3
```

- **OrdersApi**: `Azure.Storage.Blobs`, `Microsoft.Azure.Cosmos`, `Newtonsoft.Json`, `Microsoft.AspNetCore.OpenApi`.
- **OrderProcessingFunctions**: `Microsoft.Azure.Functions.Worker.Extensions.EventGrid` (pulls in `Azure.Messaging.EventGrid` transitively, used both for the trigger and for publishing the fan-out event), `Microsoft.Azure.Cosmos`, `Newtonsoft.Json`, `Azure.Storage.Blobs` (read blob content directly from the event's URL rather than via a Functions input binding — simpler to reason about).

### Project structure

```
OrdersApi/
  Program.cs                          - DI: BlobServiceClient, CosmosClient from Key-Vault-reference config
  Endpoints/OrderEndpoints.cs          - POST /orders, GET /orders/{orderId}/status
  Storage/IOrderBlobStorageService.cs / BlobOrderStorageService.cs
  Cosmos/IOrderStatusRepository.cs / CosmosOrderStatusRepository.cs   (read-only: point-read by orderId)
  Models/OrderSubmission.cs, OrderStatusResponse.cs

OrderProcessingFunctions/
  Program.cs                          - FunctionsApplication.CreateBuilder + DI
  Functions/BlobOrderProcessor.cs      - EventGridTrigger: read blob -> Cosmos create-or-flag -> publish fan-out event
  Functions/OrderProcessedFanoutLogger.cs  - EventGridTrigger (custom topic): logs only
  Cosmos/IOrderStatusRepository.cs / CosmosOrderStatusRepository.cs  (write side: create-or-flag-duplicate)
  Models/OrderJson.cs, OrderProcessedEvent.cs
```

No shared class library between the two projects — deliberate, so each is demoable standalone (small DTO duplication is an acceptable trade for a training example).

### The Cosmos duplicate-flag logic (the correctness-critical piece)

Point-read by `id = orderId`, partition key `orderId`:
```csharp
try
{
    existing = await container.ReadItemAsync<OrderStatusDocument>(orderId, partitionKey);
}
catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    existing = null;
}

if (existing is null)
{
    try
    {
        await container.CreateItemAsync(newDocument, partitionKey);
        return (false, 0);
    }
    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
    {
        // Event Grid is at-least-once delivery: a concurrent invocation for the
        // same orderId won the race. Fall through to the patch branch instead
        // of failing.
        existing = await container.ReadItemAsync<OrderStatusDocument>(orderId, partitionKey);
    }
}

document.IsDuplicate = true;
document.DuplicateAttemptCount += 1;
document.LastDuplicateAttemptAtUtc = DateTime.UtcNow;
await container.ReplaceItemAsync(document, orderId, partitionKey, new ItemRequestOptions { IfMatchEtag = existing.ETag });
```
Don't use `UpsertItemAsync` for the main path — it always overwrites, erasing the new-vs-duplicate distinction that's the whole point of the demo.

**Gotcha — Cosmos partition-key path is case-sensitive against the serialized JSON property name.** The container was created with `--partition-key-path "/orderId"` (lowercase). The C# model's `OrderId` property, with no explicit JSON attribute, serializes as `"OrderId"` (capital O) — Cosmos then can't find a value at path `/orderId` and throws `PartitionKey extracted from document doesn't match the one specified in the header` (400, substatus 1001) on every write. Fix: annotate the property explicitly:
```csharp
[JsonProperty("orderId")]
public required string OrderId { get; set; }
```
Do this in **both** copies of the document model (API's read-side and Function's write-side) so they agree on the wire format.

**Gotcha — `System.Text.Json` default deserialization is case-sensitive, but Azure's own event schemas use lowerCamelCase.** Reading the `BlobCreated` event's `data.url` field via `eventGridEvent.Data.ToObjectFromJson<T>()` against a plain `public string Url { get; set; }` property silently leaves it empty (`System.UriFormatException: Invalid URI: The URI is empty` when passed to `new Uri(...)`), because the JSON key is `"url"` and the property is `"Url"` under case-sensitive default matching. Fix:
```csharp
[System.Text.Json.Serialization.JsonPropertyName("url")]
public string Url { get; set; } = "";
```
This only bites data coming from *external* schemas (Azure Storage's event payload) — the custom `Enterprise.OrderProcessed` event round-trips fine using default System.Text.Json options on both the publish and consume sides, since both are our own code using the same (PascalCase, case-sensitive) convention symmetrically.

**Gotcha — `IFormFile`-style automatic antiforgery doesn't apply here, but a different .NET 9 default did bite the *previous* project doing file uploads; this one binds a plain JSON body via minimal APIs, so no `.DisableAntiforgery()` call is needed** — noted only because it's easy to reflexively add it out of habit from a prior project; don't.

## 4. Deploy (manual — no pipeline)

**Function App first** — Event Grid subscriptions using `--endpoint-type azurefunction` resolve an ARM child resource (`.../sites/{app}/functions/{functionName}`) that only exists once the function is deployed and indexed:
```bash
cd OrderProcessingFunctions
dotnet build -c Release
func azure functionapp publish evtgriddemo-func$SUFFIX --dotnet-isolated
az functionapp function list -g $RG -n evtgriddemo-func$SUFFIX -o table   # confirm both functions indexed
```

**Then** create the two Event Grid subscriptions:
```bash
export MSYS_NO_PATHCONV=1
SUB=$(az account show --query id -o tsv)

az eventgrid system-topic event-subscription create \
  -g $RG --system-topic-name evtgriddemo-blob-systopic -n orders-blobcreated-sub \
  --endpoint-type azurefunction \
  --endpoint "/subscriptions/$SUB/resourceGroups/$RG/providers/Microsoft.Web/sites/evtgriddemo-func$SUFFIX/functions/BlobOrderProcessor" \
  --included-event-types Microsoft.Storage.BlobCreated \
  --subject-begins-with "/blobServices/default/containers/orders/"

TOPIC_ID=$(az eventgrid topic show -g $RG -n evtgriddemo-orderprocessed-topic --query id -o tsv)
az eventgrid event-subscription create \
  -n orderprocessed-fanout-sub --source-resource-id "$TOPIC_ID" \
  --endpoint-type azurefunction \
  --endpoint "/subscriptions/$SUB/resourceGroups/$RG/providers/Microsoft.Web/sites/evtgriddemo-func$SUFFIX/functions/OrderProcessedFanoutLogger" \
  --included-event-types Enterprise.OrderProcessed
```

**Then the Web API** — publish targeting `linux-x64` explicitly:
```bash
cd ../OrdersApi
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish
```
**Gotcha — a RID-agnostic publish breaks Linux zip-deploy.** Publishing without `-r linux-x64` bundles a `runtimes/` folder with native assets for *every* platform (Windows, browser, unix), and Windows-zipping that (via PowerShell's `Compress-Archive`) produces backslash-containing entry names for the nested Windows-named folders. Kudu's `rsync`-based deployment on the Linux App Service then fails with `rsync: [generator] recv_generator: failed to stat "/home/site/wwwroot/runtimes\win\lib\...": Invalid argument (22)`. Publishing with an explicit `-r linux-x64 --self-contained false` trims the RID-specific native assets down to just what's needed, eliminating the problematic paths entirely (it also shrinks the zip drastically — 11MB down to 4.7MB in this run).

```powershell
Compress-Archive -Path ".\publish\*" -DestinationPath ".\publish.zip" -Force
```
```bash
az webapp deploy -g $RG -n evtgriddemo-api$SUFFIX --src-path ./publish.zip --type zip
```

## 5. Local development story

- **Web API**: fully runnable locally (`dotnet run`) against Azurite for blob and the real serverless Cosmos account directly (cheap enough at demo scale to skip standing up the Cosmos emulator).
- **Functions**: no native way to raise real Event Grid events locally against Azurite. Recommended: unit-test the extracted "parse event → read blob → Cosmos create-or-flag" logic directly with fakes, and separately smoke-test via `func start` plus manually POSTing a synthetic event to the local webhook route:
  ```
  http://localhost:7071/runtime/webhooks/EventGrid?functionName=BlobOrderProcessor
  ```
  This same technique — POSTing a synthetic `Microsoft.Storage.BlobCreated`-shaped JSON array to `https://<functionapp>.azurewebsites.net/runtime/webhooks/eventgrid?functionName=<name>&code=<eventgrid_extension system key>` — is also the fastest way to test the **deployed** function directly, bypassing Event Grid's own retry/backoff schedule, which is exactly how the case-sensitivity bugs above were diagnosed quickly instead of waiting on Event Grid's multi-minute retry intervals. Get the system key via:
  ```bash
  az functionapp keys list -g $RG -n evtgriddemo-func$SUFFIX --query "systemKeys.eventgrid_extension" -o tsv
  ```

## 6. Verification

1. `POST /orders` with `{"orderId":"ord-001","customerId":"cust-1","amount":42.50}` → `202 Accepted`.
2. `az storage blob list --account-name <storage> --container-name orders --auth-mode key -o table` → confirms `ord-001.json` landed.
3. Query Cosmos directly (Data Explorer, or the Cosmos SDK) for `id = "ord-001"` → `IsDuplicate: false`.
4. **Re-POST the same orderId** → confirm still exactly **one** document (same `_rid`/`_etag` lineage, not a second document), now `IsDuplicate: true`, `DuplicateAttemptCount: 1` — the core hard requirement.
5. Check Application Insights (`traces`/`requests` tables via `az monitor app-insights query`) for `OrderProcessedFanoutLogger` executions — confirm one per event (original *and* duplicate), proving the fan-out is decoupled from which branch the main function took.
6. `GET /orders/ord-001/status` → reflects the post-duplicate state.
7. A fresh `orderId` (`ord-002`) → confirms the "new" branch still works, not just the duplicate branch.

## Known limits (flagged, not fixed)

Function App Consumption-plan cold start can add several seconds to the very first invocation after idle — narrate it in a live demo rather than let it look broken, or send a warm-up POST a minute beforehand. Application Insights ingestion typically lags 30–90 seconds behind real events; don't read an empty query result as "nothing happened" without also checking directly (Cosmos document, blob listing) first.

## Cleanup (avoid ongoing charges)

```bash
az group delete --name evtgriddemo-rg --yes --no-wait
```