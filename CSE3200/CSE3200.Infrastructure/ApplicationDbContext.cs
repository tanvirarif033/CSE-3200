using CSE3200.Domain.Entities;
using CSE3200.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public DbSet<Donation> Donations { get; set; }
        public DbSet<VolunteerAssignment> VolunteerAssignments { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<DisasterAlert> DisasterAlerts { get; set; }

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

            // Add this configuration for ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
                entity.Property(u => u.Address).HasMaxLength(200);
                entity.Property(u => u.City).HasMaxLength(100);
                entity.Property(u => u.State).HasMaxLength(50);
                entity.Property(u => u.ZipCode).HasMaxLength(20);
                entity.Property(u => u.EmergencyContactName).HasMaxLength(100);
                entity.Property(u => u.EmergencyContactPhone).HasMaxLength(20);
                entity.Property(u => u.Skills).HasMaxLength(500);
                entity.Property(u => u.VolunteerRequestStatus).HasMaxLength(20);
                entity.Property(u => u.ProfilePictureUrl).HasMaxLength(500);

                // Indexes for better query performance
                entity.HasIndex(u => u.IsVolunteerRequested);
                entity.HasIndex(u => u.VolunteerRequestStatus);
                entity.HasIndex(u => u.RegistrationDate);
            });

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

            builder.Entity<Donation>(entity =>
            {
                entity.Property(d => d.Amount).HasColumnType("decimal(18,2)");
                entity.Property(d => d.DonorName).IsRequired().HasMaxLength(200);
                entity.Property(d => d.DonorEmail).IsRequired().HasMaxLength(100);
                entity.Property(d => d.DonorPhone).IsRequired().HasMaxLength(20);
                entity.Property(d => d.PaymentMethod).IsRequired();
                entity.Property(d => d.DonationDate).IsRequired();
                entity.Property(d => d.PaymentStatus).HasMaxLength(20);
                entity.Property(d => d.TransactionId).HasMaxLength(50);
                entity.Property(d => d.Notes).HasMaxLength(500);

                // Foreign key relationship
                entity.HasOne(d => d.Disaster)
                      .WithMany()
                      .HasForeignKey(d => d.DisasterId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(d => d.DonorUserId);
                entity.HasIndex(d => d.DisasterId);
                entity.HasIndex(d => d.DonationDate);
                entity.HasIndex(d => d.PaymentStatus);
            });

            builder.Entity<VolunteerAssignment>(entity =>
            {
                entity.Property(va => va.TaskDescription).IsRequired().HasMaxLength(200);
                entity.Property(va => va.Status).IsRequired().HasMaxLength(20);
                entity.Property(va => va.AssignedBy).IsRequired().HasMaxLength(450);
                entity.Property(va => va.VolunteerUserId).IsRequired().HasMaxLength(450);
                entity.Property(va => va.Notes).HasMaxLength(500);

                // Foreign key relationships
                entity.HasOne(va => va.Disaster)
                      .WithMany()
                      .HasForeignKey(va => va.DisasterId)
                      .OnDelete(DeleteBehavior.Cascade);

                // REMOVE THIS - No VolunteerUser navigation property exists
                // entity.HasOne(va => va.VolunteerUser)
                //       .WithMany()
                //       .HasForeignKey(va => va.VolunteerUserId)
                //       .HasPrincipalKey(u => u.Id.ToString())
                //       .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(va => va.DisasterId);
                entity.HasIndex(va => va.VolunteerUserId);
                entity.HasIndex(va => va.Status);
                entity.HasIndex(va => va.AssignedDate);
                entity.HasIndex(va => va.StartDate);
                entity.HasIndex(va => va.EndDate);
            });

            builder.Entity<FAQ>(entity =>
            {
                entity.Property(f => f.Question).IsRequired().HasMaxLength(200);
                entity.Property(f => f.Answer).IsRequired();
                entity.Property(f => f.Category).IsRequired();
                entity.Property(f => f.DisplayOrder).IsRequired();
                entity.Property(f => f.IsActive).IsRequired();
                entity.Property(f => f.CreatedDate).IsRequired();
                entity.Property(f => f.CreatedBy).IsRequired().HasMaxLength(450);

                entity.Property(f => f.ModifiedBy).HasMaxLength(450).IsRequired(false);
                entity.Property(f => f.ModifiedDate).IsRequired(false);

                // Add indexes
                entity.HasIndex(f => f.Category);
                entity.HasIndex(f => f.IsActive);
                entity.HasIndex(f => f.DisplayOrder);
                entity.HasIndex(f => f.CreatedDate);
            });

            builder.Entity<DisasterAlert>(entity =>
            {
                entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Message).IsRequired().HasMaxLength(500);
                entity.Property(a => a.Severity).IsRequired();
                entity.Property(a => a.IsActive).IsRequired();
                entity.Property(a => a.DisplayOrder).IsRequired();
                entity.Property(a => a.CreatedDate).IsRequired();
                entity.Property(a => a.CreatedBy).IsRequired().HasMaxLength(450);
                entity.Property(a => a.ModifiedBy).HasMaxLength(450);

                entity.HasIndex(a => a.Severity);
                entity.HasIndex(a => a.IsActive);
                entity.HasIndex(a => a.StartDate);
                entity.HasIndex(a => a.EndDate);
                entity.HasIndex(a => a.DisplayOrder);
            });
        }
    }
}