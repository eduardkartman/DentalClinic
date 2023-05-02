using DentalClinicWeb.Constans;
using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Appointment
{
    [Authorize(Roles = "Doctor")]
    public class DoctorAppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorAppointmentModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<AppointmentViewModel> Appointments { get; set; }
        public AppointmentStatus AppointmentsStatus { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var doctorId = user.Id;

            Appointments = await _context.Appointments
                .Include(a => a.Patients)
                .Include(a => a.Treatment)
                .Where(a => a.DoctorId == doctorId && a.Status == 0)
                .ToArrayAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int appointmentId, string status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            if (status == "Accepted")
            {
                appointment.Status = AppointmentStatus.Accepted;
            }
            else if (status == "Cancelled")
            {
                appointment.Status = AppointmentStatus.Cancelled;
            }

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
