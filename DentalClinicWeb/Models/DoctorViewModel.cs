﻿using System.ComponentModel.DataAnnotations;

namespace DentalClinicWeb.Models
{
    public class DoctorViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public int? UnreadNotifications { get; set; }
        public string? ImageUrl { get; set; }

    }
}
