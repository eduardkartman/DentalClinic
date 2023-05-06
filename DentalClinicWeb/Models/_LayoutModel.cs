using DentalClinicWeb.Data.Migrations;
using DentalClinicWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinicWeb.Models
{
    public class _LayoutModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public _LayoutModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private async Task<List<NotificationModel>> GetNotificationsForCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications =  _context.Notifications
                .Where(n => n.ReceiverId == user.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return notifications;
        }

        public async Task<IActionResult> OnGetNotifications()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            var notifications = await GetNotificationsForCurrentUser();
            return new JsonResult(new { notifications });
        }
    }

}
