using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL connection
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "appdb";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "appuser";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

var app = builder.Build();

// Health check endpoint (for K8s liveness/readiness probes)
app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
});

// Root endpoint (required by sample, also good for health checks)
app.MapGet("/", () =>
{
    return Results.Ok(new { message = "API is running", version = "2.0" });
});

// Endpoint to check database connectivity
app.MapGet("/db-check", async () =>
{
    try
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT version()";
                var result = await cmd.ExecuteScalarAsync();
                return Results.Ok(new
                {
                    status = "connected",
                    database = dbName,
                    host = dbHost,
                    postgresVersion = result?.ToString() ?? "unknown"
                });
            }
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            title: "Database Connection Failed",
            statusCode: 503
        );
    }
});

// Sample data endpoint (just echoes config for demo)
app.MapGet("/config", () =>
{
    return Results.Ok(new
    {
        db_host = dbHost,
        db_port = dbPort,
        db_name = dbName,
        db_user = dbUser,
        api_port = Environment.GetEnvironmentVariable("API_PORT") ?? "8080"
    });
});

app.Run();