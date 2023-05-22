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
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Common;
using Microsoft.AspNetCore.SignalR;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Appointment
{
    public class AppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly smsSender _smsSender;

        public IList<TreatmentsViewModel> Treatments { get; set; }
        public IList<DoctorViewModel> Doctors { get; set; }

        public AppointmentModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubContext<NotificationsHub> hubContext, smsSender smsSender)
        {
            _context = context;
            _userManager = userManager;
            Treatments = new List<TreatmentsViewModel>();
            Doctors = new List<DoctorViewModel>();
            _hubContext = hubContext;
            _smsSender = smsSender;
        }

        [BindProperty]
        public AppointmentViewModel Appointment { get; set; }

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
            // Save the appointment to the database
            var user = await _userManager.GetUserAsync(User);
            var patientId = user.Id;

            var patient = await _context.Patients.FindAsync(patientId);

            var doctorId = Appointment.DoctorId;
            var doctor = await _context.Doctors.FindAsync(doctorId);

            var treatmentId = Appointment.TreatmentId;
            var treatment = await _context.Treatments.FindAsync(treatmentId);

            ModelState.Clear();

            // Set the starting time and end time of the new appointment based on the selected treatment duration
            DateTime startingAppointmentDateTime = DateTime;
            DateTime endAppointmentDateTime = startingAppointmentDateTime.AddMinutes((double)treatment.DurationInMinutes);
            DateTime openingTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 9, 0, 0);
            DateTime closingTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 18, 0, 0);

            // Verify that the appointment time falls between 9 AM and 6 PM
            if (startingAppointmentDateTime.TimeOfDay < openingTime.TimeOfDay || endAppointmentDateTime.TimeOfDay > closingTime.TimeOfDay)
            {
                ModelState.AddModelError(string.Empty, "The selected appointment time is outside the working hours (9AM-6PM). Please choose another time.");
                Doctors = await _context.Doctors.ToListAsync();
                Treatments = await _context.Treatments.ToListAsync();
                return Page();
            }

            // Check if the selected date and time is available for the selected doctor
            var appointmentsForDoctor = await _context.Appointments
                .Where(a => a.DoctorId == Appointment.DoctorId
                    && a.AppointmentDateTime.Date == startingAppointmentDateTime.Date
                    && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();


            if (appointmentsForDoctor.Count >= 5)
            {
                ModelState.AddModelError(string.Empty, $"The selected doctor already has reached the maximum appointments number for the selected day. Please choose another doctor or another date.");
                Doctors = await _context.Doctors.ToListAsync();
                Treatments = await _context.Treatments.ToListAsync();
                return Page();
            }

            // Check if the selected treatment overlaps with any existing appointments for the selected doctor
            if (appointmentsForDoctor.Count >= 1)
            {
                // Sort the appointments by the start time
                appointmentsForDoctor = appointmentsForDoctor.OrderBy(a => a.AppointmentDateTime).ToList();
                int numberOfGaps = appointmentsForDoctor.Count + 1;
                int[] gapsInMinutes = new int[numberOfGaps];
                DateTime[] gapStartTimes = new DateTime[appointmentsForDoctor.Count + 1];

                // Calculate the gaps between appointments in minutes
                for (int i = 0; i < numberOfGaps; i++)
                {
                    if (i == 0)
                    {
                        gapsInMinutes[i] = (int)(appointmentsForDoctor[i].AppointmentDateTime - openingTime).TotalMinutes;
                        if (gapsInMinutes[i] != 0)
                        {
                            gapStartTimes[i] = openingTime;
                        }
                    }
                    else if (i == (numberOfGaps - 1))
                    {
                        gapsInMinutes[i] = (int)(closingTime - appointmentsForDoctor[i - 1].EndAppointmentDateTime).TotalMinutes;
                        gapStartTimes[i] = appointmentsForDoctor[i - 1].EndAppointmentDateTime;
                    }
                    else
                    {
                        gapsInMinutes[i] = (int)(appointmentsForDoctor[i].AppointmentDateTime.TimeOfDay - appointmentsForDoctor[i - 1].EndAppointmentDateTime.TimeOfDay).TotalMinutes;
                        gapStartTimes[i] = appointmentsForDoctor[i - 1].EndAppointmentDateTime;
                    }
                }


                for (int i = 0; i <= appointmentsForDoctor.Count; i++)
                {
                    if (i <= appointmentsForDoctor.Count && gapsInMinutes[i] > treatment.DurationInMinutes )
                    {
                        // There is a gap big enough to fit the appointment
                        DateTime gapStart = gapStartTimes[i];
                        DateTime gapEnd = (i < appointmentsForDoctor.Count) ? appointmentsForDoctor[i].AppointmentDateTime : closingTime;


                        int gapDuration = gapsInMinutes[i];
                        if (gapDuration > treatment.DurationInMinutes && startingAppointmentDateTime > gapStart && endAppointmentDateTime <= gapEnd)
                        {
                            // Use the exact gap that matches the selected starting and ending time
                            var appointment = new AppointmentViewModel
                            {
                                PatientId = patient.Id,
                                PatientName = patient.FirstName + patient.LastName,
                                PatientEmail = patient.Email,
                                PatientPhoneNumber = patient.PhoneNumber,
                                DoctorId = doctorId,
                                DoctorName = doctor.FirstName + doctor.LastName,
                                DoctorEmail = doctor.Email,
                                DoctorPhoneNumber = doctor.PhoneNumber,
                                TreatmentId = treatmentId,
                                TreatmentName = treatment.Name,
                                TreatmentPrice = treatment.Price,
                                TreatmentDuration = treatment.DurationInMinutes,
                                TreatmentEnabled = treatment.IsAvailable,
                                AppointmentDateTime = startingAppointmentDateTime,
                                EndAppointmentDateTime = endAppointmentDateTime,
                            };

                            _context.Appointments.Add(appointment);
                            ModelState.AddModelError(string.Empty, "Your appointment has been saved succesfully!");
                            await _context.SaveChangesAsync();

                            // Create a new notification object
                            var notification = new NotificationModel
                            {
                                AppointmentId = appointment.Id,
                                Message = "You have a new appointment! Click here to see it.",
                                ReceiverId = appointment.DoctorId,
                                SenderId = appointment.PatientId,
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
                                Message = $"Thank you for choosing us! Your appointment has been created. Appointment: {appointment.AppointmentDateTime} - {appointment.EndAppointmentDateTime}",
                                IsSent = false,
                                CreatedAt = DateTime.Now,
                            };
                            // Add the sms to the database
                            await _context.SMS.AddAsync(sms);

                            await _context.SaveChangesAsync();

                            SendSMS.sendSMS(sms.PhoneNumber, sms.Message);
                            
                            return Page();
                        }
                        else if (gapDuration > treatment.DurationInMinutes && gapEnd > endAppointmentDateTime)
                        {
                            // Recommend the available gap
                            string recommendedTime = $"{gapStart.AddMinutes(5).ToString("hh:mm tt")} - {gapEnd.AddMinutes(-5).ToString("hh:mm tt")}";
                            ModelState.AddModelError(string.Empty, $"The selected treatment requires {treatment.DurationInMinutes} minutes. We recommend using the available gap between {recommendedTime}.");
                            Doctors = await _context.Doctors.ToListAsync();
                            Treatments = await _context.Treatments.ToListAsync();
                            return Page();
                        }
                    }
                }

            }
            else if (appointmentsForDoctor.Count == 0)
            {
                // Use the exact gap that matches the selected starting and ending time
                var appointment = new AppointmentViewModel
                {
                    PatientId = patient.Id,
                    PatientName = patient.FirstName + patient.LastName,
                    PatientEmail = patient.Email,
                    PatientPhoneNumber = patient.PhoneNumber,
                    DoctorId = doctorId,
                    DoctorName = doctor.FirstName + doctor.LastName,
                    DoctorEmail = doctor.Email,
                    DoctorPhoneNumber = doctor.PhoneNumber,
                    TreatmentId = treatmentId,
                    TreatmentName = treatment.Name,
                    TreatmentPrice = treatment.Price,
                    TreatmentDuration = treatment.DurationInMinutes,
                    TreatmentEnabled = treatment.IsAvailable,
                    AppointmentDateTime = startingAppointmentDateTime,
                    EndAppointmentDateTime = endAppointmentDateTime,
                };

                _context.Appointments.Add(appointment);
                ModelState.AddModelError(string.Empty, "Your appointment has been saved succesfully!");
                await _context.SaveChangesAsync();
                // Create a new notification object
                var notification = new NotificationModel
                {
                    AppointmentId = appointment.Id,
                    Message = "You have a new appointment! Click here to see it.",
                    ReceiverId = appointment.DoctorId,
                    SenderId = appointment.PatientId,
                    CreatedAt = DateTime.Now,
                };

                // Add the notification to the database
                await _context.Notifications.AddAsync(notification);

                // Send the notification to the doctor using SignalR
                var hubContext = _hubContext.Clients.User(appointment.DoctorId);
                await hubContext.SendAsync("SendNotification", appointment.DoctorId, notification.Message);

                var sms = new SMS
                {
                    PhoneNumber = $"+4{appointment.PatientPhoneNumber}",
                    Message = $"Thank you for choosing us! Your appointment has been created. Appointment: {appointment.AppointmentDateTime} - {appointment.EndAppointmentDateTime}",
                    IsSent = false,
                    CreatedAt = DateTime.Now,
                };
                // Add the sms to the database
                await _context.SMS.AddAsync(sms);
                await _context.SaveChangesAsync();

                SendSMS.sendSMS(sms.PhoneNumber, sms.Message);
                return Page();

            }
            // No available gaps were found
            ModelState.AddModelError(string.Empty, $"The selected treatment requires {treatment.DurationInMinutes} minutes, but there are no available gaps today. Please choose another day.");
            Doctors = await _context.Doctors.ToListAsync();
            Treatments = await _context.Treatments.ToListAsync();
            return Page();

        }
    }

}