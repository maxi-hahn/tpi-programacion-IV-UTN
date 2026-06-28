using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Infrastructure.Service
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;


        public UserContext(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public Guid UserId =>
            Guid.Parse(
                _httpContextAccessor.HttpContext!
                .User
                .FindFirst(ClaimTypes.NameIdentifier)!
                .Value
            );

        public string Email =>
            _httpContextAccessor.HttpContext!
                .User
                .FindFirst(ClaimTypes.Email)!
                .Value;

        public string Role =>
            _httpContextAccessor.HttpContext!
                .User
                .FindFirst(ClaimTypes.Role)!
                .Value;
    }
}
