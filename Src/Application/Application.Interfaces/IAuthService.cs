using Application.Dtos.Responses;
using Application.Dtos.Request;
namespace Application.Interfaces
{
    public interface IAuthService
    {

        Task<AuthResponse?> SingIn(SingInRequest request);

        Task<AuthResponse?> SingUp(SingUpRequest request);

        Task<bool> VerifyEmail(string token);
        Task<bool> ResendVerificationEmail(string? email = null);

        Task<bool> ForgotPassword(string email);

        Task<bool> ResetPassword(
            string token,
            string newPassword);
    }
}
