using Application.Interfaces;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure { 
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasherService _hasher;
        private readonly IConfiguration _configuration;

        public DatabaseSeeder(
            ApplicationDbContext context,
            IPasswordHasherService hasher,
            IConfiguration configuration)
        {
            _context = context;
            _hasher = hasher;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            var adminExists = await _context.Users
                .OfType<SysAdmin>()
                .AnyAsync();

            if (!adminExists)
            {
                var admin = new SysAdmin
                {
                    Name = _configuration["SeedAdmin:Name"]!,
                    Email = _configuration["SeedAdmin:Email"]!,
                    Password = _hasher.Hash(
                        _configuration["SeedAdmin:Password"]!)
                };

                _context.Users.Add(admin);

                await _context.SaveChangesAsync();
            }
        }
    }
}