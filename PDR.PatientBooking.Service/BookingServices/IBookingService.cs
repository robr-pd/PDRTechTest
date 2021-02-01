using PDR.PatientBooking.Service.BookingServices.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        void AddPatientBooking(NewBookingRequest request);
    }
}