using Microsoft.AspNetCore.Mvc;
using System;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("patient/{identificationNumber}/next")]
        public IActionResult GetPatientNextAppointment(long identificationNumber)
        {
            try
            {
                return Ok(_bookingService.GetPatientNextAppointment(new GetPatientNextAppointmentRequest
                {
                    PatientId = identificationNumber
                }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public IActionResult AddBooking(AddBookingRequest request)
        {
            try
            {
                _bookingService.Add(request);
                
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut]
        public IActionResult CancelBooking(CancelBookingRequest request)
        {
            try
            {
                _bookingService.CancelBooking(request);
                
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}