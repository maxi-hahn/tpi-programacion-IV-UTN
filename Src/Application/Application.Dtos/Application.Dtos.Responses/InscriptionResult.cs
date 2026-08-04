namespace Application.Dtos.Responses
{
    public class InscriptionResult
    {
        public bool Success { get; set; }
        public string? code { get; set; }
        public string? ErrorMessage { get; set; }
        public InscriptionResponse? Data { get; set; }
    }
}