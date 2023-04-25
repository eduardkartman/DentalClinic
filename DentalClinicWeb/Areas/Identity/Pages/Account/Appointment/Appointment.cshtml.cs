using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Appointment
{
    public class AppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IList<TreatmentsViewModel> Treatments { get; set; }
        public IList<DoctorViewModel> Doctors { get; set; }

        public AppointmentModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            Treatments = new List<TreatmentsViewModel>();
            Doctors = new List<DoctorViewModel>();
        }

        [BindProperty]
        public AppointmentViewModel Appointment { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public DateTime DateTime { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            Doctors = await _context.Doctors.ToListAsync();
            Treatments = await _context.Treatments.ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the selected date and time is available for the selected doctor
            var appointmentsForDoctor = await _context.Appointments
                .Where(a => a.DoctorId == Appointment.DoctorId && a.AppointmentDateTime.Date == Appointment.AppointmentDateTime.Date)
                .ToListAsync();
            var timeSlotTaken = appointmentsForDoctor.Any(a => (Appointment.AppointmentDateTime - a.AppointmentDateTime).TotalMinutes < 120);

            if (timeSlotTaken)
            {
                ModelState.AddModelError(string.Empty, "The selected time slot is already taken. Please choose another one.");
                Doctors = await _context.Doctors.ToListAsync();
                Treatments = await _context.Treatments.ToListAsync();
                return Page();
            }

            // Check if the selected doctor already has 5 appointments for the selected day
            var numberOfAppointmentsForDoctor = appointmentsForDoctor.Count();
            if (numberOfAppointmentsForDoctor >= 5)
            {
                ModelState.AddModelError(string.Empty, "The selected doctor already has 5 appointments for the selected day. Please choose another doctor or another date.");
                Doctors = await _context.Doctors.ToListAsync();
                Treatments = await _context.Treatments.ToListAsync();
                return Page();
            }

            // Save the appointment to the database
            var user = await _userManager.GetUserAsync(User);

            var doctorId = Request.Form["doctorId"];
            var doctor = await _context.Doctors.FindAsync(doctorId);

            var treatmentId = int.Parse(Request.Form["treatmentId"]);
            var treatment = await _context.Treatments.FindAsync(treatmentId);


            var appointment = new AppointmentViewModel
            {
                PatientId = user.Id,
                PatientName = user.FirstName + user.LastName,
                PatientEmail = user.Email,
                PatientPhoneNumber = user.PhoneNumber,
                DoctorId = doctor.Id,
                DoctorName = doctor.FirstName + doctor.LastName,
                DoctorEmail = doctor.Email,
                DoctorPhoneNumber = doctor.PhoneNumber,
                TreatmentId = treatment.Id,
                TreatmentName = treatment.Name,
                TreatmentPrice = treatment.Price,
                TreatmentDuration = treatment.DurationInMinutes,
                AppointmentDateTime = DateTime,
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();


            return RedirectToAction("/Home/Index");
        }

    }
}