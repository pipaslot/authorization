using Demo.App.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.App.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(t => new { t.UserId, t.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(pt => pt.UserId);
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);
            modelBuilder.Entity<Role>()
                .HasMany(u => u.Permissions)
                .WithOne(r => r.Role)
                .HasForeignKey(r => r.RoleId);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Company> Company { get; set; }
    }
}
