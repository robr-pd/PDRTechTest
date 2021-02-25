using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.BookingServices.Requests
{
    public class GetPatientNextAppointmentRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PatientId should be greater than or equal to 1")]
        public long PatientId { get; set; }

        public DateTime CurrentDateTime { get; } = DateTime.UtcNow;
    }
}