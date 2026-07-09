using Azure.Identity;
using Azure.Storage.Blobs;
 
var builder = WebApplication.CreateBuilder(args);
 
var keyVaultUri = new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(x =>
{
    var connStr = builder.Configuration["BlobStorageConnectionString"];
    return new BlobServiceClient(connStr);
});

 
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
