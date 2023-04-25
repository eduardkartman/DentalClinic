using DentalClinicWeb.Data;
using DentalClinicWeb.Data.Migrations;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateDoctorModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public DoctorViewModel Doctors { get; set; }

        public CreateDoctorModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            var doctor = new DoctorViewModel
            {
                Id = GenerateNewId(),
                Email = Doctors.Email,
                FirstName = Doctors.FirstName,
                LastName = Doctors.LastName,
                PhoneNumber = Doctors.PhoneNumber,
                City = Doctors.City,
                Country = Doctors.Country,
                ZipCode = Doctors.ZipCode,
            };

            _context.Doctors.Add(doctor);

            var user = new UserViewModel 
            { 
                UserId = doctor.Id,
                Email = doctor.Email,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = doctor.PhoneNumber,
                City = doctor.City,
                Country = doctor.Country,
                ZipCode = doctor.ZipCode,
                Role = "Doctor"
            };

            var aspUser = new ApplicationUser
            { 
                Id = doctor.Id,
                Email = doctor.Email,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = doctor.PhoneNumber,
                City = doctor.City,
                Country = doctor.Country,
                ZipCode = doctor.ZipCode,

            };

            await _context.SaveChangesAsync();



            return RedirectToPage("/Account/Treatments/Visit", new { area = "Identity" });
        }

        private string GenerateNewId()
        {
            return Guid.NewGuid().ToString();
        }


    }

}
