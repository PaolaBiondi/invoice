var builder = WebApplication.CreateBuilder(args);
builder.AddLogger();

try
{
    Log.Information("Starting up the service at {timestamp}", DateTimeOffset.Now);

    var env = builder.Environment;
    builder.Configuration.Sources.Clear();
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

    if (env.IsDevelopment())
        builder.Configuration.AddUserSecrets<Program>();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddInfraServices(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (env.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapHealthChecks("/health");

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.Information("Service stopped at {timestamp}", DateTimeOffset.Now);
    Log.CloseAndFlush();
}