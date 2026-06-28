using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Inscription> Inscriptions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SysAdmin> SysAdmins { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin")
                .HasValue<SysAdmin>("SysAdmin");

            base.OnModelCreating(modelBuilder);
        }
    }
}