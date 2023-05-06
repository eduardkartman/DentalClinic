using DentalClinicWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicWeb.Models
{
    public class NotificationsHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SendNotification(string userId, string message)
        {
            var aspuser = await _userManager.FindByIdAsync(userId);
            aspuser.UnreadNotifications++;
            await _userManager.UpdateAsync(aspuser);

            var user = await _context.Users.FindAsync(userId);
            user.UnreadNotifications++;

            var notification = await _context.Notifications
               .Where(n => n.ReceiverId == user.UserId)
               .OrderByDescending(n => n.CreatedAt)
               .ToListAsync();

            await _context.SaveChangesAsync();

            // Get the updated count of unread notifications and the latest notification message
            var unreadNotifications = user.UnreadNotifications;
            var latestNotificationMessage =message;

            // Send the updated count and latest notification message to the client using SignalR
            await Clients.User(userId).SendAsync("ReceiveNotification", latestNotificationMessage, unreadNotifications);
        }
    }
}

