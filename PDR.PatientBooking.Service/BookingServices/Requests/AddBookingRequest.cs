using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.BookingServices.Requests
{
    public class AddBookingRequest
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PatientId should be greater than or equal to 1")]
        public long PatientId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PatientId should be greater than or equal to 1")]
        public long DoctorId { get; set; }
    }
}