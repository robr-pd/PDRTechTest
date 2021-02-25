using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        PatientNextAppointmentResponse GetPatientNextAppointment(GetPatientNextAppointmentRequest request);
        void Add(AddBookingRequest request);
        void CancelBooking(CancelBookingRequest request);
    }
}