using CSE3200.Domain.Entities;
using CSE3200.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSE3200.Infrastructure.Identity;
using CSE3200.Infrastructure.Seeds;

namespace CSE3200.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
        ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole,
        ApplicationUserLogin, 
        ApplicationRoleClaim,
        ApplicationUserToken>
    {
        private readonly string _connectionString;
        private readonly string _migrationAssembly;

        public DbSet<Product> Products { get; set; }
        public DbSet<Disaster> Disasters { get; set; }

        public ApplicationDbContext(string connectionString, string migrationAssembly)
        {
            _migrationAssembly = migrationAssembly;
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString,
                    x => x.MigrationsAssembly(_migrationAssembly));
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed data
            builder.Entity<ApplicationRole>().HasData(RoleSeed.GetRoles());
            builder.Entity<ApplicationUserClaim>().HasData(ClaimSeed.GetClaims());

            builder.Entity<Disaster>(entity =>
            {
                entity.Property(d => d.Title).IsRequired().HasMaxLength(200);
                entity.Property(d => d.Description).IsRequired();
                entity.Property(d => d.Location).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Severity).IsRequired();
                entity.Property(d => d.Status).IsRequired();
                entity.Property(d => d.CreatedBy).IsRequired().HasMaxLength(450);
                entity.Property(d => d.CreatedDate).IsRequired();
                entity.Property(d => d.RequiredAssistance).IsRequired();
                entity.Property(d => d.AffectedPeople).IsRequired();

                // Make ApprovedBy and ApprovedDate optional
                entity.Property(d => d.ApprovedBy).HasMaxLength(450);
                entity.Property(d => d.ApprovedDate).IsRequired(false);

                // Add indexes
                entity.HasIndex(d => d.Status);
                entity.HasIndex(d => d.CreatedBy);
                entity.HasIndex(d => d.CreatedDate);
            });

        }
    }
}
