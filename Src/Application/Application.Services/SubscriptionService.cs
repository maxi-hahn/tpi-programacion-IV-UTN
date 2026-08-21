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

            var activeClients = clients
                .OfType<Client>()
                .Where(c =>
                    c.IsActive &&
                    c.SubscriptionEndDate.HasValue);

            foreach (var client in activeClients)
            {
                var daysLeft = (client.SubscriptionEndDate!.Value.Date - DateTime.UtcNow.Date).Days;

                // Caso 1: Aviso 3 días antes del vencimiento
                if (daysLeft == 3)
                {
                    await _emailService.SendEmailAsync(
                        client.Email,
                        EmailSubjects.SubscriptionExpiring,
                        EmailTemplates.SubscriptionExpiring(
                            client.Name,
                            client.SubscriptionEndDate.Value,
                            daysLeft));
                }
                // Caso 2: Suscripción vencida
                else if (daysLeft < 0)
                {
                    client.IsActive = false;

                    await _emailService.SendEmailAsync(
                        client.Email,
                        EmailSubjects.SubscriptionExpired,
                        EmailTemplates.SubscriptionExpired(client.Name));

                    await _userRepo.Update(client);
                }
            }

            // Guardar todos los cambios pendientes
            await _userRepo.Save();
        }
    }
}