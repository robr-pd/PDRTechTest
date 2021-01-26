using Microsoft.AspNetCore.Mvc;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly PatientBookingContext _context;

        public BookingController(PatientBookingContext context)
        {
            _context = context;
        }

        [HttpGet("patient/{identificationNumber}/next")]
        public IActionResult GetPatientNextAppointment(long identificationNumber)
        {
            var nextBooking = _context.Order
                                .Where(o => o.PatientId == identificationNumber
                                        && o.StartTime > DateTime.UtcNow)
                                .OrderBy(o => o.StartTime)
                                .FirstOrDefault();

            if (nextBooking is null)
            {
                return Ok();
            }

            return Ok(new
            {
                nextBooking.Id,
                nextBooking.DoctorId,
                nextBooking.StartTime,
                nextBooking.EndTime
            });
        }

        [HttpPost()]
        public IActionResult AddBooking(NewBooking newBooking)
        {
            // really needs refactoring but for the sake of the exercise I will leave it
            var bookingId = new Guid();
            var bookingStartTime = newBooking.StartTime; 
            var bookingEndTime = newBooking.EndTime;
            
            if (bookingStartTime > bookingEndTime)
            {
                return StatusCode(400, "Booking StartTime must be earlier then EndTime");
            }

            if (bookingStartTime.CompareTo(DateTime.UtcNow) < 0)
            {
                return StatusCode(400, "A Booking StartTime cannot be in the past.");
            }

            var bookingPatientId = newBooking.PatientId;
            var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == newBooking.PatientId);
            
            if (bookingPatient is null)
            {
                return StatusCode(400, "Patient doesn't exist with this Id");
            }

            var bookingDoctorId = newBooking.DoctorId;
            var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == newBooking.DoctorId);
            
            if (bookingDoctor is null)
            {
                return StatusCode(400, "Doctor doesn't exist with this Id");
            }

            var bookingSurgeryType = _context.Patient.FirstOrDefault(x => x.Id == bookingPatientId).Clinic.SurgeryType;       
            var bookingExists = bookingDoctor.Orders.Any(x => x.StartTime <= bookingEndTime && x.EndTime >= bookingStartTime);            
            
            if (bookingExists)
            {
                return StatusCode(400, "The requested appointment time is already taken."); 
            }

            var myBooking = new Order
            {
                Id = bookingId,
                StartTime = bookingStartTime,
                EndTime = bookingEndTime,
                PatientId = bookingPatientId,
                DoctorId = bookingDoctorId,
                Patient = bookingPatient,
                Doctor = bookingDoctor,
                SurgeryType = (int)bookingSurgeryType
            };

            _context.Order.Add(myBooking);
            _context.SaveChanges();

            return StatusCode(200);
        }

        public class NewBooking
        {
            public Guid Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public long PatientId { get; set; }
            public long DoctorId { get; set; }
        }

        private static MyOrderResult UpdateLatestBooking(List<Order> bookings2, int i)
        {
            MyOrderResult latestBooking;
            latestBooking = new MyOrderResult();
            latestBooking.Id = bookings2[i].Id;
            latestBooking.DoctorId = bookings2[i].DoctorId;
            latestBooking.StartTime = bookings2[i].StartTime;
            latestBooking.EndTime = bookings2[i].EndTime;
            latestBooking.PatientId = bookings2[i].PatientId;
            latestBooking.SurgeryType = (int)bookings2[i].GetSurgeryType();

            return latestBooking;
        }

        private class MyOrderResult
        {
            public Guid Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public long PatientId { get; set; }
            public long DoctorId { get; set; }
            public int SurgeryType { get; set; }
        }
    }
}