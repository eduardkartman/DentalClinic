using DentalClinicWeb.Data;
using Microsoft.AspNetCore.Identity;

namespace DentalClinicWeb.Models
{
    public class NotificationsViewModel
    {
        public SignInManager<ApplicationUser> SignInManager { get; set; }
        public NotificationModel Notifications { get; set; }

        public NotificationsViewModel()
        {
            Notifications = new NotificationModel();
        }


    }

}
