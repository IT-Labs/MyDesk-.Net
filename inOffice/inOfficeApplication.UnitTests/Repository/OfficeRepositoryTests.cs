using inOffice.Repository;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.Entities;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Repository
{
    public class OfficeRepositoryTests : RepositoryBaseTest
    {
        private IOfficeRepository _officeRepository;

        [SetUp]
        public void Setup()
        {
            _officeRepository = new OfficeRepository(base._dbContext);
        }

        [TearDown]
        public void CleanUp()
        {
            base.CleanDbContext();
        }

        [TestCase(null, null)]
        [TestCase(true, null)]
        [TestCase(null, true)]
        [TestCase(true, true)]
        [Order(1)]
        public void Get_Success(bool? includeDesks, bool? includeConferenceRooms)
        {
            // Arrange
            int id = 1;

            // Act
            Office office = _officeRepository.Get(id, includeDesks: includeDesks, includeConferenceRooms: includeConferenceRooms);

            // Assert
            Assert.NotNull(office, "Office should exist.");
            Assert.IsTrue(office.Id == id);

            if (includeDesks == true)
            {
                Assert.IsTrue(office.Desks.Count == 2);
            }
            else
            {
                Assert.IsTrue(office.Desks.Count == 0);
            }

            if (includeConferenceRooms == true)
            {
                Assert.IsTrue(office.ConferenceRooms.Count == 2);
            }
            else
            {
                Assert.IsTrue(office.ConferenceRooms.Count == 0);
            }
        }

        [Test]
        [Order(2)]
        public void Get_Failure()
        {
            // Arrange
            int id = 2;

            // Act
            Office office = _officeRepository.Get(id);

            // Assert
            Assert.IsNull(office, "Office shouldn't exist.");
        }

        [Test]
        [Order(3)]
        public void GetByName_Success()
        {
            // Arrange
            string name = "Main office";

            // Act
            Office office = _officeRepository.GetByName(name);

            // Assert
            Assert.NotNull(office, "Office should exist.");
            Assert.IsTrue(office.Name == name);
        }

        [Test]
        [Order(4)]
        public void GetByName_Failure()
        {
            // Arrange + Act
            Office office = _officeRepository.GetByName("Non existing office");

            // Assert
            Assert.IsNull(office, "Office shouldn't exist.");
        }

        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(1, 1)]
        [Order(5)]
        public void GetAll_Success(int? take, int? skip)
        {
            // Arrange + Act
            List<Office> offices = _officeRepository.GetAll(take: take, skip: skip);

            // Assert
            if (skip.HasValue)
            {
                Assert.IsTrue(offices.Count == take);
            }
            else
            {
                Assert.IsTrue(offices.Count == 2);
            }

            offices.ForEach(x => Assert.IsTrue(x.IsDeleted == false));
        }

        [Test]
        [Order(6)]
        public void Insert_Success()
        {
            // Arrange
            Office office = new Office()
            {
                Name = "New office",
                OfficeImage = "New image",
                IsDeleted = false
            };

            // Act
            _officeRepository.Insert(office);

            // Assert
            Office createdOffice = _officeRepository.Get(office.Id);

            Assert.NotNull(createdOffice, "Office should not be null.");
            Assert.IsTrue(createdOffice.Id == office.Id);
        }

        [Test]
        [Order(7)]
        public void Update_Success()
        {
            // Arrange
            int id = 1;
            string name = "Changed name";

            Office office = _officeRepository.Get(id);
            office.Name = name;

            // Act
            _officeRepository.Update(office);

            // Assert
            Office updatedOffice = _officeRepository.Get(id);
            Assert.IsTrue(updatedOffice.Name == name);
        }

        [Test]
        [Order(8)]
        public void SoftDelete_Success()
        {
            // Arrange
            int id = 1;
            Office office = _officeRepository.Get(id);

            // Act
            _officeRepository.SoftDelete(office);

            // Assert
            Office updatedOffice = _officeRepository.Get(id);
            Assert.IsNull(updatedOffice, "Office shouldn't exist.");
        }
    }
}
