using AutoMapper;
using inOffice.BusinessLogicLayer.Implementation;
using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class DeskServiceTests
    {
        private IDeskService _deskService;
        private IOfficeRepository _officeRepository;
        private IDeskRepository _deskRepository;
        private ICategoriesRepository _categoriesRepository;
        private IMapper _mapper;

        [OneTimeSetUp]
        public void Setup()
        {
            _officeRepository = Substitute.For<IOfficeRepository>();
            _deskRepository = Substitute.For<IDeskRepository>();
            _categoriesRepository = Substitute.For<ICategoriesRepository>();
            _mapper = Substitute.For<IMapper>();

            _deskService = new DeskService(_officeRepository, _deskRepository, _categoriesRepository, _mapper);
        }

        [TestCase(10, 0)]
        [Order(1)]
        public void GetOfficeDesks_Success(int? take, int? skip)
        {
            // Arrange
            int officeId = 11;
            List<Desk> desks = new List<Desk>()
            {
                new Desk()
                {
                    Id = 1,
                    OfficeId = officeId,
                    CategorieId = 2,
                    IndexForOffice = 1
                },
                new Desk()
                {
                    Id = 2,
                    OfficeId = officeId,
                    CategorieId = 2,
                    IndexForOffice = 2
                }
            };

            List<DeskDto> deskDtos = new List<DeskDto>()
            {
                new DeskDto()
                {
                    Id = 1
                },
                new DeskDto()
                {
                    Id = 2
                }
            };

            _deskRepository.GetOfficeDesks(officeId, true, true, true, take: take, skip: skip).Returns(desks);
            _mapper.Map<List<DeskDto>>(desks).Returns(deskDtos);

            // Act
            List<DeskDto> result = _deskService.GetOfficeDesks(officeId, take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result == deskDtos);
        }

        [Test]
        [Order(2)]
        public void Create_Success()
        {
            // Arrange
            int officeId = 11;
            int index = 5;
            int numberOfDesks = 2;

            Office office = new Office()
            {
                Id = officeId,
                Name = "Main office"
            };

            _officeRepository.Get(officeId).Returns(office);
            _deskRepository.GetHighestDeskIndexForOffice(officeId).Returns(index);

            // Act
            _deskService.Create(officeId, numberOfDesks);

            // Assert
            _deskRepository.Received(1).BulkInsert(Arg.Is<List<Desk>>(x => x.Count == numberOfDesks && 
                x.All(y => y.OfficeId == officeId && y.Categories == "regular") && x.Any(y => y.IndexForOffice == index + 1) && x.Any(y => y.IndexForOffice == index + 2)));
        }

        [Test]
        [Order(3)]
        public void Create_ThrowsNotFoundException()
        {
            // Arrange
            int officeId = 21;

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _deskService.Create(officeId, 3));
            Assert.IsTrue(exception.Message == $"Office with ID: {officeId} not found.");
        }

        [Test]
        [Order(4)]
        public void Update_Success()
        {
            // Arrange
            int existingCategoryId = 31;
            List<DeskDto> deskDtos = new List<DeskDto>() 
            { 
                new DeskDto() 
                { 
                    Id = 1,
                    Category = new CategoryDto() { Id = 1, DoubleMonitor = true, SingleMonitor = false, NearWindow = true, Unavailable = false, Desks = new List<DeskDto>() }
                }, 
                new DeskDto() 
                { 
                    Id = 2,
                    Category = new CategoryDto() { Id = 2, DoubleMonitor = false, SingleMonitor = true, NearWindow = true, Unavailable = false, Desks = new List<DeskDto>() }
                }, 
                new DeskDto() 
                { 
                    Id = 3
                } 
            };

            foreach (DeskDto deskDto in deskDtos.Where(x => x.Id != 3))
            {
                _deskRepository.Get(deskDto.Id.Value).Returns(new Desk() { Id = deskDto.Id.Value });
            }

            _categoriesRepository.Get(true, true, false, false).Returns(new Category() { Id = existingCategoryId });

            // Act
            _deskService.Update(deskDtos);

            // Assert
            _deskRepository.Received(1).Update(Arg.Is<Desk>(x => x.CategorieId == existingCategoryId));
            _categoriesRepository.Received(1).Insert(Arg.Is<Category>(x => x.DoubleMonitor == false && 
                x.SingleMonitor == true && x.NearWindow == true && x.Unavailable == false));
        }

        [Test]
        [Order(5)]
        public void Delete_Success()
        {
            // Arrange
            Desk desk = new Desk()
            {
                Id = 5,
                Reservations = new List<Reservation>()
                {
                    new Reservation()
                    {
                        Id = 7,
                        IsDeleted = false,
                        Reviews = new List<Review>()
                        {
                            new Review() { Id = 1, IsDeleted = false },
                            new Review() { Id = 2, IsDeleted = false }
                        }
                    },
                    new Reservation()
                    {
                        Id = 10,
                        IsDeleted = false,
                        Reviews = new List<Review>()
                        {
                            new Review() { Id = 4, IsDeleted = false },
                            new Review() { Id = 6, IsDeleted = false }
                        }
                    }
                }
            };

            _deskRepository.Get(desk.Id, true, true).Returns(desk);

            // Act
            _deskService.Delete(desk.Id);

            // Assert
            _deskRepository.Received(1).SoftDelete(Arg.Is<Desk>(x => x.Reservations.All(y => y.IsDeleted == true && y.Reviews.All(z => z.IsDeleted == true))));
        }

        [Test]
        [Order(6)]
        public void Delete_ThrowsNotFoundException()
        {
            // Arrange
            int id = 8;

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _deskService.Delete(id));
            Assert.IsTrue(exception.Message == $"Desk with ID: {id} not found.");
        }
    }
}
