using AutoMapper;
using inOffice.BusinessLogicLayer;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class OfficeServiceTests
    {
        private IOfficeService _officeService;
        private IOfficeRepository _officeRepository;
        private IDeskRepository _deskRepository;
        private IConferenceRoomRepository _conferenceRoomRepository;
        private IMapper _mapper;

        [OneTimeSetUp]
        public void Setup()
        {
            _officeRepository = Substitute.For<IOfficeRepository>();
            _deskRepository = Substitute.For<IDeskRepository>();
            _conferenceRoomRepository = Substitute.For<IConferenceRoomRepository>();
            _mapper = Substitute.For<IMapper>();

            _officeService = new OfficeService(_officeRepository, _deskRepository, _conferenceRoomRepository, _mapper);
        }

        [Test]
        [Order(1)]
        public void CreateNewOffice_Success()
        {
            // Arrange
            string name = "Test office";
            string image = "image";

            OfficeDto officeDto = new OfficeDto()
            {
                Name = name,
                OfficeImage = image
            };

            // Act
            _officeService.CreateNewOffice(officeDto);

            // Assert
            _officeRepository.Received(1).Insert(Arg.Is<Office>(x => x.Name == name && x.OfficeImage == image));
        }

        [Test]
        [Order(2)]
        public void CreateNewOffice_ThrowsConflictException()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto()
            {
                Name = "Test office",
                OfficeImage = "image"
            };

            Office office = new Office()
            {
                Name = "Test office",
                OfficeImage = "image"
            };

            _officeRepository.GetByName(office.Name).Returns(office);

            // Act + Assert
            ConflictException exception = Assert.Throws<ConflictException>(() => _officeService.CreateNewOffice(officeDto));
            Assert.IsTrue(exception.Message == "There is allready office with the same name");
        }

        [Test]
        [Order(3)]
        public void UpdateOffice_Success()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto()
            {
                Id = 1,
                Name = "Changed office",
                OfficeImage = "Changed image"
            };

            Office office = new Office()
            {
                Id = 1,
                Name = "Test office",
                OfficeImage = "image"
            };

            _officeRepository.Get(officeDto.Id.Value).Returns(office);

            // Act
            _officeService.UpdateOffice(officeDto);

            // Assert
            _officeRepository.Received(1).Update(Arg.Is<Office>(x => x.Id == officeDto.Id && x.Name == officeDto.Name && x.OfficeImage == officeDto.OfficeImage));
        }

        [Test]
        [Order(4)]
        public void UpdateOffice_ThrowsNotFoundException()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto()
            {
                Id = 10,
                Name = "Changed office",
                OfficeImage = "Changed image"
            };

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _officeService.UpdateOffice(officeDto));
            Assert.IsTrue(exception.Message == $"Office with ID: {officeDto.Id} not found.");
        }

        [Test]
        [Order(5)]
        public void UpdateOffice_ThrowsConflictException()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto()
            {
                Id = 1,
                Name = "Changed office",
                OfficeImage = "Changed image"
            };

            Office sameExistingOffice = new Office()
            {
                Id = 1,
                Name = "Test office",
                OfficeImage = "image"
            };

            Office differentExistingOffice = new Office()
            {
                Id = 2,
                Name = "Changed office",
                OfficeImage = "Some other image"
            };

            _officeRepository.Get(officeDto.Id.Value).Returns(sameExistingOffice);
            _officeRepository.GetByName(officeDto.Name).Returns(differentExistingOffice);

            // Act + Assert
            ConflictException exception = Assert.Throws<ConflictException>(() => _officeService.UpdateOffice(officeDto));
            Assert.IsTrue(exception.Message == "There is already office with the same name.");
        }

        [Test]
        [Order(6)]
        public void DeleteOffice_Success()
        {
            // Arrange
            int id = 7;
            Office office = new Office()
            {
                Id = id,
                Name = "Test office",
                OfficeImage = "image",
                Desks = new List<Desk>()
                {
                    new Desk()
                    {
                        Id = 1,
                        IsDeleted = false,
                        Reservations = new List<Reservation>()
                        {
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false }, new Review() { IsDeleted = false } } },
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false } } }
                        }
                    },
                    new Desk()
                    {
                        Id = 2,
                        IsDeleted = false,
                        Reservations = new List<Reservation>()
                        {
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false }, new Review() { IsDeleted = false } } },
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false } } }
                        }
                    },
                    new Desk()
                    {
                        Id = 3,
                        IsDeleted = false,
                        Reservations = new List<Reservation>()
                        {
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false }, new Review() { IsDeleted = false } } },
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false } } }
                        }
                    }
                },
                ConferenceRooms = new List<ConferenceRoom>()
                {
                    new ConferenceRoom()
                    {
                        Id = 1,
                        IsDeleted = false,
                        Reservations = new List<Reservation>()
                        {
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false }, new Review() { IsDeleted = false } } },
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false } } }
                        }
                    },
                    new ConferenceRoom()
                    {
                        Id = 2,
                        IsDeleted = false,
                        Reservations = new List<Reservation>()
                        {
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false }, new Review() { IsDeleted = false } } },
                            new Reservation() { IsDeleted = false, Reviews = new List<Review>() { new Review() { IsDeleted = false } } }
                        }
                    }
                }
            };

            foreach (Desk desk in office.Desks)
            {
                _deskRepository.Get(desk.Id, true, true).Returns(desk);
            }

            foreach (ConferenceRoom conferenceRoom in office.ConferenceRooms)
            {
                _conferenceRoomRepository.Get(conferenceRoom.Id, true, true).Returns(conferenceRoom);
            }

            _officeRepository.Get(id, true, true).Returns(office);

            // Act
            _officeService.DeleteOffice(id);

            // Assert
            _officeRepository.Received(1).SoftDelete(Arg.Is<Office>(x => 
                x.Desks.All(y => y.IsDeleted == true && y.Reservations.All(z => z.IsDeleted == true && z.Reviews.All(r => r.IsDeleted == true))) && 
                x.ConferenceRooms.All(y => y.IsDeleted == true && y.Reservations.All(z => z.IsDeleted == true && z.Reviews.All(r => r.IsDeleted == true)))));
        }

        [Test]
        [Order(7)]
        public void DeleteOffice_ThrowsNotFoundException()
        {
            // Arrange
            int id = 21;

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _officeService.DeleteOffice(id));
            Assert.IsTrue(exception.Message == $"Office with ID: {id} not found.");
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(8)]
        public void GetAllOffices_Success(int? take, int? skip)
        {
            // Arrange
            List<Office> offices = new List<Office>()
            {
                new Office()
                {
                    Name = "Main office"
                },
                new Office()
                {
                    Name = "Side office"
                }
            };

            List<OfficeDto> officeDtos = new List<OfficeDto>()
            {
                new OfficeDto()
                {
                    Name = "Main office"
                },
                new OfficeDto()
                {
                    Name = "Side office"
                }
            };

            _officeRepository.GetAll(take: take, skip: skip).Returns(offices);
            _mapper.Map<List<OfficeDto>>(offices).Returns(officeDtos);

            // Act
            List<OfficeDto> result = _officeService.GetAllOffices(take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.Count == 2);
            _officeRepository.Received(1).GetAll(take: take, skip: skip);
            _mapper.Received(1).Map<List<OfficeDto>>(offices);
        }

        [Test]
        [Order(9)]
        public void GetDetailsForOffice_Success()
        {
            // Arrange
            int id = 7;
            Office office = new Office()
            {
                Id = id,
                Name = "Test office",
                OfficeImage = "image"
            };

            OfficeDto officeDto = new OfficeDto()
            {
                Id = id,
                Name = "Test office",
                OfficeImage = "image"
            };

            _officeRepository.Get(id).Returns(office);
            _mapper.Map<OfficeDto>(office).Returns(officeDto);

            // Act
            OfficeDto result = _officeService.GetDetailsForOffice(id);

            // Assert
            Assert.NotNull(result);
            _officeRepository.Received(1).Get(id);
            _mapper.Received(1).Map<OfficeDto>(office);
        }

        [Test]
        [Order(10)]
        public void GetDetailsForOffice_ThrowsNotFoundException()
        {
            // Arrange
            int id = 32;

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _officeService.GetDetailsForOffice(id));
            Assert.IsTrue(exception.Message == $"Office with ID: {id} not found.");
        }
    }
}
