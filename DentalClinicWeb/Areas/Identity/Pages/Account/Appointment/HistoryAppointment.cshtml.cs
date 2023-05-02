using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Appointment
{
    public class HistoryAppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HistoryAppointmentsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<AppointmentViewModel> AcceptedAppointments { get; set; }
        public IList<AppointmentViewModel> DeclinedAppointments { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // get the current doctor's ID
            var user = await _userManager.GetUserAsync(User);
            var currentDoctorId = user.Id;
            AcceptedAppointments = await _context.Appointments
                .Include(a => a.Patients)
                .Include(a => a.Treatment)
                .Where(a => a.Status == AppointmentStatus.Accepted && a.DoctorId == currentDoctorId)
                .ToListAsync();

            DeclinedAppointments = await _context.Appointments
                .Include(a => a.Patients)
                .Include(a => a.Treatment)
                .Where(a => a.Status == AppointmentStatus.Cancelled && a.DoctorId == currentDoctorId)
                .ToListAsync();

            return Page();
        }

    }
}
