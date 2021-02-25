using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.BookingServices.Requests
{
    public class GetPatientNextAppointmentRequest
    {
        [Required]
        public long PatientId { get; set; }

        public DateTime CurrentDateTime { get; } = DateTime.UtcNow;
    }
}