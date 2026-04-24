using Domain.Entity;
using Domain.Entity.UsersChild;
using Microsoft.EntityFrameworkCore;

namespace TrabajoP4.Infrastructure.Persistance
{
    public class GestionComplejoDbContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }   
        public DbSet<Schedule> Scheduless { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SysAdmin> SysAdmins { get; set; }

        public GestionComplejoDbContext(DbContextOptions<GestionComplejoDbContext> options) : base(options)
        {

        }  
    }
}