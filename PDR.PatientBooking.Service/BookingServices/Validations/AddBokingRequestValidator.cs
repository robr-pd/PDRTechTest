using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices.Validations
{
    public class AddBokingRequestValidator : IAddBokingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddBokingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(NewBookingRequest request)
        {
            var result = new PdrValidationResult(true);
            ValidateBookingParameters(request, ref result);
            BookingAlreadyInDB(request, ref result);
            return result;
        }

        /// <summary>
        /// Validate request object parameters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ValidateBookingParameters(NewBookingRequest request, ref PdrValidationResult result)
        {
            TimeSpan ts = request.StartTime.Subtract(DateTime.UtcNow);

            if (ts.TotalMinutes < 0)
            {
                result.PassedValidation = false;
                result.Errors.Add("Booking Can not made in Past");
                return true;
            }
            return false;
        }

        /// <summary>
        /// validates if booking already exists in db for same patient for same time
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool BookingAlreadyInDB(NewBookingRequest request, ref PdrValidationResult result)
        {
            if (_context.Order.Any(x => x.PatientId == request.PatientId && x.StartTime >= request.StartTime && x.EndTime <= request.EndTime))
            {
                result.PassedValidation = false;
                result.Errors.Add("Booking already exists for this patient");
                return true;
            }

            return false;
        }
    }
}