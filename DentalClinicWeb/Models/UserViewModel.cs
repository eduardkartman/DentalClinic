using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicWeb.Models
{
    public class UserViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserId { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string?  Country { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string Role { get; set; }
        public int? UnreadNotifications { get; set; }
    }

}
