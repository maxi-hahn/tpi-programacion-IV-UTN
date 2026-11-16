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
                ClassDate = inscription.ClassDate,
                IsActive = inscription.IsActive,
                IsConsumed = inscription.IsConsumed  // NUEVO
            };
        }

        public static MyInscriptionResponse ToMyInscriptionResponse(this Inscription inscription)
        {
            var scheduleResponse = inscription.Schedule?.ToScheduleResponse(inscription.Class?.Max_Users ?? 0);

            var classDateOffset = new DateTimeOffset(
                DateTime.SpecifyKind(inscription.ClassDate, DateTimeKind.Unspecified),
                TimeZoneInfo.Local.GetUtcOffset(inscription.ClassDate));

            var inscriptionDateOffset = new DateTimeOffset(
                DateTime.SpecifyKind(inscription.InscriptionDate, DateTimeKind.Unspecified),
                TimeZoneInfo.Local.GetUtcOffset(inscription.InscriptionDate));

            return new MyInscriptionResponse
            {
                InscriptionId = inscription.Id,
                ClassId = inscription.ClassId,
                ClassName = inscription.Class?.Name ?? string.Empty,
                ScheduleId = inscription.ScheduleId,
                Schedule = scheduleResponse,
                InscriptionDate = inscriptionDateOffset,
                ClassDate = classDateOffset,
                IsActive = inscription.IsActive,
                IsConsumed = inscription.IsConsumed
            };
        }
    }
}
