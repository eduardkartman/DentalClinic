using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Evaluation;

namespace DentalClinicWeb.Models
{
    [Table("Appointments")]
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhoneNumber { get; set;}

        [ForeignKey("PatientId")]
        public PatientViewModel Patients { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set;}
        public string DoctorEmail { get; set;}
        public string DoctorPhoneNumber { get; set;}

        [ForeignKey("DoctorId")]
        public DoctorViewModel Doctors { get; set; }

        public int TreatmentId { get; set; }
        public string TreatmentName { get;set; }
        public decimal? TreatmentPrice { get; set; }
        public int? TreatmentDuration { get; set;}
        public bool TreatmentEnabled { get; set; }

        [ForeignKey("TreatmentId")]
        public TreatmentsViewModel Treatment { get; set; }

        public DateTime AppointmentDateTime { get; set; }
        public DateTime EndAppointmentDateTime { get; set; }

        public AppointmentStatus Status { get; set; }

    
    }
}
