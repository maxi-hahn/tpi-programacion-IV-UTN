using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Domain.Entity.UsersChild;
using Microsoft.EntityFrameworkCore;

namespace Trabajop4.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }   
        public DbSet<Schedule> Scheduless { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SysAdmin> SysAdmins { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }  
    }
}