using System;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using PDR.PatientBooking.Service.BookingServices.Validation;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _bookingRequestValidator;
        private readonly ICancelBookingRequestValidator _cancelBookingRequestValidator;
        private readonly IGetPatientNextAppointmentValidator _patientNextAppointmentValidator;

        public BookingService(
            PatientBookingContext context, 
            IAddBookingRequestValidator bookingRequestValidator, 
            ICancelBookingRequestValidator cancelBookingRequestValidator, 
            IGetPatientNextAppointmentValidator patientNextAppointmentValidator)
        {
            _context = context;
            _bookingRequestValidator = bookingRequestValidator;
            _cancelBookingRequestValidator = cancelBookingRequestValidator;
            _patientNextAppointmentValidator = patientNextAppointmentValidator;
        }

        public PatientNextAppointmentResponse GetPatientNextAppointment(GetPatientNextAppointmentRequest request)
        {
            var validationResult = _patientNextAppointmentValidator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }
            
            var booking = _context.Order
                .Where(x => x.PatientId == request.PatientId)
                .Where(x => x.Status == Status.Active)
                .Where(x => x.StartTime > request.CurrentDateTime)
                .OrderBy(x => x.StartTime)
                .First();

            return new PatientNextAppointmentResponse
            {
                Id = booking.Id,
                DoctorId = booking.DoctorId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };
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
    }
}