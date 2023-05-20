using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Users
{
    public class PatientsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public IList<AppointmentViewModel> Appointments { get; set; }

        public PatientsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            Appointments = new List<AppointmentViewModel>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var dotorId = user.Id;

            Appointments= await _context.Appointments
                .Include(a => a.Patients)
                .Where(a => a.DoctorId == dotorId && a.AppointmentDateTime < DateTime.Now && (a.Status == AppointmentStatus.Accepted || a.Status == AppointmentStatus.Completed))
                .ToListAsync();

            return Page();
        }
    }
}
