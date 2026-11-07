using System.Reflection;
using System.Text.Json.Serialization;
using CitizenTech.Api.Application.Abstractions;
using CitizenTech.Api.Application.Services;
using CitizenTech.Api.Domain;
using CitizenTech.Api.Infrastructure.Persistence;
using CitizenTech.Api.Infrastructure.Repositories;
using CitizenTech.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----- Web API + documentação (Swagger) -----
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CitizenTech Alerts API",
        Version = "v1",
        Description = "Backend da CitizenTech: alertas de eventos extremos (chuva forte, alagamento, calor e frio extremos) " +
                      "e direcionamento da população para pontos de segurança (fictícios no protótipo)."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ----- Persistência (EF Core + SQLite) -----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
                      ?? "Data Source=citizentech.db"));

// ----- Injeção de dependência -----
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IAlertRepository, EfAlertRepository>();
builder.Services.AddScoped<ISafeZoneRepository, EfSafeZoneRepository>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<ISafeZoneService, SafeZoneService>();

builder.Services.AddSingleton<IRiskEvaluator, RainRiskEvaluator>();
builder.Services.AddSingleton<IRiskEvaluator, FloodRiskEvaluator>();
builder.Services.AddSingleton<IRiskEvaluator, HeatRiskEvaluator>();
builder.Services.AddSingleton<IRiskEvaluator, ColdRiskEvaluator>();

// ----- Tratamento global de erros -----
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Cria o banco (aplica migrations, se existirem) e os pontos de segurança fictícios.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetMigrations().Any())
        await db.Database.MigrateAsync();
    else
        await db.Database.EnsureCreatedAsync();

    await DbSeeder.SeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "CitizenTech Alerts API v1"));

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
