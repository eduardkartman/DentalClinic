using System.ComponentModel.DataAnnotations;

namespace DentalClinicWeb.Models
{
    public class SMS
    {
        [Key]
        public int Id { get;set; }
        public string PhoneNumber{ get; set; }
        public string Message { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
