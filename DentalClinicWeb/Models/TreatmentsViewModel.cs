using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicWeb.Models
{
    public class TreatmentsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int? DurationInMinutes { get; set; }

        public bool IsAvailable { get; set; }

    }


}
