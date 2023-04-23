using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Treatments
{
    [Authorize(Roles = "Admin")]
    public class CreateVisitModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public TreatmentsViewModel Treatments { get; set; }

        public CreateVisitModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var visit = new TreatmentsViewModel
            {
                Name = Treatments.Name,
                Description = Treatments.Description,
                Price = Treatments.Price,
                DurationInMinutes = Treatments.DurationInMinutes,
                IsAvailable = Treatments.IsAvailable,
            };

            _context.Treatments.Add(visit);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Treatments/Visit", new { area = "Identity" });
        }
    }
}
