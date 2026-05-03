using System;
using System.Collections.Generic;
using System.Text;
using Application.Tools;
using Microsoft.AspNetCore.Identity;




namespace Infrastructure.Service
            
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public bool Verify(string hash, string password)
        {
            var result = _hasher.VerifyHashedPassword(null, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }

}
