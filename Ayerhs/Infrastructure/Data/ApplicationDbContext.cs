using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.UserManagement;
using Ayerhs.Core.Entities.Utility;
using Microsoft.EntityFrameworkCore;

namespace Ayerhs.Infrastructure.Data
{
    /// <summary>
    /// This class represents the application database context derived from DbContext.
    /// </summary>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        /// <summary>
        /// DbSet for the Clients entity.
        /// </summary>
        public DbSet<Clients> Clients { get; set; }

        /// <summary>
        /// DbSet for the Roles entity.
        /// </summary>
        public DbSet<Roles> Roles { get; set; }

        /// <summary>
        /// DbSet for the ClientRoles entity.
        /// </summary>
        public DbSet<ClientRoles> ClientRoles { get; set; }

        /// <summary>
        /// DbSet for the OtpStorage entity.
        /// </summary>
        public DbSet<OtpStorage> OtpStorages { get; set; }

        /// <summary>
        /// DbSet for the Partiton entity.
        /// </summary>
        public DbSet<Partition> Partitions { get; set; }

        /// <summary>
        /// Configures entity mappings and relationships for the database model.
        /// </summary>
        /// <param name="modelBuilder">The model builder instance for configuration.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Clients>(entity => entity.ToTable("tblclients"));
            modelBuilder.Entity<Roles>(entity => entity.ToTable("tblroles"));
            modelBuilder.Entity<ClientRoles>(entity => entity.ToTable("tblclient_roles"));
            modelBuilder.Entity<OtpStorage>(entity => entity.ToTable("tblotp_storage"));
            modelBuilder.Entity<Partition>(entity => entity.ToTable("tblpartitions"));

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
