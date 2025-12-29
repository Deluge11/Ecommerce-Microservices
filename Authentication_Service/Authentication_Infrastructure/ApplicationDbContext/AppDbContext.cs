using Authentication_Infrastructure.Entities;
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
                .UsingEntity(ar => ar.ToTable("AccountRoles"));

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity(rp => rp.ToTable("RolePermissions"));

            base.OnModelCreating(modelBuilder);

        }

    }
}
