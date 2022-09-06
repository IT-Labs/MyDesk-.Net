using inOffice.Repository.Implementation;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Entities;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Repository
{
    public class DeskRepositoryTests : TestBase
    {
        private IDeskRepository _deskRepository;

        [SetUp]
        public void Setup()
        {
            _deskRepository = new DeskRepository(base._dbContext);
        }

        [TearDown]
        public void CleanUp()
        {
            base.CleanDbContext();
        }

        [TestCase(null, null)]
        [TestCase(true, null)]
        [TestCase(true, true)]
        [Order(1)]
        public void Get_Success(bool? includeReservations, bool? includeReviews)
        {
            // Arrange + Act
            Desk desk = _deskRepository.Get(1, includeReservations: includeReservations, includeReviews: includeReviews);

            // Assert
            Assert.NotNull(desk, "Desk should exist.");

            if (includeReservations == true)
            {
                Assert.IsTrue(desk.Reservations.Count == 2);

                Reservation reservation = desk.Reservations.Single(x => x.Id == 1);
                if (includeReviews == true)
                {
                    Assert.IsTrue(reservation.Reviews.Count == 1);
                }
                else
                {
                    Assert.IsTrue(reservation.Reviews.Count == 0);
                }
            }
            else
            {
                Assert.IsTrue(desk.Reservations.Count == 0);
            }
        }

        [Test]
        [Order(2)]
        public void Get_Failure()
        {
            // Arrange + Act
            Desk desk = _deskRepository.Get(2);

            // Assert
            Assert.IsNull(desk, "Desk shouldn't exist.");
        }

        [Test]
        [Order(3)]
        public void GetHighestDeskIndexForOffice_Success()
        {
            // Arrange + Act
            int index = _deskRepository.GetHighestDeskIndexForOffice(1);

            // Assert
            Assert.IsTrue(index == 3);
        }

        [TestCase(null, null, null)]
        [TestCase(true, null, null)]
        [TestCase(true, 1, 0)]
        [Order(4)]
        public void GetOfficeDesks_Success(bool? includeCategory, int? take, int? skip)
        {
            // Arrange + Act
            List<Desk> desks = _deskRepository.GetOfficeDesks(1, includeCategory: includeCategory, take: take, skip: skip);

            // Assert
            if (take.HasValue && skip.HasValue)
            {
                Assert.IsTrue(desks.Count == take);
            }
            else
            {
                Assert.IsTrue(desks.Count == 2);
            }

            if (includeCategory == true)
            {
                foreach (Desk desk in desks)
                {
                    Assert.NotNull(desk.Categorie);
                }
            }
        }

        [Test]
        [Order(5)]
        public void BulkInsert_Success()
        {
            // Arrange
            List<Desk> desks = new List<Desk>()
            {
                new Desk()
                {
                    OfficeId = 3,
                    CategorieId = 2,
                    IndexForOffice = 4,
                    Categories = "regular",
                    IsDeleted = false
                },
                new Desk()
                {
                    OfficeId = 3,
                    CategorieId = 2,
                    IndexForOffice = 5,
                    Categories = "regular",
                    IsDeleted = false
                }
            };

            // Act
            _deskRepository.BulkInsert(desks);

            // Assert
            foreach (Desk desk in desks)
            {
                Desk createdDesk = _deskRepository.Get(desk.Id);
                Assert.NotNull(createdDesk);
            }
        }

        [Test]
        [Order(6)]
        public void Update_Success()
        {
            // Arrange
            int id = 3;
            string categories = "not regular";

            Desk desk = _deskRepository.Get(id);
            desk.Categories = categories;

            // Act
            _deskRepository.Update(desk);

            // Assert
            Desk updatedDesk = _deskRepository.Get(id);
            Assert.IsTrue(updatedDesk.Id == id && desk.Categories == categories);
        }

        [Test]
        [Order(7)]
        public void SoftDelete_Success()
        {
            // Arrange
            int id = 3;
            Desk desk = _deskRepository.Get(id);

            // Act
            _deskRepository.SoftDelete(desk);

            // Assert
            Desk deletedDesk = _deskRepository.Get(id);
            Assert.IsNull(deletedDesk, "Desk shouldn't exist.");
        }
    }
}
