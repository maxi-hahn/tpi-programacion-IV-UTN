using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Exceptions;

namespace Infrastructure.Service
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IClientService _clientService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepo;

        public MercadoPagoService(
            IClientService clientService,
            HttpClient httpClient,
            IConfiguration configuration,
            IUserRepository userRepo)
        {
            _clientService = clientService;
            _httpClient = httpClient;
            _configuration = configuration;
            _userRepo = userRepo;
        }

        public async Task ProcessPayment(string paymentId)
        {
            Console.WriteLine("=== PROCESS PAYMENT DEBUG ===");
            Console.WriteLine($"Payment ID: {paymentId}");

            var accessToken = _configuration["MercadoPago:AccessToken"];

            var request = new HttpRequestMessage(HttpMethod.Get, $"v1/payments/{paymentId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var payment = JsonSerializer.Deserialize<MercadoPagoPaymentResponse>(result)
                ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de MercadoPago.");

            Console.WriteLine($"Status: {payment.Status}");
            Console.WriteLine($"ExternalReference: {payment.ExternalReference}");

            if (payment.Status == "approved")
            {
                var parts = payment.ExternalReference.Split('|');
                Console.WriteLine($"Parts count: {parts.Length}");

                // Fix Bug 2: do NOT silently return on format errors — throw so the webhook can retry.
                if (parts.Length != 2)
                {
                    Console.WriteLine($"ERROR: Invalid ExternalReference format: {payment.ExternalReference}");
                    throw new InvalidOperationException($"Invalid ExternalReference format: {payment.ExternalReference}");
                }

                if (!Guid.TryParse(parts[0], out var planId) || !Guid.TryParse(parts[1], out var userId))
                {
                    Console.WriteLine($"ERROR: Cannot parse GUIDs from ExternalReference: {payment.ExternalReference}");
                    throw new InvalidOperationException($"Cannot parse GUIDs from ExternalReference: {payment.ExternalReference}");
                }

                Console.WriteLine($"Parsed PlanId: {planId}, UserId: {userId}");

                try
                {
                    await _clientService.UpdatePlan(planId, userId);
                    Console.WriteLine("UpdatePlan SUCCESS");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UpdatePlan FAILED: {ex.Message}");
                    throw; // Re-lanzar para que el webhook maneje el error
                }
            }
            else
            {
                Console.WriteLine($"Payment not approved — status: {payment.Status}. No plan update performed.");
            }

            Console.WriteLine("=============================");
        }
        public async Task<string> CreatePreference(Plan plan, Guid userId)
        {
            var user = await _userRepo.GetById(userId);
            if (user == null)
                throw new NotFoundException("Client not found");

            if (!user.EmailVerified)
                throw new ForbiddenException("Debes verificar tu email antes de adquirir un plan.");

            var accessToken = _configuration["MercadoPago:AccessToken"];
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";

            float priceToCharge = plan.Value;

            var currentPlanStatus = await _clientService.GetUserPlan(userId);

            // Fix Bug 1: hasActivePlan must NOT depend on currentPlanStatus.IsActive (account status).
            // GetUserPlan now returns IsActive = plan-is-active, but we also guard with PlanId
            // to be explicit and resilient to future changes in GetPlanStatusForClient.
            var hasActivePlan = currentPlanStatus.PlanId.HasValue &&
                                currentPlanStatus.PlanValue.HasValue &&
                                currentPlanStatus.SubscriptionEndDate.HasValue &&
                                currentPlanStatus.SubscriptionEndDate.Value > DateTime.Now;

            Console.WriteLine("=== CREATE PREFERENCE DEBUG ===");
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"New Plan: {plan.Name} - ${plan.Value}");
            Console.WriteLine($"Current Plan Status:");
            Console.WriteLine($"  - IsActive (plan): {currentPlanStatus.IsActive}");
            Console.WriteLine($"  - PlanId: {currentPlanStatus.PlanId}");
            Console.WriteLine($"  - PlanName: {currentPlanStatus.PlanName}");
            Console.WriteLine($"  - PlanValue: {currentPlanStatus.PlanValue}");
            Console.WriteLine($"  - SubscriptionEndDate: {currentPlanStatus.SubscriptionEndDate}");
            Console.WriteLine($"  - HasActivePlan: {hasActivePlan}");

            if (hasActivePlan)
            {
                // Eliminar downgrade: si el plan nuevo es más barato, rechazar
                if (plan.Value <= currentPlanStatus.PlanValue!.Value)
                {
                    throw new InvalidOperationException("No se puede cambiar a un plan más barato o igual.");
                }

                // Cobrar diferencia EXACTA, sin prorrateo
                priceToCharge = plan.Value - currentPlanStatus.PlanValue!.Value;
            }

            Console.WriteLine($"  - PriceToCharge: {priceToCharge}");
            Console.WriteLine("===============================");

            var requestBody = new
            {
                items = new[]
                {
            new
            {
                title = plan.Name,
                quantity = 1,
                currency_id = "ARS",
                unit_price = priceToCharge,
            }
        },
                notification_url = _configuration["MercadoPago:NotificationUrl"],
                external_reference = $"{plan.Id}|{userId}",
                back_urls = new
                {
                    success = $"{frontendBaseUrl}/payment/success",
                    failure = $"{frontendBaseUrl}/payment/failure",
                    pending = $"{frontendBaseUrl}/payment/pending"
                }
            };


            var json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "checkout/preferences");
            request.Content = content;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
          
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error {response.StatusCode}: {responseContent}");
            }

            var preference = JsonSerializer.Deserialize<MercadoPagoPreferenceResponse>(responseContent)
                ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de MercadoPago.");

            return preference.InitPoint
                ?? throw new InvalidOperationException("No se pudo obtener el init_point de MercadoPago.");
        }
    }
}