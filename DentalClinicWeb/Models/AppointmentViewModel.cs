using DentalClinicWeb.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DentalClinicWeb.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        public string PatientId { get; set; }
        public PatientViewModel Patient { get; set; }

        public string DoctorId { get; set; }
        public DoctorViewModel Doctor { get; set; }

        public int TreatmentId { get; set; }
        public TreatmentsViewModel Treatment { get; set; }

        public decimal Price { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public int AvailabilityId { get; set; }
        //public AvailabilityViewModel Availability { get; set; }

        public AppointmentStatus Status { get; set; }

        public bool HasDiscount => Patient != null && Patient.DiscountThreshold >= 2;

        public decimal Discount => HasDiscount ? Price * 0.2M : 0M;
    }


}
