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
            var client = (Client) await _userRepo.GetById(userId);
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
            var client =
                await _userRepo.GetById(request.ClientId)
                as Client;

            if (client == null)
                throw new NotFoundException("Client not found");

            var plan =
                await _planRepo.GetById(request.PlanId);

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
    }
    }