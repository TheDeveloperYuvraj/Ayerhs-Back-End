using Ayerhs.Core.Entities.AccountManagement;
using Microsoft.EntityFrameworkCore;

namespace Ayerhs.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<ClientRoles> ClientRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Clients>(entity => entity.ToTable("tblclients"));
            modelBuilder.Entity<Roles>(entity => entity.ToTable("tblroles"));
            modelBuilder.Entity<ClientRoles>(entity => entity.ToTable("tblclient_roles"));

            modelBuilder.Entity<ClientRoles>()
                .HasKey(cr => new { cr.ClientId, cr.RoleId });

            modelBuilder.Entity<ClientRoles>()
                .HasOne(cr => cr.Client)
                .WithMany(c => c.ClientRoles)
                .HasForeignKey(cr => cr.ClientId);

            modelBuilder.Entity<ClientRoles>()
                .HasOne(cr => cr.Role)
                .WithMany(r => r.ClientRoles)
                .HasForeignKey(cr => cr.RoleId);

            modelBuilder.Entity<Roles>().HasData(
                new Roles { Id = 1, Name = "SuperAdmin", Description = "Super Admin -> Role Full control over the system." },
                new Roles { Id = 2, Name = "Admin", Description = "Admin Role -> Manage users, roles, and some system settings." },
                new Roles { Id = 3, Name = "User", Description = "User Role -> Can access and modify their own data." },
                new Roles { Id = 4, Name = "Viewer", Description = "Viewer Role -> Limited access to view specific data." }
            );
        }
    }
}
