using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DentalClinicWeb.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalClinicWeb.Models
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = new List<UserViewModel>();

            foreach (var user in _userManager.Users)
            {
                Users.Add(new UserViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                });
            }
        }
    }

    public class UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
