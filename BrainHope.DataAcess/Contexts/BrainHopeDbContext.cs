using BrainHope.DataAcess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.DataAcess.Contexts
{
    public class BrainHopeDbContext:IdentityDbContext<ApplicationUser>
    {
        public BrainHopeDbContext(DbContextOptions<BrainHopeDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
