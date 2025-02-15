using BrainHope.DataAcess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.DataAcess.Contexts
{
    public class BrainHopeDbContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<DoctorPatient> DoctorPatients { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public BrainHopeDbContext(DbContextOptions<BrainHopeDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Define One-to-One Relationship between ApplicationUser and Doctor
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Doctor)
                .WithOne(d => d.AppUser)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            // Define One-to-One Relationship between ApplicationUser and Patient
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Patient)
                .WithOne(p => p.AppUser)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            // Define Composite Primary Key
            builder.Entity<DoctorPatient>()
                .HasKey(dp => new { dp.DoctorId, dp.PatientId });

            // Define Relationships
            builder.Entity<DoctorPatient>()
                .HasOne(dp => dp.Doctor)
                .WithMany(d => d.DoctorPatients)
                .HasForeignKey(dp => dp.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DoctorPatient>()
                .HasOne(dp => dp.Patient)
                .WithMany(p => p.DoctorPatients)
                .HasForeignKey(dp => dp.PatientId)
                .OnDelete(DeleteBehavior.Cascade);


            SeedRoles(builder);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name=SD.Role_Admin , ConcurrencyStamp="1" , NormalizedName=SD.Role_Admin},
                new IdentityRole() { Name=SD.Role_Patient , ConcurrencyStamp="2" , NormalizedName=SD.Role_Patient},
                new IdentityRole() { Name=SD.Role_Doctor , ConcurrencyStamp="3" , NormalizedName=SD.Role_Doctor}
                );
        }
    }
}
