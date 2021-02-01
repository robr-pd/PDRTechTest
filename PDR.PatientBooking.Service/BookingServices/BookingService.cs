﻿using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBokingRequestValidator _validator;

        public BookingService(PatientBookingContext context, IAddBokingRequestValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        /// <summary>
        ///  validate and add booking to the db
        /// </summary>
        /// <param name="request"></param>
        public void AddPatientBooking(NewBookingRequest request)
        {
            var validationResult = _validator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var bookingId = new Guid();
            var bookingStartTime = request.StartTime;
            var bookingEndTime = request.EndTime;
            var bookingPatientId = request.PatientId;
            var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == request.PatientId);
            var bookingDoctorId = request.DoctorId;
            var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == request.DoctorId);
            var bookingSurgeryType = _context.Patient.FirstOrDefault(x => x.Id == bookingPatientId).Clinic.SurgeryType;

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

            _context.Order.AddRange(new List<Order> { myBooking });
            _context.SaveChanges();
        }

        /// <summary>
        /// method to cancel booking
        /// </summary>
        /// <param name="bookingId"></param>
        public void CancelPatientBooking(Guid bookingId)
        {
            var existingBooking = _context.Order.FirstOrDefault(f => f.Id == bookingId);
            if (existingBooking != null && existingBooking.PatientId > 0)
            {
                // no object order did not find order so had to remove found booking against given identifier
                _context.Remove(existingBooking);
                _context.SaveChanges();
            }
        }
    }
}