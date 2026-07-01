using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Dtos.Responses
{
    public class MercadoPagoPaymentResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }  // long, no string

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("date_created")]
        public DateTimeOffset DateCreated { get; set; }
    }
}
