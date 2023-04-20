using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Threading.Tasks;
using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class EditUsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditUsersModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Id { get; set; }

            [Required(ErrorMessage = "The First Name field is required.")]
            [Display(Name = "First Name")]
            public string? FirstName { get; set; }

            [Required(ErrorMessage = "The Last Name field is required.")]
            [Display(Name = "Last Name")]
            public string? LastName { get; set; }

            [Required(ErrorMessage = "The Phone Number field is required.")]
            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Country")]
            public string? Country { get; set; }

            [Display(Name = "City")]
            public string? City { get; set; }

            [Display(Name = "Zip Code")]
            public string? ZipCode { get; set; }

            [Required(ErrorMessage = "The Role field is required.")]
            [Display(Name = "Role")]
            public string Role { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            string userId = Request.Query["id"];

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                City = user.City,
                ZipCode = user.ZipCode,
                Role = user.Role,
            };

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Input.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;
                user.Country = Input.Country;
                user.City = Input.City;
                user.ZipCode = Input.ZipCode;
                user.Role = Input.Role;

                // Get the user's current roles
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove the user's current roles
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add the new role
                await _userManager.AddToRoleAsync(user, Input.Role);

                await _userManager.UpdateAsync(user);

                var userViewModel = await _context.Users.FindAsync(Input.Id);

                if (userViewModel == null)
                {
                    return NotFound();
                }

                userViewModel.FirstName = Input.FirstName;
                userViewModel.LastName = Input.LastName;
                userViewModel.PhoneNumber = Input.PhoneNumber;
                userViewModel.Country = Input.Country;
                userViewModel.City = Input.City;
                userViewModel.ZipCode = Input.ZipCode;
                userViewModel.Role = Input.Role;

                // Update DoctorViewModel and PatientViewModel tables
                if (Input.Role == "Doctor")
                {
                    var doctorViewModel = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == userViewModel.UserId);
                    var patientViewModel = await _context.Patients.FirstOrDefaultAsync(p => p.Id == userViewModel.UserId);

                    if (doctorViewModel == null)
                    {
                        // If the user was not already a doctor, create a new DoctorViewModel and add it to the table
                        doctorViewModel = new DoctorViewModel
                        {
                            Id = userViewModel.UserId,
                            Email = userViewModel.Email,
                            FirstName = userViewModel.FirstName,
                            LastName = userViewModel.LastName,
                            PhoneNumber = userViewModel.PhoneNumber,
                            Country = userViewModel.Country,
                            City = userViewModel.City,
                            ZipCode = userViewModel.ZipCode,
                        };

                        _context.Doctors.Add(doctorViewModel);
                    }
                    else
                    {
                        // If the user was already a doctor, update the existing DoctorViewModel
                        // with any changes to the user's data
                        doctorViewModel.FirstName = userViewModel.FirstName;
                        doctorViewModel.LastName = userViewModel.LastName;
                        doctorViewModel.PhoneNumber = userViewModel.PhoneNumber;
                        doctorViewModel.Country = userViewModel.Country;
                        doctorViewModel.City = userViewModel.City;
                        doctorViewModel.ZipCode = userViewModel.ZipCode;
                    }

                    // Remove the user from the Patient table, if they were previously a patient
                    if (patientViewModel != null)
                    {
                        _context.Patients.Remove(patientViewModel);
                    }

                }

                else if (Input.Role == "Patient")
                {
                    var doctorViewModel = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == userViewModel.UserId);
                    var patientViewModel = await _context.Patients.FirstOrDefaultAsync(p => p.Id == userViewModel.UserId);

                    if (patientViewModel == null)
                    {
                        // If the user was not already a patient, create a new PatientViewModel and add it to the table
                        patientViewModel = new PatientViewModel
                        {
                            Id = userViewModel.UserId,
                            Email = userViewModel.Email,
                            FirstName = userViewModel.FirstName,
                            LastName = userViewModel.LastName,
                            PhoneNumber = userViewModel.PhoneNumber,
                            Country = userViewModel.Country,
                            City = userViewModel.City,
                            ZipCode = userViewModel.ZipCode,
                        };

                        _context.Patients.Add(patientViewModel);
                    }
                    else
                    {
                        // If the user was already a patient, update the existing PatientViewModel
                        // with any changes to the user's data
                        patientViewModel.FirstName = userViewModel.FirstName;
                        patientViewModel.LastName = userViewModel.LastName;
                        patientViewModel.PhoneNumber = userViewModel.PhoneNumber;
                        patientViewModel.Country = userViewModel.Country;
                        patientViewModel.City = userViewModel.City;
                        patientViewModel.ZipCode = userViewModel.ZipCode;
                    }
                    // Remove the user from the Doctor table, if they were previously a doctor
                    if (doctorViewModel != null)
                    {
                        _context.Doctors.Remove(doctorViewModel);
                    }
                }

              _context.Users.Update(userViewModel);
              await _context.SaveChangesAsync();

            }
            return RedirectToPage("/Account/ManageUsers", new { area = "Identity" });

        }
    }
}
