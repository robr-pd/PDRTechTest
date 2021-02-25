using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class GetPatientNextAppointmentValidator : IGetPatientNextAppointmentValidator
    {
        private readonly PatientBookingContext _context;

        public GetPatientNextAppointmentValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(GetPatientNextAppointmentRequest request)
        {
            var result = new PdrValidationResult(true);
            
            if (MissingRequiredFields(request, ref result))
                return result;

            if (AppointmentsExistForPatient(request, ref result))
                return result;

            return result;
        }
        
        private bool MissingRequiredFields(GetPatientNextAppointmentRequest request, ref PdrValidationResult result)
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
        
        private bool AppointmentsExistForPatient(GetPatientNextAppointmentRequest request, ref PdrValidationResult result)
        {
            if (!_context.Order.Any(x => x.PatientId == request.PatientId && x.Status == Status.Active && x.StartTime > request.CurrentDateTime))
            {
                result.PassedValidation = false;
                result.Errors.Add($"There are no appointments for patient with id - {request.PatientId}");
                return true;
            }

            return false;
        }
    }
}