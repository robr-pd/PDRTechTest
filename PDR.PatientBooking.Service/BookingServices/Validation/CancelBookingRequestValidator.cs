using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class CancelBookingRequestValidator : ICancelBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public CancelBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }
        
        public PdrValidationResult ValidateRequest(CancelBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (BookingNotExistsInDb(request, ref result))
                return result;
            
            if (BookingAlreadyCanceled(request, ref result))
                return result;
            
            return result;
        }
        
        private bool BookingNotExistsInDb(CancelBookingRequest request, ref PdrValidationResult result)
        {
            if (!_context.Order.Any(x => x.Id == request.Id))
            {
                result.PassedValidation = false;
                result.Errors.Add($"A booking with Id {request.Id} does not exist");
                return true;
            }

            return false;
        }
        
        private bool BookingAlreadyCanceled(CancelBookingRequest request, ref PdrValidationResult result)
        {
            if (_context.Order.Any(x => x.Id == request.Id && x.Status == Status.Cancelled))
            {
                result.PassedValidation = false;
                result.Errors.Add($"A booking with Id {request.Id} already cancelled");
                return true;
            }

            return false;
        }
        
        private bool MissingRequiredFields(CancelBookingRequest request, ref PdrValidationResult result)
        {
            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);
            
            Validator.TryValidateObject(request, validationContext, results, true);
            
            if (!results.Any())
            {
                return false;
            }
            
            result.PassedValidation = false;
            result.Errors.AddRange(results.Select(x => x.ErrorMessage));
            
            return true;
        }
    }
}