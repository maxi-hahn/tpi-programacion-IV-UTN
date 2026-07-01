using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Dtos.Request
{
    
        public class MercadoPagoPreferenceResponse
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("init_point")]
            public string? InitPoint { get; set; }

            [JsonPropertyName("sandbox_init_point")]
            public string? SandboxInitPoint { get; set; }
        }
    

}
