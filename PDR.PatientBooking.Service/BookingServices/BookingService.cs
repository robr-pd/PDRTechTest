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
        private readonly ICancelBookingRequestValidator _cancelBookingRequestValidator;

        public BookingService(
            PatientBookingContext context, 
            IAddBookingRequestValidator bookingRequestValidator, 
            ICancelBookingRequestValidator cancelBookingRequestValidator)
        {
            _context = context;
            _bookingRequestValidator = bookingRequestValidator;
            _cancelBookingRequestValidator = cancelBookingRequestValidator;
        }

        public void CancelBooking(CancelBookingRequest request)
        {
            var validationResult = _cancelBookingRequestValidator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var booking = _context.Order.Find(request.Id);
            booking.Status = Status.Cancelled;

            _context.Order.Update(booking);
            _context.SaveChanges();
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