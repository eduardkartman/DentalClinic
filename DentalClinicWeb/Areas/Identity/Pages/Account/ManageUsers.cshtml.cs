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
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id, string firstName, string lastName, string phoneNumber, string country, string city, string role)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(id.ToString());

            // If user not found, return 404 not found
            if (user == null)
            {
                return NotFound();
            }

            // Get the existing user data from the UserViewModel table
            var userViewModel = await _context.Users.FindAsync(id);

            // Modify the user properties as needed
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

            // Update the user in the AspNetUsers table
            var result = await _userManager.UpdateAsync(user);

            // If update successful, update the user in the UserViewModel table
            if (result.Succeeded)
            {
                userViewModel.FirstName = firstName;
                userViewModel.LastName = lastName;
                userViewModel.PhoneNumber = phoneNumber;
                userViewModel.Country = country;
                userViewModel.City = city;
                userViewModel.Role = role;

                _context.Users.Update(userViewModel);
                await _context.SaveChangesAsync();

                return RedirectToPage("/ManageUsers");
            }
            // If update failed, add error messages to ModelState and return to the edit form
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }
        }



    }
}

