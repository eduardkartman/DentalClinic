using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Threading.Tasks;
using DentalClinicWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalClinicWeb.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class EditUsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditUsersModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Id { get; set; }

            [Required(ErrorMessage = "The First Name field is required.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "The Last Name field is required.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "The Phone Number field is required.")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }

            [Required(ErrorMessage = "The Role field is required.")]
            [Display(Name = "Role")]
            public string Role { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                City = user.City,
                ZipCode = user.ZipCode,
                Role = user.Role,
            };
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Input.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;
                user.Country = Input.Country;
                user.City = Input.City;
                user.ZipCode = Input.ZipCode;
                user.Role = Input.Role;

                await _userManager.UpdateAsync(user);

                var userViewModel = await _context.Users.FindAsync(Input.Id);
                if (userViewModel == null)
                {
                    return NotFound();
                }

                userViewModel.FirstName = Input.FirstName;
                userViewModel.LastName = Input.LastName;
                userViewModel.Role = Input.Role;

                await _context.SaveChangesAsync();

                return RedirectToPage("/ManageUsers");
            }

            return Page();
        }

    }
}
