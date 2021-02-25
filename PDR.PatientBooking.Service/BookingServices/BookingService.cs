using System;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _bookingRequestValidator;

        public BookingService(PatientBookingContext context, IAddBookingRequestValidator bookingRequestValidator)
        {
            _context = context;
            _bookingRequestValidator = bookingRequestValidator;
        }

        public void Add(AddBookingRequest request)
        {
            var validationResult = _bookingRequestValidator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }
            
            var patient = _context.Patient.Find(request.PatientId);
            var doctor = _context.Doctor.Find(request.DoctorId);
            
            var booking = new Order
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PatientId = patient.Id,
                Patient = patient,
                DoctorId = doctor.Id,
                Doctor = doctor,
                SurgeryType = (int)patient.Clinic.SurgeryType
            };

            _context.Order.Add(booking);
            _context.SaveChanges();
        }
    }
}