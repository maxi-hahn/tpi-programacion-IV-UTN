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
        private readonly IInscriptionRepository _inscriptionRepo;
        private readonly INotificationService _notificationService;

        public SubscriptionService(
            IUserRepository userRepo,
            IEmailService emailService,
            IInscriptionRepository inscriptionRepo,
            INotificationService notificationService)
        {
            _userRepo = userRepo;
            _emailService = emailService;
            _inscriptionRepo = inscriptionRepo;
            _notificationService = notificationService;
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
                var daysLeft = (client.SubscriptionEndDate!.Value.Date - DateTime.Now.Date).Days;

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

                    await _notificationService.CreateNotification(
                        client.Id,
                        "Plan por vencer",
                        $"Tu plan vence en {daysLeft} días ({client.SubscriptionEndDate.Value:dd/MM/yyyy}). Renovalo para seguir disfrutando.",
                        "PlanExpiring");
                }
                // Caso 2: Suscripción vencida
                else if (daysLeft < 0)
                {
                    client.IsActive = false;

                    await _emailService.SendEmailAsync(
                        client.Email,
                        EmailSubjects.SubscriptionExpired,
                        EmailTemplates.SubscriptionExpired(client.Name));

                    await _notificationService.CreateNotification(
                        client.Id,
                        "Plan vencido",
                        "Tu plan ha vencido. Adquirí uno nuevo para poder reservar clases.",
                        "PlanExpired");

                    await _userRepo.Update(client);
                }
            }

            // Guardar todos los cambios pendientes
            await _userRepo.Save();
        }

        public async Task AutoUnsubscribePastClasses()
        {
            var pastInscriptions = await _inscriptionRepo.GetPastActiveInscriptions(DateTime.Now);

            if (!pastInscriptions.Any())
                return;

            foreach (var inscription in pastInscriptions)
            {
                inscription.IsActive = false;
                await _inscriptionRepo.Update(inscription);

                await _notificationService.CreateNotification(
                    inscription.UserId,
                    "Clase completada",
                    $"Tu inscripción a la clase del {inscription.ClassDate:dd/MM/yyyy HH:mm} fue marcada como completada.",
                    "ClassCompleted");
            }

            await _inscriptionRepo.Save();
        }
    }
}