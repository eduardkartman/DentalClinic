using Microsoft.AspNetCore.Identity;

namespace DentalClinicWeb.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData] 
        public string? FirstName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }
        [PersonalData]
        public string? PhoneNumber { get;set; }
        [PersonalData]
        public string? Country { get; set; }
        [PersonalData]
        public string? City { get; set; }
        [PersonalData]
        public string? ZipCode { get; set; }

    }
}
