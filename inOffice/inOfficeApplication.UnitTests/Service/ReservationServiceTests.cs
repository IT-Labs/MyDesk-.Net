using AutoMapper;
using inOffice.BusinessLogicLayer.Implementation;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.Repository.Interface;
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

        [Test]
        [Order(3)]
        public void EmployeeReservations_Success()
        {
            // Arrange
            Employee employee = new Employee() { Id = 11, Email = "test@it-labs.com" };
            List<Reservation> reservations = new List<Reservation>()
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
            };

            List<ReservationDto> reservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 2 } };

            _employeeRepository.GetByEmail(employee.Email).Returns(employee);
            _reservationRepository.GetEmployeeFutureReservations(employee.Id, includeDesk: true, includeConferenceRoom: true, includeOffice: true).Returns(reservations);
            _mapper.Map<List<ReservationDto>>(Arg.Is<List<Reservation>>(x => x.Count == 2)).Returns(reservationDtos);

            // Act
            List<ReservationDto> result = _reservationService.FutureReservations(employee.Email);

            // Assert
            Assert.IsTrue(result.Count == 1);
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

        [Test]
        [Order(5)]
        public void PastReservations_Success()
        {
            // Arrange
            Employee employee = new Employee() { Id = 4, Email = "test@it-labs.com" };
            List<Reservation> reservations = new List<Reservation>()
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
            };

            List<ReservationDto> reservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 6 } };

            _employeeRepository.GetByEmail(employee.Email).Returns(employee);
            _reservationRepository.GetEmployeePastReservations(employee.Id, true, true, true, true).Returns(reservations);
            _mapper.Map<List<ReservationDto>>(Arg.Any<List<Reservation>>()).Returns(reservationDtos);

            // Act
            List<ReservationDto> result = _reservationService.PastReservations(employee.Email);

            // Assert
            Assert.IsTrue(result.Count == 1);
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

        [TestCase(null, null)]
        [TestCase(2, 0)]
        [Order(7)]
        public void AllReservations_Success(int? take, int? skip)
        {
            // Arrange
            int? dbTotal = 3;
            List<Reservation> partReservations = new List<Reservation>() { new Reservation() { Id = 1 }, new Reservation() { Id = 2 } };
            List<Reservation> allReservations = new List<Reservation>() { new Reservation() { Id = 1 }, new Reservation() { Id = 2 }, new Reservation() { Id = 3 } };
            List<ReservationDto> partReservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 1 }, new ReservationDto() { Id = 2 } };
            List<ReservationDto> allReservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 1 }, new ReservationDto() { Id = 2 }, new ReservationDto() { Id = 3 } };

            Tuple<int?, List<Reservation>> tuple;
            if (take.HasValue && skip.HasValue)
            {
                tuple = Tuple.Create(take, partReservations);
            }
            else
            {
                tuple = Tuple.Create(dbTotal, allReservations);
            }

            _reservationRepository.GetAll(includeEmployee: true, includeDesk: true, includeOffice: true, take: take, skip: skip).Returns(tuple);
            _mapper.Map<List<ReservationDto>>(partReservations).Returns(partReservationDtos);
            _mapper.Map<List<ReservationDto>>(allReservations).Returns(allReservationDtos);

            // Act
            PaginationDto<ReservationDto> result = _reservationService.AllReservations(take: take, skip: skip);

            // Assert
            if (take.HasValue && skip.HasValue)
            {
                Assert.IsTrue(result.TotalCount == take);
                Assert.IsTrue(result.Values.Select(x => x.Id).All(x => partReservationDtos.Select(y => y.Id).Contains(x)));
            }
            else
            {
                Assert.IsTrue(result.TotalCount == dbTotal);
                Assert.IsTrue(result.Values.Select(x => x.Id).All(x => allReservationDtos.Select(y => y.Id).Contains(x)));
            }
        }

        [Test]
        [Order(8)]
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
        [Order(9)]
        public void CoworkerReserve_WrongDeskId_ThrowsNotFoundException()
        {
            // Arrange
            ReservationRequest request = new ReservationRequest() { DeskId = 11 };

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reservationService.CoworkerReserve(request));
            Assert.IsTrue(exception.Message == $"Desk with ID: {request.DeskId} not found.");
        }

        [Test]
        [Order(10)]
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
        [Order(11)]
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
        [Order(12)]
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
