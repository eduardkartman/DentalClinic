using DentalClinicWeb.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicWeb.Models
{
    public class NotificationModel
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public UserViewModel Sender { get; set; }
        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public UserViewModel Receiver { get; set; }
        public int AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public AppointmentViewModel Appointment { get; set; }
    }

}
