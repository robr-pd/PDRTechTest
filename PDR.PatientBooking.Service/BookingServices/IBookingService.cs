using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        void CancelBooking(CancelBookingRequest request);
        void Add(AddBookingRequest request);
    }
}