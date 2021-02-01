using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices.Requests
{
    public class CancelBookingRequest
    {
        public Guid BookingIdentifier { get; set; }
    }
}