using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class ClientService : UserService, IClientService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPlanRepository _planRepo;

        public ClientService(IUserRepository userRepo, IPasswordHasherService hasher, IUserContext userContext, IPlanRepository planRepo)
            : base(userRepo, hasher, userContext)
        {
            _userRepo = userRepo;
            _planRepo = planRepo;
        }

        public async Task UpdatePlan(Guid planId, Guid userId)
        {
            var client = (Client)await _userRepo.GetById(userId);
            if (client == null)
                throw new NotFoundException("Client not found");

            client.Id_Plan = planId;
            client.IsActive = true;
            await _userRepo.Update(client);
            await _userRepo.Save();
        }

        public new async Task Update(Guid id, User updatedUser)
        {
            var user = await _userRepo.GetById(id);

            if (user == null)
                throw new NotFoundException("User not found");

            user.Name = updatedUser.Name ?? user.Name;
            user.Email = updatedUser.Email ?? user.Email;

            if (user is Client client && updatedUser is Client updatedClient)
            {
                client.Id_Plan = updatedClient.Id_Plan ?? client.Id_Plan;
            }

            await _userRepo.Update(user);
            await _userRepo.Save();
        }

        public async Task<Client?> SubscribeToPlan(SubscribePlanRequest request)
        {
            var client = await _userRepo.GetById(request.ClientId) as Client;

            if (client == null)
                throw new NotFoundException("Client not found");

            var plan = await _planRepo.GetById(request.PlanId);

            if (plan == null)
                throw new NotFoundException("Plan not found");

            client.Id_Plan = plan.Id;
            client.SubscriptionStartDate = DateTime.UtcNow;
            client.SubscriptionEndDate = DateTime.UtcNow.AddMonths(1);
            client.IsActive = true;

            await _userRepo.Update(client);
            await _userRepo.Save();

            return client;
        }

        public async Task<ClientPlanResponse> GetMyPlanStatus()
        {
            var client = await _userRepo.GetById(_userContext.UserId) as Client
                ?? throw new NotFoundException("Client not found");

            return await GetPlanStatusForClient(client);
        }

        public async Task<ClientPlanResponse> GetUserPlan(Guid userId)
        {
            var client = await _userRepo.GetById(userId) as Client
                ?? throw new NotFoundException("Client not found");

            return await GetPlanStatusForClient(client);
        }

        public async Task RemoveUserPlan(Guid userId)
        {
            var client = await _userRepo.GetById(userId) as Client
                ?? throw new NotFoundException("Client not found");

            client.Id_Plan = null;
            client.SubscriptionStartDate = null;
            client.SubscriptionEndDate = null;
            client.IsActive = false;

            await _userRepo.Update(client);
            await _userRepo.Save();
        }

        private async Task<ClientPlanResponse> GetPlanStatusForClient(Client client)
        {
            string? planName = null;
            float? planValue = null;
            int? planMaxClass = null;
            bool? planIsUnlimited = null;

            if (client.Id_Plan.HasValue)
            {
                var plan = await _planRepo.GetById(client.Id_Plan.Value);
                if (plan != null)
                {
                    planName = plan.Name;
                    planValue = plan.Value;
                    planMaxClass = plan.Max_Class;
                    planIsUnlimited = plan.IsUnlimited;
                }
            }

            return new ClientPlanResponse
            {
                PlanId = client.Id_Plan,
                PlanName = planName,
                PlanValue = planValue,
                PlanMaxClass = planMaxClass,
                PlanIsUnlimited = planIsUnlimited,
                IsActive = client.IsActive,
                SubscriptionStartDate = client.SubscriptionStartDate,
                SubscriptionEndDate = client.SubscriptionEndDate,
            };
        }
    }
}