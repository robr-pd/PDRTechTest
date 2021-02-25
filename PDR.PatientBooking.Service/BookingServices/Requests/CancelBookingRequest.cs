using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.BookingServices.Requests
{
    public class CancelBookingRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}