using PDR.PatientBooking.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.DoctorServices.Requests
{
    public class AddDoctorRequest
    {
        [Required(ErrorMessage = "FirstName must be populated")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "LastName must be populated")]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        
        [Required(ErrorMessage = "Email must be populated")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string Email { get; set; }
    }
}
