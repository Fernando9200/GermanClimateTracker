using Application.Common.Interfaces;
using Infrastructure.Services;
using Jobs.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Hangfire;
using Hangfire.SqlServer;
using WorkerService;
using DotNetEnv;

DotNetEnv.Env.Load("../.env");
var apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    hostContext.Configuration["OpenWeather:ApiKey"] = apiKey;

    hostContext.Configuration["OpenWeather:ApiKey"] = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            hostContext.Configuration.GetConnectionString("DefaultConnection")));

    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    services.AddHttpClient<IWeatherService, OpenWeatherService>();
    services.AddScoped<IWeatherService, OpenWeatherService>();

    services.AddScoped<WeatherDataUpsertJob>();

    services.AddHangfire((serviceProvider, config) => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(hostContext.Configuration.GetConnectionString("HangfireConnection")));

    services.AddHangfireServer(options =>
    {
        options.ServerName = "weatherdataserver";
    });

    services.AddSingleton<IRecurringJobManager>(provider => new RecurringJobManager());

    services.AddHostedService<Worker>();
});

var host = builder.Build();
await host.RunAsync();