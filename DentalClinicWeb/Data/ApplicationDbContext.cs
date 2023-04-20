using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Numerics;
using System.Reflection.Emit;

namespace DentalClinicWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<UserViewModel> Users { get; set; }
        public DbSet<PatientViewModel> Patients { get; set; }
        public DbSet<DoctorViewModel> Doctors { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ApplicationEntityConfiguration());

        }
    }
}

//Added more fields to AspNetUsers table
public class ApplicationEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(50);
        builder.Property(u => u.LastName).HasMaxLength(50);
        builder.Property(u => u.PhoneNumber).HasMaxLength(10);
        builder.Property(u => u.Country).HasMaxLength(50);
        builder.Property(u => u.City).HasMaxLength(50);
        builder.Property(u => u.ZipCode).HasMaxLength(10);
        builder.Property(u => u.Role).HasMaxLength(50);
    }
}
