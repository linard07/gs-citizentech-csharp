using Microsoft.EntityFrameworkCore;
using Oasis.Api.Data;
using Oasis.Api.Repositories;
using Oasis.Domain.Interfaces;
using Oasis.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OasisDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<
    IProtectionPointRepository,
    ProtectionPointRepository>();

builder.Services.AddScoped<HeatAlertService>();

var app = builder.Build();
// Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<OasisDbContext>();

    await DbSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Oasis API v1"
        );
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
