using Application.Constants;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;
using Application.Templates;

namespace Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;

        public SubscriptionService(IUserRepository userRepo, IEmailService emailService)
        {
            _userRepo = userRepo;
            _emailService = emailService;
        }

        public async Task CheckExpiredSubscriptions()
        {
            var clients = await _userRepo.GetAll();

            var expiredClients = clients
                .OfType<Client>()
                .Where(c =>
                    c.IsActive &&
                    c.SubscriptionEndDate.HasValue &&
                    c.SubscriptionEndDate.Value < DateTime.UtcNow);


            foreach (var client in expiredClients)
            {
                client.IsActive = false;
                if (client.SubscriptionEndDate == null)
                    continue;

                var daysLeft = (client.SubscriptionEndDate.Value.Date - DateTime.UtcNow.Date).Days;

                // Aviso 3 dias antes

                if (daysLeft == 3)
                {
                    await _emailService.SendEmailAsync(client.Email, EmailSubjects.SubscriptionExpiring, EmailTemplates.SubscriptionExpiring(client.Name, client.SubscriptionEndDate.Value, daysLeft));
                }

                // Suscripcion vencida

                if (daysLeft < 0 && client.IsActive)
                {
                    client.IsActive = false;

                    await _emailService.SendEmailAsync(client.Email, EmailSubjects.SubscriptionExpired, EmailTemplates.SubscriptionExpired(client.Name));
                    await _userRepo.Update(client);
                }
            }
        }
    }
}