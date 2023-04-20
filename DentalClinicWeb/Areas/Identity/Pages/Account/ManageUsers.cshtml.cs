using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DentalClinicWeb.Data;
using DentalClinicWeb.Data.Migrations;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Areas.Identity.Pages
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ManageUsersModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            Users = new List<UserViewModel>();
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _context.Users
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Country = u.Country,
                    City = u.City,
                    ZipCode = u.ZipCode,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            // Get the user from the AspNetUsers table
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                // Delete the user from the AspNetUsers table
                await _userManager.DeleteAsync(user);

                // Get the user from the UserViewModel table
                var userViewModel = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

                if (userViewModel != null)
                {
                    // Delete the user from the UserViewModel table
                    _context.Users.Remove(userViewModel);

                    // Check if the user is a patient
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
                    if (patient != null)
                    {
                        // Delete the patient from the Patient table
                        _context.Patients.Remove(patient);
                    }

                    // Check if the user is a doctor
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
                    if (doctor != null)
                    {
                        // Delete the doctor from the Doctor table
                        _context.Doctors.Remove(doctor);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToPage("/Account/ManageUsers", new { area = "Identity" });

            }
            else
            {
                return NotFound();
            }
        }

    }
}

