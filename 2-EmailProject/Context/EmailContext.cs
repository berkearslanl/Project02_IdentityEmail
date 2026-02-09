using _2_EmailProject.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _2_EmailProject.Context
{
    public class EmailContext:IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LAPTOP-5Q1ARH5E;initial catalog=Project2EmailDb;integrated security=true;TrustServerCertificate=True");
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Category> Categories { get; set; }
    }

    
}
