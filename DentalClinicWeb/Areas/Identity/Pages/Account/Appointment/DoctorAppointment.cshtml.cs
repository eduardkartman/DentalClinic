using DentalClinicWeb.Constans;
using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Appointment
{
    [Authorize(Roles = "Doctor")]
    public class DoctorAppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public DoctorAppointmentModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubContext<NotificationsHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
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
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                // Create a new notification object
                var notification = new NotificationModel
                {
                    AppointmentId = appointmentId,
                    Message = $"Your appointment has been accepted! {appointment.TreatmentName} - {appointment.AppointmentDateTime}",
                    ReceiverId = appointment.PatientId,
                    SenderId = appointment.DoctorId,
                    CreatedAt = DateTime.Now
                };

                // Add the notification to the database
                await _context.Notifications.AddAsync(notification);

                // Send the notification to the doctor using SignalR
                var hubContext = _hubContext.Clients.User(appointment.DoctorId);
                await hubContext.SendAsync("SendNotification", appointment.DoctorId, notification.Message);


                var sms = new SMS
                {
                    PhoneNumber = $"+4{appointment.PatientPhoneNumber}",
                    Message = $"We are happy to announce you that your appointment has been accepted by our doctor. Appointment's date: {appointment.AppointmentDateTime} - {appointment.EndAppointmentDateTime}",
                    IsSent = false,
                    CreatedAt = DateTime.Now,
                };
                // Add the sms to the database
                await _context.SMS.AddAsync(sms);

                await _context.SaveChangesAsync();

                SendSMS.sendSMS(sms.PhoneNumber, sms.Message);
                return RedirectToPage();

            }

            else 
                if (status == "Cancelled")
            {
                appointment.Status = AppointmentStatus.Cancelled;
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                // Create a new notification object
                var notification = new NotificationModel
                {
                    AppointmentId = appointmentId,
                    Message = $"Your appointment has been cancelled! We recommend you to reschedule!",
                    ReceiverId = appointment.PatientId,
                    SenderId = appointment.DoctorId,
                    CreatedAt = DateTime.Now
                };

                // Add the notification to the database
                await _context.Notifications.AddAsync(notification);

                // Send the notification to the doctor using SignalR
                var hubContext = _hubContext.Clients.User(appointment.DoctorId);
                await hubContext.SendAsync("SendNotification", appointment.DoctorId, notification.Message);

                var sms = new SMS
                {
                    PhoneNumber = $"+4{appointment.PatientPhoneNumber}",
                    Message = $"Unfortunately your appointment has been cancelled by our doctor. We recommend you to reschedule.",
                    IsSent = false,
                    CreatedAt = DateTime.Now,
                };
                // Add the sms to the database
                await _context.SMS.AddAsync(sms);

                await _context.SaveChangesAsync();
                
                SendSMS.sendSMS(sms.PhoneNumber, sms.Message);
                return RedirectToPage();
            }

            return RedirectToPage();
        }
    }
}
