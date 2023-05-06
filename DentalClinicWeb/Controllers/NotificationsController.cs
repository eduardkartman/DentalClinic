using DentalClinicWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinicWeb.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
        { 
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetAllRead()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var notifications = _context.Notifications
                .Where(n => n.ReceiverId == userId && n.IsRead == false)
                .ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
