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
                // Run immediately at startup, then again at every midnight
                using (var scope = _serviceProvider.CreateScope())
                {
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    await subscriptionService.AutoUnsubscribePastClasses();
                    await subscriptionService.CheckExpiredSubscriptions();
                }

                // TESTING: run every 1 minute so expiration can be verified without waiting.
                // PRODUCTION: revert to the midnight calculation below:
                //   var now = DateTime.Now;
                //   var nextMidnight = now.Date.AddDays(1);
                //   await Task.Delay(nextMidnight - now, stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}