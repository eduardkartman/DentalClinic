using DentalClinicWeb.Constans;
using DentalClinicWeb.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace DentalClinicWeb.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Patient.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Doctor.ToString()));

            // creating admin
            var user = new ApplicationUser
            {
                UserName = "admin123",
                Email = "admin@gmail.com",
                FirstName = "admin",
                LastName = "admin",
                EmailConfirmed = true
            };
            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                userManager.CreateAsync(user, "Admin123!") ;
                userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        }
    }
}
