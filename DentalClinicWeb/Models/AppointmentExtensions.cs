namespace DentalClinicWeb.Models
{
    public static class AppointmentExtensions
    {
        public static bool HasDiscount(this AppointmentViewModel appointment)
        {
            return appointment.Patient != null && appointment.Patient.DiscountThreshold >= 2;
        }

        public static decimal Discount(this AppointmentViewModel appointment)
        {
            return appointment.HasDiscount() ? appointment.Price * 0.2M : 0M;
        }

        public static bool IsAvailable(this AppointmentViewModel appointment)
        {
            return appointment.Status == AppointmentStatus.Pending;
        }

        public static bool IsDayFullyBooked(DateTime date, IEnumerable<AppointmentViewModel> appointments, int maxAppointmentsPerDay)
        {
            var appointmentsOnDay = appointments.Where(a => a.AppointmentDateTime.Date == date.Date);

            return appointmentsOnDay.Count() >= maxAppointmentsPerDay;
        }

        public static IEnumerable<DateTime> GetAvailableDates(IEnumerable<AppointmentViewModel> appointments, int maxAppointmentsPerDay, int daysAhead)
        {
            var today = DateTime.Now.Date;
            var endDate = today.AddDays(daysAhead);

            var availableDates = new List<DateTime>();

            for (DateTime date = today; date <= endDate; date = date.AddDays(1))
            {
                if (!IsDayFullyBooked(date, appointments, maxAppointmentsPerDay))
                {
                    availableDates.Add(date);
                }
            }

            return availableDates;
        }

        public static IEnumerable<DateTime> GetAvailableTimes(DateTime date, IEnumerable<AppointmentViewModel> appointments, int maxAppointmentsPerDay, TimeSpan start, TimeSpan end, TimeSpan slotDuration)
        {
            var appointmentsOnDay = appointments.Where(a => a.AppointmentDateTime.Date == date.Date);

            var slots = new List<DateTime>();

            var slotStart = date.Date.Add(start);
            var slotEnd = date.Date.Add(end);

            while (slotStart < slotEnd)
            {
                if (appointmentsOnDay.Where(a => a.AppointmentDateTime.TimeOfDay >= slotStart.TimeOfDay && a.AppointmentDateTime.TimeOfDay < slotStart.Add(slotDuration).TimeOfDay).Count() < maxAppointmentsPerDay)
                {
                    slots.Add(slotStart);
                }

                slotStart = slotStart.Add(slotDuration);
            }

            return slots;
        }
    }
}

