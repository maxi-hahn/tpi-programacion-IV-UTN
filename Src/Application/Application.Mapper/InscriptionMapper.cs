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
                ScheduleId = inscription.ScheduleId,
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
                ScheduleId = inscription.ScheduleId,
                Schedule = inscription.Schedule?.ToScheduleResponse (inscription.Class?.Max_Users ?? 0),
                InscriptionDate = inscription.InscriptionDate
            };
        }

        public static Inscription ToInscription(this InscriptionRequest request)
        {
            return new Inscription
            {
                Id = Guid.NewGuid(),
                ScheduleId = request.ScheduleId,
                InscriptionDate = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}