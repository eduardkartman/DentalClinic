using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Appointment
{
    public class CompleteAppModel : PageModel
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public List<AppointmentViewModel> Appointments { get; set; }

        public CompleteAppModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            Appointments = new List<AppointmentViewModel>();
        }

        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var dotorId = user.Id;

           // Appointments = await _context.Appointments
            //    .Include(a => a.Patients)
             //   .FirstOrDefaultAsync(a => a.Id == id);

            if (Appointments == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id, string doctorConclusions)
        {
            var user = await _userManager.GetUserAsync(User);
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Completed;
            appointment.DoctorConclusion = doctorConclusions;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            // Send notification to the linked patient's ID
            var notification = new NotificationModel
            {
                ReceiverId = appointment.PatientId,
                SenderId = user.Id,
                Message = "Your appointment has been completed. Please check the doctor's conclusions.",
                IsRead = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Appointment/Index");
        }
    }
}
