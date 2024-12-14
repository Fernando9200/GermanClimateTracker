using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IServiceProvider _serviceProvider;

        public Worker(
            ILogger<Worker> logger,
            IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _recurringJobManager = recurringJobManager;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker Service starting at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                // Schedule the recurring job
                _recurringJobManager.AddOrUpdate(
                    "weather-data-upsert",
                    () => scope.ServiceProvider.GetRequiredService<Jobs.BackgroundJobs.WeatherDataUpsertJob>().Execute(),
                    "*/5 * * * *" // Every 5 minutes
                );

                // Execute the job immediately
                var weatherJob = scope.ServiceProvider.GetRequiredService<Jobs.BackgroundJobs.WeatherDataUpsertJob>();
                await weatherJob.Execute();
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker Service stopping at: {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
        }
    }
}