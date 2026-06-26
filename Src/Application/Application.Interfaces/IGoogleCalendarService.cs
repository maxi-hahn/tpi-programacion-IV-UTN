using Domain.Entity;
using Application.Dtos.Request;

namespace Application.Application.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task SyncInscriptionAsync(
            string accessToken,
            Inscription inscription,
            string className,
            List<ClassScheduleRequest> schedules
        );
    }
}