using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.PatientServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PDR.PatientBooking.Service.PatientServices.Validation
{
    public class AddPatientRequestValidator : IAddPatientRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddPatientRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddPatientRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (PatientAlreadyInDb(request, ref result))
                return result;

            if (ClinicNotFound(request, ref result))
                return result;

            return result;
        }

        private bool MissingRequiredFields(AddPatientRequest request, ref PdrValidationResult result)
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

        private bool PatientAlreadyInDb(AddPatientRequest request, ref PdrValidationResult result)
        {
            if (_context.Patient.Any(x => x.Email == request.Email))
            {
                result.PassedValidation = false;
                result.Errors.Add("A patient with that email address already exists");
                return true;
            }

            return false;
        }

        private bool ClinicNotFound(AddPatientRequest request, ref PdrValidationResult result)
        {
            if (!_context.Clinic.Any(x => x.Id == request.ClinicId))
            {
                result.PassedValidation = false;
                result.Errors.Add("A clinic with that ID could not be found");
                return true;
            }

            return false;
        }
    }
}
