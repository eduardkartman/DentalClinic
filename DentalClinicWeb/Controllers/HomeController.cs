using DentalClinicWeb.Areas.Identity.Pages.Account;
using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using System.Security.AccessControl;

namespace DentalClinicWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> PatientView()
        {
            var user = _userManager.GetUserAsync(User).Result;
            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;
            ViewBag.Email = user.Email;
            ViewBag.PhoneNumber = user.PhoneNumber;
            ViewBag.Country = user.Country;
            ViewBag.City = user.City;
            ViewBag.ZipCode = user.ZipCode;
            ViewBag.Role = user.Role;

            var patientId = user.Id;

            var todaysAppointments = await _context.Appointments
                    .Include(a => a.Treatment)
                    .Include(a => a.Doctors)
                    .OrderBy(a => a.AppointmentDateTime)
                    .Where(a => a.PatientId == patientId && a.AppointmentDateTime.Date == DateTime.Now.Date && a.Status != AppointmentStatus.Cancelled)
                    .Select(a => new AppointmentViewModel
                    {
                        Id = a.Id,
                        TreatmentName = a.Treatment.Name,
                        AppointmentDateTime = a.AppointmentDateTime,
                        EndAppointmentDateTime = a.EndAppointmentDateTime,
                        TreatmentPrice = a.TreatmentPrice,
                        DoctorEmail = a.DoctorEmail,
                        DoctorPhoneNumber = a.DoctorPhoneNumber,
                        Status = a.Status,
                    })
                    .ToListAsync();
            ViewBag.TodaysAppointments = todaysAppointments;

            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Treatment)
                .Include(a => a.Doctors)
                .OrderBy(a => a.AppointmentDateTime)
                .Where(a => a.PatientId == patientId && a.AppointmentDateTime.Date > DateTime.Now.Date && a.Status != AppointmentStatus.Cancelled)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    TreatmentName = a.Treatment.Name,
                    AppointmentDateTime = a.AppointmentDateTime,
                    EndAppointmentDateTime = a.EndAppointmentDateTime,
                    TreatmentPrice = a.Treatment.Price,
                    DoctorEmail = a.Doctors.Email,
                    DoctorPhoneNumber = a.Doctors.PhoneNumber,
                    Status = a.Status,
                })
                .ToListAsync();

            ViewBag.UpcomingAppointments = upcomingAppointments;

            return View();
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DoctorView()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var doctorId = user.Id;
            var todaysAppointments = await _context.Appointments
                           .Include(a => a.Treatment)
                           .Include(a => a.Patients)
                           .OrderBy(a => a.AppointmentDateTime)
                           .Where(a => a.DoctorId == doctorId && a.AppointmentDateTime.Date == DateTime.Now.Date && a.Status != AppointmentStatus.Cancelled)
                           .Select(a => new AppointmentViewModel
                           {
                               Id = a.Id,
                               TreatmentName = a.Treatment.Name,
                               AppointmentDateTime = a.AppointmentDateTime,
                               EndAppointmentDateTime = a.EndAppointmentDateTime,
                               TreatmentPrice = a.Treatment.Price,
                               DoctorEmail = a.Doctors.Email,
                               DoctorPhoneNumber = a.Doctors.PhoneNumber,
                               Status = a.Status,
                           })
                           .ToListAsync();

            ViewBag.TodaysAppointments = todaysAppointments;

            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Treatment)
                .Include(a => a.Patients)
                .OrderBy(a => a.AppointmentDateTime)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDateTime.Date > DateTime.Now.Date && a.Status != AppointmentStatus.Cancelled)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    TreatmentName = a.Treatment.Name,
                    AppointmentDateTime = a.AppointmentDateTime,
                    EndAppointmentDateTime = a.EndAppointmentDateTime,
                    TreatmentPrice = a.Treatment.Price,
                    DoctorEmail = a.Doctors.Email,
                    DoctorPhoneNumber = a.Doctors.PhoneNumber,
                    Status = a.Status,
                })
                .ToListAsync();

            ViewBag.UpcomingAppointments = upcomingAppointments;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}