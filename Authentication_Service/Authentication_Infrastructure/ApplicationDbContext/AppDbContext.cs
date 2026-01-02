using Authentication_Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;



namespace Authentication_Infrastructure.ApplicationDbContext
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasMany(a => a.Roles)
                .WithMany(r => r.Accounts)
                .UsingEntity<Dictionary<string, object>>(
                    "AccountRole", 
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    j => j.HasOne<Account>().WithMany().HasForeignKey("AccountId"),
                    j =>
                    {
                        j.HasIndex("AccountId", "RoleId").IsUnique();
                    });

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission", 
                    j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    j =>
                    {
                        j.HasIndex("RoleId", "PermissionId").IsUnique();
                    });

            modelBuilder.Entity<Account>()
            .HasIndex(a => a.Email)
            .IsUnique();


            base.OnModelCreating(modelBuilder);

        }

    }
}
