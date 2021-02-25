using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    [TestFixture]
    public class GetPatientNextAppointmentValidatorTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private GetPatientNextAppointmentValidator _getPatientNextAppointmentValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _getPatientNextAppointmentValidator = new GetPatientNextAppointmentValidator(
                _context
            );
        }

        private void SetupMockDefaults()
        {
        }
        
        [TestCase]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.PatientId = 1;

            var order = new Order
            {
                PatientId = request.PatientId,
                StartTime = DateTime.UtcNow.AddHours(1)
            };

            _context.Order.Add(order);
            _context.SaveChanges();
            
            //act
            var res = _getPatientNextAppointmentValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        [TestCase]
        public void ValidateRequest_WithNoAppointments_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.PatientId = 1;
            
            //act
            var res = _getPatientNextAppointmentValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain($"There are no appointments for patient with id - {request.PatientId}");
        }
        
        [TestCase]
        public void ValidateRequest_InvalidPatientId_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.PatientId = 0;
            
            //act
            var res = _getPatientNextAppointmentValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain($"PatientId should be greater than or equal to 1");
        }
        
        private GetPatientNextAppointmentRequest GetValidRequest()
        {
            var request = _fixture.Create<GetPatientNextAppointmentRequest>();
            return request;
        }
        
    }
}