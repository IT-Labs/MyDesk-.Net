using AutoMapper;
using MyDesk.BusinessLogicLayer;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;
using NSubstitute;
using NUnit.Framework;
using MyDesk.Core.Database;
using NSubstitute.ReceivedExtensions;

namespace MyDesk.UnitTests.Service
{
    public class OfficeServiceTests
    {
        private IOfficeService _officeService;
        private IMapper _mapper;
        private IContext _context;

        [OneTimeSetUp]
        public void Setup()
        {
            _mapper = Substitute.For<IMapper>();
            _context = Substitute.For<IContext>();

            _officeService = new OfficeService(_mapper, _context);
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
            _context.Received().Insert(Arg.Is<Office>(x => x.Name == name && x.OfficeImage == image));
        }

        [Test]
        [Order(2)]
        public void CreateNewOffice_ThrowsConflictException()
        {
            // Arrange
            Office office = GetOffices().Where(x => x.Id == 5).First();
            OfficeDto officeDto = new OfficeDto
            {
                Id = office.Id,
                Name = office.Name,
                OfficeImage = office.OfficeImage
            };

            _context.AsQueryable<Office>().Returns(GetOffices());

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
                Id = 5,
                Name = "Changed office",
                OfficeImage = "Changed image"
            };

            _context.AsQueryable<Office>().Returns(GetOffices());

            // Act
            _officeService.UpdateOffice(officeDto);

            // Assert
            _context.Received(1).Modify(Arg.Is<Office>(x => x.Id == officeDto.Id && x.Name == officeDto.Name && x.OfficeImage == officeDto.OfficeImage));
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
            Office firstOffice = GetOffices().Skip(0).Take(1).First();
            Office secondOffice = GetOffices().Skip(1).Take(1).First();
            OfficeDto officeDto = new OfficeDto
            {
                Id = firstOffice.Id,
                Name = secondOffice.Name,
                OfficeImage = firstOffice.OfficeImage
            };

            _context.AsQueryable<Office>().Returns(GetOffices());

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
                IsDeleted = false,
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

            var offices = new List<Office> { office };

            _context.AsQueryable<Office>().Returns(offices.AsQueryable());
            _context.AsQueryable<Desk>().Returns(offices.First().Desks.AsQueryable());
            _context.AsQueryable<ConferenceRoom>().Returns(offices.First().ConferenceRooms.AsQueryable());

            // Act
            _officeService.DeleteOffice(id);

            // Assert
            _context.Received().Modify(Arg.Is<Office>(x =>
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
            var offices = GetOffices();
            _context.AsQueryable<Office>().Returns(offices);
            var excpectedCount = offices.Count();

            var expectedOfficesDto = new List<OfficeDto>();
            foreach (var item in offices.ToList())
            {
                expectedOfficesDto.Add(
                    new OfficeDto {
                    Id = item.Id,
                    Name = item.Name,
                    OfficeImage = item.OfficeImage
                });
            }
            _mapper.Map<List<OfficeDto>>(Arg.Any<object>()).Returns(expectedOfficesDto);

            //_mapper.When(t => t.Map<List<OfficeDto>>(Arg.Any<object>())).Do(p => expectedOfficesDto = p);
            List<OfficeDto> result = _officeService.GetAllOffices(take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.Count == excpectedCount);
        }

        [Test]
        [Order(9)]
        public void GetDetailsForOffice_Success()
        {
            // Arrange
            var office = GetOffices().First();
            var expectedId = office.Id;
            OfficeDto officeDto = new OfficeDto()
            {
                Id = office.Id,
                Name = office.Name,
                OfficeImage = office.OfficeImage
            };

            _context.AsQueryable<Office>().Returns(GetOffices());
            _mapper.Map<OfficeDto>(Arg.Any<object>()).Returns(officeDto);

            // Act
            OfficeDto actualResult = _officeService.GetDetailsForOffice(expectedId);

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsTrue(expectedId == actualResult.Id);
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


        private IQueryable<Office> GetOffices ()
        {
            var offices = new List<Office>
            {
                new Office { Id = 5, Name = "Test office5", OfficeImage = "image6", IsDeleted = false },
                new Office { Id = 6, Name = "Test office6", OfficeImage = "image6", IsDeleted = false },
                new Office { Id = 7, Name = "Test office7", OfficeImage = "image7", IsDeleted = false }
            };
            return offices.AsQueryable();
        }
    }
}
