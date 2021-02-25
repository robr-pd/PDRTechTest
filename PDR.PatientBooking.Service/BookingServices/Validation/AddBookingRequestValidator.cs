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
    public class AddBookingRequestValidator : IAddBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }
        
        public PdrValidationResult ValidateRequest(AddBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (DoctorNotExistsInDb(request, ref result))
                return result;
            
            if (PatientNotExistsInDb(request, ref result))
                return result;

            if (DoctorIsNotAvailable(request, ref result))
                return result;
            
            return result;
        }
        
        private bool MissingRequiredFields(AddBookingRequest request, ref PdrValidationResult result)
        {
            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);
            
            Validator.TryValidateObject(request, validationContext, results, true);

            if (request.StartTime < DateTime.UtcNow)
            {
                results.Add(new ValidationResult("Appointment start date cannot be in the past"));
            }

            if (request.StartTime > request.EndTime)
            {
                results.Add(new ValidationResult("Appointment end date cannot be lower than start date"));
            }
            
            if (!results.Any())
            {
                return false;
            }
            
            result.PassedValidation = false;
            result.Errors.AddRange(results.Select(x => x.ErrorMessage));
            
            return true;
        }

        private bool DoctorNotExistsInDb(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (!_context.Doctor.Any(x => x.Id == request.DoctorId))
            {
                result.PassedValidation = false;
                result.Errors.Add($"A doctor with Id {request.DoctorId} does not exist");
                return true;
            }

            return false;
        }
        
        private bool PatientNotExistsInDb(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (!_context.Patient.Any(patient => patient.Id == request.PatientId))
            {
                result.PassedValidation = false;
                result.Errors.Add($"A patient with Id {request.PatientId} does not exist");
                return true;
            }

            return false;
        }

        private bool DoctorIsNotAvailable(AddBookingRequest request, ref PdrValidationResult result)
        {
            var doctorNotAvailable = _context.Order
                .Any(order => order.DoctorId == request.DoctorId && order.Status == Status.Active
                              && request.StartTime < order.EndTime && order.StartTime < request.EndTime);

            if (doctorNotAvailable)
            {
                result.PassedValidation = false;
                result.Errors.Add($"A doctor is not available for this period of time");
                return true;
            }

            return false;

        }
    }
}