using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.ClinicServices;
using PDR.PatientBooking.Service.ClinicServices.Requests;
using PDR.PatientBooking.Service.ClinicServices.Validation;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.Tests.BookingServices
{
    [TestFixture]
    public class BookingServiceTests
    {
        private MockRepository _mockRepository;
        private IFixture _fixture;

        private PatientBookingContext _context;
        private Mock<IAddBookingRequestValidator> _validator;

        private BookingService _bookingService;
        private Mock<ICancelBookingRequestValidator> _cancelledBookingValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fixture = new Fixture();

            //Prevent fixture from generating circular references
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _validator = _mockRepository.Create<IAddBookingRequestValidator>();
            _cancelledBookingValidator = _mockRepository.Create<ICancelBookingRequestValidator>();

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _bookingService = new BookingService(
                _context,
                _validator.Object,
                _cancelledBookingValidator.Object
            );
        }

        private void SetupMockDefaults()
        {
            _validator.Setup(x => x.ValidateRequest(It.IsAny<AddBookingRequest>()))
                .Returns(new PdrValidationResult(true));
            
            _cancelledBookingValidator.Setup(x => x.ValidateRequest(It.IsAny<CancelBookingRequest>()))
                .Returns(new PdrValidationResult(true));
        }
        
        [Test]
        public void CancelBooking_CancelsBookingToContextWithGeneratedId()
        {
            //arrange
            var orderId = Guid.NewGuid();
            var request = _fixture.Create<CancelBookingRequest>();
            request.Id = orderId;

            var order = new Order
            {
                Id = orderId
            };

            _context.Order.Add(order);
            _context.SaveChanges();
            
            var expected = new Order 
            {
                Id = orderId,
                Status = Status.Cancelled
            };

            //act
            _bookingService.CancelBooking(request);

            //assert
            _context.Order.Should().ContainEquivalentOf(expected);
        }
        
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }
    }
}