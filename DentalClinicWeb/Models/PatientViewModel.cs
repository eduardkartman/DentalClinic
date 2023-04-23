using System.ComponentModel.DataAnnotations;

namespace DentalClinicWeb.Models
{
    public class PatientViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }

        public int DiscountThreshold { get; set; }

        public decimal DiscountPercentage { get; set; }

    }
}
