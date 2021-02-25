using System;
using System.Linq;
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
    public class AddBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private AddBookingRequestValidator _addBookingRequestValidator;

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
            _addBookingRequestValidator = new AddBookingRequestValidator(
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
            request.DoctorId = 1;
            request.PatientId = 1;
            request.StartTime = DateTime.UtcNow.AddMinutes(1);
            request.EndTime = DateTime.UtcNow.AddMinutes(2);
            
            SeedDatabase(request);
            
            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }
        
        
        [TestCase]
        public void ValidateRequest_InvalidStartDate_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddHours(-1);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Appointment start date cannot be in the past");
        }
        
        [TestCase]
        public void ValidateRequest_StartDateGreaterThanEndDate_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddHours(1);
            request.EndTime = DateTime.UtcNow;

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Appointment end date cannot be lower than start date");
        }
        
        [TestCase]
        public void ValidateRequest_ZeroPatientId_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.PatientId = 0;

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("PatientId should be greater than or equal to 1");
        }
        
        [TestCase]
        public void ValidateRequest_ZeroDoctorId_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.DoctorId = 0;

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("PatientId should be greater than or equal to 1");
        }

        [TestCase]
        public void ValidateRequest_OverlapAppointment_ReturnsFailedValidationResult()
        {
            var request = GetValidRequest();
            request.DoctorId = 1;
            request.PatientId = 1;
            
            request.StartTime = DateTime.UtcNow.AddHours(1);
            request.EndTime = DateTime.UtcNow.AddMinutes(90);

           SeedDatabase(request);

           var order = _context.Order.First(x => x.DoctorId == request.DoctorId);
           order.StartTime = DateTime.UtcNow.AddMinutes(50);
           order.EndTime = DateTime.UtcNow.AddMinutes(80);
           
           _context.Order.Update(order);
           _context.SaveChanges();
           
            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("A doctor is not available for this period of time");
        }

        [TestCase]
        public void ValidateRequest_NotOverlapAppointment_ReturnsPassedValidationResult()
        {
            var request = GetValidRequest();
            request.DoctorId = 1;
            request.PatientId = 1;
            
            request.StartTime = DateTime.UtcNow.AddHours(1);
            request.EndTime = DateTime.UtcNow.AddMinutes(90);
            
            SeedDatabase(request);
            
            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        private AddBookingRequest GetValidRequest()
        {
            var request = _fixture.Create<AddBookingRequest>();
            return request;
        }
        
        private void SeedDatabase(AddBookingRequest request)
        {
            var doctor = _fixture.Build<Doctor>()
                .With(x => x.Id, 1)
                .Create();

            var patient = _fixture.Build<Patient>()
                .With(x => x.Id, 1)
                .Create();


            var order = _fixture
                .Build<Order>()
                .With(x => x.DoctorId, request.DoctorId)
                .With(x => x.Doctor, doctor)
                .With(x => x.PatientId, request.PatientId)
                .With(x => x.Patient, patient)
                .With(x => x.StartTime, DateTime.UtcNow.AddMinutes(120))
                .With(x => x.EndTime, DateTime.UtcNow.AddMinutes(180))
                .Create();

            _context.Add(order);
            _context.Add(doctor);
            _context.Add(patient);

            _context.SaveChanges();
        }
        
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }
    }
}