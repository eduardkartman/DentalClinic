using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Treatments
{
    [Authorize(Roles = "Admin")]
    public class EditVisitsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditVisitsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "The Treatment Name field is required.")]
            [Display(Name = "Treatment Name")]
            public string Name { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }

            [Display(Name = "Price")]
            public decimal? Price { get; set; }

            [Display(Name = "Estimated Time")]
            public int? DurationInMinutes { get; set; }

            public bool IsAvailable { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        { 
            var visit = await _context.Treatments.FindAsync(id);
            if (visit == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = visit.Id,
                Name = visit.Name,
                Description = visit.Description,
                Price = visit.Price,
                DurationInMinutes = visit.DurationInMinutes,
                IsAvailable = visit.IsAvailable,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var visit = await _context.Treatments.FindAsync(Input.Id);
            if (visit == null)
            {
                return NotFound();
            }

            visit.Name = Input.Name;
            visit.Description = Input.Description;
            visit.Price = Input.Price;
            visit.DurationInMinutes = Input.DurationInMinutes;
            visit.IsAvailable = Input.IsAvailable;

            _context.Treatments.Update(visit);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Treatments/Visit", new { area = "Identity" });

        }
    }
}
