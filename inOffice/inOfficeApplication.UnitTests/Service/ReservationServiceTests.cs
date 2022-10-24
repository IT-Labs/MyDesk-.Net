using AutoMapper;
using inOffice.BusinessLogicLayer;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.Requests;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class ReservationServiceTests
    {
        private IReservationService _reservationService;
        private IReservationRepository _reservationRepository;
        private IEmployeeRepository _employeeRepository;
        private IDeskRepository _deskRepository;
        private IMapper _mapper;

        [OneTimeSetUp]
        public void Setup()
        {
            _reservationRepository = Substitute.For<IReservationRepository>();
            _employeeRepository = Substitute.For<IEmployeeRepository>();
            _deskRepository = Substitute.For<IDeskRepository>();
            _mapper = Substitute.For<IMapper>();

            _reservationService = new ReservationService(_reservationRepository, _employeeRepository, _deskRepository, _mapper);
        }

        [Test]
        [Order(1)]
        public void CancelReservation_Success()
        {
            // Arrange
            Reservation reservation = new Reservation()
            {
                Id = 1,
                Reviews = new List<Review>()
                {
                    new Review() { IsDeleted = false },
                    new Review() { IsDeleted = false },
                    new Review() { IsDeleted = false }
                }
            };

            _reservationRepository.Get(reservation.Id, includeReviews: true).Returns(reservation);

            // Act
            _reservationService.CancelReservation(reservation.Id);

            // Assert
            _reservationRepository.Received(1).SoftDelete(Arg.Is<Reservation>(x => x.Reviews.All(y => y.IsDeleted == true)));
        }

        [Test]
        [Order(2)]
        public void CancelReservation_ThrowsNotFoundException()
        {
            // Arrange
            int id = 13;

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reservationService.CancelReservation(id));
            Assert.IsTrue(exception.Message == $"Reservation with ID: {id} not found.");
        }

        [TestCase(null, null, null)]
        [TestCase("test@it-labs.com", 1, 1)]
        [Order(3)]
        public void FutureReservations_Success(string employeeEmail, int? take, int? skip)
        {
            // Arrange
            int? totalCount = 1;
            Tuple<int?, List<Reservation>> reservations = Tuple.Create(totalCount, new List<Reservation>()
            {
                new Reservation()
                {
                    Id = 2,
                    StartDate = DateTime.Now.AddDays(1)
                },
                new Reservation()
                {
                    Id = 3,
                    StartDate = DateTime.Now.AddDays(-1)
                }
            });

            if (!string.IsNullOrEmpty(employeeEmail))
            {
                Employee employee = new Employee() { Id = 11, Email = employeeEmail };
                _employeeRepository.GetByEmail(employee.Email).Returns(employee);
                _reservationRepository.GetFutureReservations(employee.Id, includeDesk: true, includeConferenceRoom: true, includeOffice: true, take: take, skip: skip).Returns(reservations);
            }
            else
            {
                _reservationRepository.GetFutureReservations(null, includeDesk: true, includeConferenceRoom: true, includeOffice: true, take: take, skip: skip).Returns(reservations);
            }

            List<ReservationDto> reservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 2 } };
            _mapper.Map<List<ReservationDto>>(Arg.Is<List<Reservation>>(x => x.Count == 2)).Returns(reservationDtos);

            // Act
            PaginationDto<ReservationDto> result = _reservationService.FutureReservations(employeeEmail, take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.TotalCount == 1);
            Assert.IsTrue(result.Values.Count == 1);
        }

        [Test]
        [Order(4)]
        public void EmployeeReservations_ThrowsNotFoundException()
        {
            // Arrange
            string email = "test email";

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reservationService.FutureReservations(email));
            Assert.IsTrue(exception.Message == $"Employee with email: {email} not found.");
        }

        [TestCase(null, null, null)]
        [TestCase("test@it-labs.com", 1, 1)]
        [Order(5)]
        public void PastReservations_Success(string employeeEmail, int? take, int? skip)
        {
            // Arrange
            int? totalCount = 1;
            Tuple<int?, List<Reservation>> reservations = Tuple.Create(totalCount, new List<Reservation>()
            {
                new Reservation()
                {
                    Id = 5,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                new Reservation()
                {
                    Id = 6,
                    StartDate = DateTime.Now.AddDays(-2),
                    EndDate = DateTime.Now.AddDays(-1)
                }
            });

            if (!string.IsNullOrEmpty(employeeEmail))
            {
                Employee employee = new Employee() { Id = 4, Email = employeeEmail };
                _employeeRepository.GetByEmail(employee.Email).Returns(employee);
                _reservationRepository.GetPastReservations(employee.Id, includeDesk: true, includeConferenceRoom: true, includeOffice: true, includeReviews: true, take: take, skip: skip).Returns(reservations);
            }
            else
            {
                _reservationRepository.GetPastReservations(null, includeDesk: true, includeConferenceRoom: true, includeOffice: true, includeReviews: true, take: take, skip: skip).Returns(reservations);
            }

            List<ReservationDto> reservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 6 } };
            _mapper.Map<List<ReservationDto>>(Arg.Any<List<Reservation>>()).Returns(reservationDtos);

            // Act
            PaginationDto<ReservationDto> result = _reservationService.PastReservations(employeeEmail, take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.TotalCount == 1);
            Assert.IsTrue(result.Values.Count == 1);
        }

        [Test]
        [Order(6)]
        public void PastReservations_ThrowsNotFoundException()
        {
            // Arrange
            string email = "test email";

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reservationService.PastReservations(email));
            Assert.IsTrue(exception.Message == $"Employee with email: {email} not found.");
        }

        [Test]
        [Order(7)]
        public void CoworkerReserve_Success()
        {
            // Arrange
            ReservationRequest request = new ReservationRequest()
            {
                DeskId = 14,
                EmployeeEmail = "new test email",
                StartDate = DateTime.Now.ToString("dd-MM-yyyy"),
                EndDate = DateTime.Now.AddDays(2).ToString("dd-MM-yyyy")
            };
            Employee employee = new Employee() { Id = 5, Email = request.EmployeeEmail };

            _deskRepository.Get(request.DeskId).Returns(new Desk() { Id = request.DeskId, OfficeId = 1 });
            _employeeRepository.GetByEmail(request.EmployeeEmail).Returns(employee);
            _reservationRepository.GetDeskReservations(request.DeskId).Returns(new List<Reservation>()
            {
                new Reservation()
                {
                    StartDate = DateTime.Now.AddDays(-4),
                    EndDate = DateTime.Now.AddDays(-1)
                }
            });
            _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeConferenceRoom: true).Returns(new List<Reservation>()
            {
                new Reservation()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    ConferenceRoom = new ConferenceRoom()
                    {
                        OfficeId = 2
                    }
                }
            });

            // Act
            _reservationService.CoworkerReserve(request);

            // Assert
            _reservationRepository.Received(1).Insert(Arg.Is<Reservation>(x => x.DeskId == request.DeskId && 
                x.EmployeeId == employee.Id && x.StartDate.ToString("dd-MM-yyyy") == request.StartDate && x.EndDate.ToString("dd-MM-yyyy") == request.EndDate));
        }

        [Test]
        [Order(8)]
        public void CoworkerReserve_WrongDeskId_ThrowsNotFoundException()
        {
            // Arrange
            ReservationRequest request = new ReservationRequest() { DeskId = 11 };

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reservationService.CoworkerReserve(request));
            Assert.IsTrue(exception.Message == $"Desk with ID: {request.DeskId} not found.");
        }

        [Test]
        [Order(9)]
        public void CoworkerReserve_WrongEmployeeEmail_ThrowsNotFoundException()
        {
            // Arrange
            ReservationRequest request = new ReservationRequest() { DeskId = 12, EmployeeEmail = "test email" };
            _deskRepository.Get(request.DeskId).Returns(new Desk() { Id = request.DeskId });

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reservationService.CoworkerReserve(request));
            Assert.IsTrue(exception.Message == $"Employee with email: {request.EmployeeEmail} not found.");
        }

        [Test]
        [Order(10)]
        public void CoworkerReserve_ReservationTime_ThrowsConflictException()
        {
            // Arrange
            ReservationRequest request = new ReservationRequest() 
            { 
                DeskId = 13, 
                EmployeeEmail = "email", 
                StartDate = DateTime.Now.ToString("dd-MM-yyyy"),
                EndDate = DateTime.Now.AddDays(2).ToString("dd-MM-yyyy")
            };

            _deskRepository.Get(request.DeskId).Returns(new Desk() { Id = request.DeskId });
            _employeeRepository.GetByEmail(request.EmployeeEmail).Returns(new Employee() { Id = 2, Email = request.EmployeeEmail });
            _reservationRepository.GetDeskReservations(request.DeskId).Returns(new List<Reservation>() 
            { 
                new Reservation() 
                { 
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now
                }
            });

            // Act + Assert
            ConflictException exception = Assert.Throws<ConflictException>(() => _reservationService.CoworkerReserve(request));
            Assert.IsTrue(exception.Message == $"Reservation for that time period already exists for desk {request.DeskId}");
        }

        [Test]
        [Order(11)]
        public void CoworkerReserve_DeskAlreadyReserved_ThrowsConflictException()
        {
            // Arrange
            ReservationRequest request = new ReservationRequest() 
            { 
                DeskId = 14, 
                EmployeeEmail = "new test email",
                StartDate = DateTime.Now.ToString("dd-MM-yyyy"),
                EndDate = DateTime.Now.AddDays(2).ToString("dd-MM-yyyy")
            };
            Employee employee = new Employee() { Id = 5, Email = request.EmployeeEmail };

            _deskRepository.Get(request.DeskId).Returns(new Desk() { Id = request.DeskId, OfficeId = 1 });
            _employeeRepository.GetByEmail(request.EmployeeEmail).Returns(employee);
            _reservationRepository.GetDeskReservations(request.DeskId).Returns(new List<Reservation>());
            _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeConferenceRoom: true).Returns(new List<Reservation>()
            {
                new Reservation()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    Desk = new Desk()
                    {
                        OfficeId = 1
                    }
                }
            });

            // Act + Assert
            ConflictException exception = Assert.Throws<ConflictException>(() => _reservationService.CoworkerReserve(request));
            Assert.IsTrue(exception.Message == $"Reservation for that time period already exists for {employee.Email}");
        }
    }
}
