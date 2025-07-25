using Microsoft.EntityFrameworkCore;
using Localize.API.Models;

namespace Localize.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>()
                .HasKey(c => c.Cnpj);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.User)       
                .WithMany(u => u.Companies) 
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}