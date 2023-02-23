using AutoMapper;
using MyDesk.BusinessLogicLayer;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using NSubstitute;
using NUnit.Framework;
using MyDesk.Core.Database;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;

namespace MyDesk.UnitTests.Service
{
    public class ConferenceRoomServiceTests
    {
        private IConferenceRoomService _conferenceRoomService;
        private IMapper _mapper;
        IContext _context;

        [OneTimeSetUp]
        public void Setup()
        {
            _mapper = Substitute.For<IMapper>();
            _context = Substitute.For<IContext>();

            _conferenceRoomService = new ConferenceRoomService(_mapper, _context);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(1)]
        public void GetOfficeConferenceRooms_Success(int? take, int? skip)
        {
            // Arrange
            int officeId = 15;
            List<ConferenceRoom> conferenceRooms = new List<ConferenceRoom>()
            {
                new ConferenceRoom()
                {
                    Id = 1,
                    OfficeId = 1,
                    Capacity = 10,
                    IndexForOffice = 1
                },
                new ConferenceRoom()
                {
                    Id = 2,
                    OfficeId = 1,
                    Capacity = 20,
                    IndexForOffice = 2
                }
            };

            List<ConferenceRoomDto> conferenceRoomDtos = new List<ConferenceRoomDto>()
            {
                new ConferenceRoomDto()
                {
                    Id = 1,
                    Capacity = 10,
                    IndexForOffice = 1,
                    Office = new OfficeDto(),
                    Reservations = new List<ReservationDto>()
                },
                new ConferenceRoomDto()
                {
                    Id = 2,
                    Capacity = 20,
                    IndexForOffice = 2,
                    Office = new OfficeDto(),
                    Reservations = new List<ReservationDto>()
                }
            };

            //_conferenceRoomRepository.GetOfficeConferenceRooms(officeId, true, take, skip).Returns(conferenceRooms);
            _mapper.Map<List<ConferenceRoomDto>>(conferenceRooms).Returns(conferenceRoomDtos);

            // Act
            List<ConferenceRoomDto> result = _conferenceRoomService.GetOfficeConferenceRooms(officeId, take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.Count == 2);
            //_conferenceRoomRepository.Received(1).GetOfficeConferenceRooms(officeId, true, take, skip);
            _mapper.Received(1).Map<List<ConferenceRoomDto>>(conferenceRooms);
        }

        [Test]
        [Order(2)]
        public void Delete_Success()
        {
            // Arrange
            int conferenceRoomId = 15;
            ConferenceRoom conferenceRoom = new ConferenceRoom()
            {
                Id = conferenceRoomId,
                OfficeId = 1,
                Capacity = 10,
                IndexForOffice = 1,
                Reservations = new List<Reservation>()
                {
                    new Reservation() 
                    { 
                        Id = 1, 
                        ConferenceRoomId = conferenceRoomId, 
                        EmployeeId = 2, 
                        IsDeleted = false, 
                        StartDate = DateTime.Now, 
                        EndDate = DateTime.Now.AddDays(2),
                        Reviews = new List<Review>()
                        {
                            new Review()
                            {
                                Id = 1,
                                ReservationId = 1,
                                Reviews = "test",
                                IsDeleted = false
                            },
                            new Review()
                            {
                                Id = 2,
                                ReservationId = 1,
                                Reviews = "test 2",
                                IsDeleted = false
                            }
                        }
                    },
                    new Reservation() 
                    { 
                        Id = 2, 
                        ConferenceRoomId = conferenceRoomId, 
                        EmployeeId = 5, 
                        IsDeleted = false, 
                        StartDate = DateTime.Now.AddDays(4), 
                        EndDate = DateTime.Now.AddDays(6),
                        Reviews = new List<Review>()
                        {
                            new Review()
                            {
                                Id = 3,
                                ReservationId = 2,
                                Reviews = "test 3",
                                IsDeleted = false
                            }
                        }
                    }
                }
            };

            //_conferenceRoomRepository.Get(conferenceRoomId, Arg.Any<bool>(), Arg.Any<bool>()).Returns(conferenceRoom);

            // Act
            _conferenceRoomService.Delete(conferenceRoomId);

            // Assert
            //_conferenceRoomRepository.Received(1).SoftDelete(Arg.Is<ConferenceRoom>(x => x.Reservations.All(y => y.IsDeleted == true && y.Reviews.All(z => z.IsDeleted == true))));
        }

        [Test]
        [Order(3)]
        public void Delete_ThrowsNotFoundException()
        {
            // Arrange
            int id = 2;
            //_conferenceRoomRepository.Get(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<bool>()).Returns(x => null);

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _conferenceRoomService.Delete(id));
            Assert.IsTrue(exception.Message == $"Conference room with ID: {id} not found.");
        }
    }
}
