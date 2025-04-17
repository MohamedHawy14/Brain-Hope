using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Models.Chat;
using BrainHope.DataAcess.Models.Posts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
        public DbSet<Admin> Admins { get; set; }
        public DbSet<DoctorPatient> DoctorPatients { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }


        // public DbSet<Appointment> Appointments { get; set; }

        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
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

            // Define One-to-One Relationship between ApplicationUser and Admin
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Admin)
                .WithOne(p => p.AppUser)
                .HasForeignKey<Admin>(p => p.UserId)
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

           //builder.Entity<Appointment>()
           //    .HasOne(a => a.Doctor)
           //    .WithMany(d => d.Appointments) 
           //    .HasForeignKey(a => a.DoctorId)
           //    .OnDelete(DeleteBehavior.Cascade); 

           // builder.Entity<Appointment>()
           //     .HasOne(a => a.Patient)
           //     .WithMany(p => p.Appointments) 
           //     .HasForeignKey(a => a.PatientId)
           //     .OnDelete(DeleteBehavior.Cascade);

            //chat 
            builder.Entity<UserConnection>()
           .HasOne(uc => uc.User)
           .WithMany(u => u.UserConnections)
           .HasForeignKey(uc => uc.UserId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatMessage>()
               .HasOne(m => m.Sender)
               .WithMany()
               .HasForeignKey(m => m.SenderId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete


            //medical history with patient ya fannnnan
            builder.Entity<MedicalHistory>()
                .HasOne(h => h.Patient)
                .WithMany()
                .HasForeignKey(h => h.PatientId);


            //SeedRoles(builder);
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
