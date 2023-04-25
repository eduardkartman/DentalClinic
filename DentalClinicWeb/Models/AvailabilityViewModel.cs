namespace DentalClinicWeb.Models
{
/*
    public class AvailabilityViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string DoctorId { get; set; }
        public DoctorViewModel Doctor { get; set; }

        public ICollection<AppointmentViewModel> Appointments { get; set; }

        public ICollection<TreatmentsViewModel> Treatments { get; set; }

        public int AvailableSlots
        {
            get
            {
                int takenSlots = Appointments.Sum(a => a.DurationInMinutes / 60);
                return 5 - takenSlots;
            }
        }

        public bool IsAvailableForTreatment(int treatmentId)
        {
            return Treatments.Any(t => t.Id == treatmentId && t.IsAvailable);
        }

        public bool IsAvailableAt(DateTime appointmentTime)
        {
            // Check if appointment time is within the working hours of the doctor
            if (appointmentTime.TimeOfDay < Doctor.StartTime || appointmentTime.TimeOfDay > Doctor.EndTime)
            {
                return false;
            }

            // Check if appointment time is already taken
            foreach (var appointment in Appointments)
            {
                if (appointmentTime >= appointment.AppointmentDateTime &&
                    appointmentTime < appointment.AppointmentDateTime.AddMinutes(appointment.DurationInMinutes))
                {
                    return false;
                }
            }

            return true;
        }
    }

    */

}
