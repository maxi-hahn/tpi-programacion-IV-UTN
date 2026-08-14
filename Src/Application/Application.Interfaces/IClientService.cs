using Application.Dtos.Responses;
using Application.Interfaces;
using Domain.Entity;

namespace Application.Interfaces
{
    public interface IClientService : IUserService
    {
        Task UpdatePlan(Guid planId, Guid userId);
        Task<Client?> SubscribeToPlan(SubscribePlanRequest request);
        Task<ClientPlanResponse> GetMyPlanStatus();
        Task<ClientPlanResponse> GetUserPlan(Guid userId);
        Task RemoveUserPlan(Guid userId);
    }
}