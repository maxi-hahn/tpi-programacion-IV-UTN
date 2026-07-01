using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.Responses
{
    public class InscriptionResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public InscriptionResponse? Data { get; set; }
    }
}