/*
using DentalClinicWeb.Data;

namespace DentalClinicWeb.Models
{
    public class smsSender
    {
        private readonly ApplicationDbContext _context;
        public smsSender(ApplicationDbContext context)
        {
            _context= context;
        }
        private Timer _timer;

        public void StartSending()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    // Retrieve unsent SMS records from the database
                    var unsentSmsRecords = _context.SMS.Where(sms => !sms.IsSent).ToList();

                    foreach (var smsRecord in unsentSmsRecords)
                    {
                        bool isSent = false;

                        // Send the SMS using the sendSMS method
                        try
                        {
                            isSent = SendSMS.sendSMS(smsRecord.PhoneNumber, smsRecord.Message);
                        }
                        catch (Exception ex)
                        {
                            // Handle any exceptions that occur during SMS sending
                            Console.WriteLine($"Error sending SMS: {ex.Message}");
                            // You may want to log the exception or take other appropriate actions
                        }

                        if (isSent)
                        {
                            // Update the IsSent field to true if the SMS was sent successfully
                            smsRecord.IsSent = true;
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Delay for 10 seconds before sending the next batch of SMS messages
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            });
        }


        public void StopSending()
        {
            // Stop the timer and release its resources
            _timer?.Dispose();
        }
    }
}


*/