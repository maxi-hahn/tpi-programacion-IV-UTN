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
            Console.WriteLine($">>> ProcessPayment llamado con id: {paymentId}");

            var accessToken = _configuration["MercadoPago:AccessToken"];

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"v1/payments/{paymentId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var payment = JsonSerializer.Deserialize<MercadoPagoPaymentResponse>(result)
                ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de MercadoPago.");

            if (payment.Status == "approved")
            {
                var parts = payment.ExternalReference.Split('|');
                var planId = Guid.Parse(parts[0]);
                var userId = Guid.Parse(parts[1]);

                Console.WriteLine($">>> Pago aprobado! PlanId: {planId} - UserId: {userId}");

                await _clientService.UpdatePlan(planId, userId);

                Console.WriteLine($">>> Plan activado!");
            }
            else
            {
                Console.WriteLine($">>> Status del pago: {payment.Status}");
            }
        }
        public async Task<string> CreatePreference(Plan plan, Guid userId)
        {
            var accessToken = _configuration["MercadoPago:AccessToken"];

            var requestBody = new
            {
                items = new[]
                {
                    new
                    {
                        title = plan.Name,
                        quantity = 1,
                        currency_id = "ARS",
                        unit_price = plan.Value,
                    }
                },
                notification_url = _configuration["MercadoPago:NotificationUrl"],
                external_reference = $"{plan.Id}|{userId}"  // los dos juntos
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "checkout/preferences");
            request.Content = content;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var preference = JsonSerializer.Deserialize<MercadoPagoPreferenceResponse>(result)
                ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de MercadoPago.");

            return preference.InitPoint
                ?? throw new InvalidOperationException("No se pudo obtener el init_point de MercadoPago.");
        }
    }
}