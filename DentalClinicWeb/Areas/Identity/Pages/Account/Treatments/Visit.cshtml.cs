using DentalClinicWeb.Data;
using DentalClinicWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Treatments
{
    public class VisitModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public IList<TreatmentsViewModel> Visits { get; set; }

        public VisitModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            Visits = new List<TreatmentsViewModel>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get all visits from the database and populate the Visits property
            Visits = await _context.Treatments.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Find the visit with the specified ID in the database
            var visit = await _context.Treatments.FindAsync(id);

            if (visit == null)
            {
                return NotFound();
            }

            // Remove the visit from the database and save changes
            _context.Treatments.Remove(visit);
            await _context.SaveChangesAsync();

            return RedirectToPage("Visit");
        }

        public IActionResult OnPostAdd()
        {
            // Redirect to the AddVisit page
            return RedirectToPage("AddVisit");
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            // Find the visit with the specified ID in the database
            var visit = await _context.Treatments.FindAsync(id);

            if (visit == null)
            {
                return NotFound();
            }

            // Set the VisitViewModel property of the EditVisit page to the visit we found
            TempData["VisitViewModel"] = visit;

            // Redirect to the EditVisit page
            return RedirectToPage("EditVisit");
        }
    }

}
