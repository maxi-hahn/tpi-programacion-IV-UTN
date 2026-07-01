using Application.Dtos.Request;
using Application.Dtos.Request.Admin;
using Application.Dtos.Responses;
using Domain.Entity;

namespace Application.Interfaces
{
    public interface IAdminService : IUserService
    {

        Task<Plan?> DeletePlan(Guid id);
        Task<IEnumerable<Plan>> GetPlan();

        Task<Schedule?> DeleteSchedule(Guid id);

        Task<Plan?> UpdatePlan(Guid id, CreatePlanAdminRequest request);

        Task<Plan?> CreatePlan(CreatePlanAdminRequest request);

        Task<Class?> CreteClass(CreateClassRequest request, List<CreteScheduleAdminRequest> scheduleRequests);

        Task<Schedule?> CreteSchedule(CreteScheduleAdminRequest request);

        Task<IEnumerable<Class?>> DeleteClass(Guid id);

        Task<IEnumerable<Class>> GetClass();

        Task<ClassDetailResponse?> GetClassDetail(Guid id);

        Task<ClientInscriptionsResponse?> GetClientInscriptions(Guid clientId);

        Task<IEnumerable<Class?>> UpdateClass(Guid id, CreateClassRequest request, List<CreteScheduleAdminRequest> scheduleRequests);


        Task<IEnumerable<Schedule?>> UpdateSchedule(Guid id, List<CreteScheduleAdminRequest> scheduleRequests);



    }
}
