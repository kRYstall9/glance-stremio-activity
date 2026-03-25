using GlanceStremioActivity.Models;
using GlanceStremioActivity.Services;
using GlanceStremioActivity.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IStremioAuthService, StremioAuthService>();
builder.Services.AddHttpClient<IStremioApiService, StremioApiService>();
builder.Services.AddSingleton<IUserConfigurationService, UserConfigurationService>();

var apiKey = Environment.GetEnvironmentVariable("API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
    throw new InvalidOperationException("API_KEY environment variable is required but was not set.");

Config config = new() { ApiKey = apiKey };
builder.Services.AddSingleton(config);

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
