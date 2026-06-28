using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Infrastructure.Service
{
    public class SubscriptionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

                await subscriptionService.CheckExpiredSubscriptions();

                await Task.Delay(TimeSpan.FromHours(24),stoppingToken);
            }
        }
    }
}