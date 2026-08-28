using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace Infrastructure.Service
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IClientService _clientService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MercadoPagoService(
            IClientService clientService,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _clientService = clientService;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task ProcessPayment(string paymentId)
        {
            var accessToken = _configuration["MercadoPago:AccessToken"];

            var request = new HttpRequestMessage(HttpMethod.Get, $"v1/payments/{paymentId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var payment = JsonSerializer.Deserialize<MercadoPagoPaymentResponse>(result)
                ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de MercadoPago.");

            if (payment.Status == "approved")
            {
                var parts = payment.ExternalReference.Split('|');

                if (parts.Length != 2)
                {
                  
                    return;
                }

                if (!Guid.TryParse(parts[0], out var planId) || !Guid.TryParse(parts[1], out var userId))
                {
         
                    return;
                }

              
                try
                {
                    await _clientService.UpdatePlan(planId, userId);
                    
                }
                catch (Exception ex)
                {
                    
                    throw; // Re-lanzar para que el webhook maneje el error
                }
            }
            else
            {
               
            }
        }
        public async Task<string> CreatePreference(Plan plan, Guid userId)
        {
            var accessToken = _configuration["MercadoPago:AccessToken"];
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";

            // --- Objective 3: Calculate prorated price if client has an active plan ---
            float priceToCharge = plan.Value;

            var currentPlanStatus = await _clientService.GetUserPlan(userId);
            var hasActivePlan = currentPlanStatus.IsActive &&
                                currentPlanStatus.SubscriptionEndDate.HasValue &&
                                currentPlanStatus.SubscriptionEndDate.Value > DateTime.Now &&
                                currentPlanStatus.PlanValue.HasValue;

            if (hasActivePlan)
            {
                var remainingDays = (currentPlanStatus.SubscriptionEndDate!.Value.Date - DateTime.Now.Date).Days;

                if (remainingDays > 0)
                {
                    var currentProrated = (currentPlanStatus.PlanValue!.Value / 30f) * remainingDays;
                    var newProrated = (plan.Value / 30f) * remainingDays;
                    var difference = newProrated - currentProrated;

                    if (difference <= 0)
                    {
                        // New plan is cheaper or equal: activate immediately, no payment needed
                        await _clientService.UpdatePlan(plan.Id, userId);
                        return $"{frontendBaseUrl}/payment/success";
                    }

                    priceToCharge = difference;
                }
            }
            // -------------------------------------------------------------------------

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
                // Sin auto_return
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