using Application.Dtos.Request;
using Application.Dtos.Responses;
using Domain.Entity;

namespace Application.Mapper
{
    public static class InscriptionMapper
    {
        public static InscriptionResponse ToInscriptionResponse(this Inscription inscription)
        {
            return new InscriptionResponse
            {
                Id = inscription.Id,
                UserId = inscription.UserId,
                ClassId = inscription.ClassId,
                InscriptionDate = inscription.InscriptionDate,
                IsActive = inscription.IsActive
            };
        }

        public static MyInscriptionResponse ToMyInscriptionResponse(this Inscription inscription)
        {
            return new MyInscriptionResponse
            {
                InscriptionId = inscription.Id,
                ClassId = inscription.ClassId,
                ClassName = inscription.Class?.Name ?? string.Empty,
                Schedules = inscription.Class?.Schedules?.Select(s => s.ToScheduleResponse()).ToList() ?? new(),
                InscriptionDate = inscription.InscriptionDate
            };
        }

        public static Inscription ToInscription(this InscriptionRequest request)
        {
            return new Inscription
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ClassId = request.ClassId,
                InscriptionDate = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}