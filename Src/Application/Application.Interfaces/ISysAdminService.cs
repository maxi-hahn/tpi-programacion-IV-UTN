using Application.Dtos.Request;
using Domain.Entity;

namespace Application.Interfaces
{
    public interface ISysAdminService : IUserService
    {
        Task<User?> UpgradeUsersRol(UpgradeUsersRol request);
        Task<User> ToggleUserStatus(Guid userId);

    }
}
