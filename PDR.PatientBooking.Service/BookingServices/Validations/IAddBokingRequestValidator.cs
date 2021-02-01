using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices.Validations
{
    public interface IAddBokingRequestValidator
    {
        PdrValidationResult ValidateRequest(NewBookingRequest request);
    }
}