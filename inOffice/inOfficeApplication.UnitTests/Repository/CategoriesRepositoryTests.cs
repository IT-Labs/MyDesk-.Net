using inOffice.Repository.Implementation;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Entities;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Repository
{
    public class CategoriesRepositoryTests : TestBase
    {
        private ICategoriesRepository _categoriesRepository;

        [SetUp]
        public void Setup()
        {
            _categoriesRepository = new CategoriesRepository(base._dbContext);
        }

        [TearDown]
        public void CleanUp()
        {
            base.CleanDbContext();
        }

        [TestCase(true, true, false)]
        [TestCase(false, true, true)]
        [Order(1)]
        public void Get_Success(bool singleMonitor, bool nearWindow, bool doubleMonitor)
        {
            // Arrange + Act
            Category category = _categoriesRepository.Get(doubleMonitor, nearWindow, singleMonitor, false);

            // Assert
            Assert.NotNull(category, "Category should not be null.");

            if (singleMonitor == true)
            {
                Assert.IsTrue(category.Id == 2, "Single monitor category should have ID: 2.");
            }
            if (doubleMonitor == true)
            {
                Assert.IsTrue(category.Id == 3, "Single monitor category should have ID: 3.");
            }
        }

        [Test]
        [Order(2)]
        public void Insert_Success()
        {
            // Arrange
            Category category = new Category()
            {
                DoubleMonitor = false,
                NearWindow = false,
                SingleMonitor = true,
                Unavailable = false
            };

            // Act
            _categoriesRepository.Insert(category);

            // Assert
            Category createdCategory = _categoriesRepository.Get(category.DoubleMonitor, category.NearWindow, category.SingleMonitor, category.Unavailable);

            Assert.NotNull(createdCategory, "Category should not be null.");
            Assert.IsTrue(createdCategory.Id == category.Id);
        }

        [Test]
        [Order(3)]
        public void Update_Success()
        {
            // Arrange
            Category category = _categoriesRepository.Get(true, true, false, false);
            category.Unavailable = true;

            // Act
            _categoriesRepository.Update(category);

            // Assert
            Category updatedCategory = _categoriesRepository.Get(true, true, false, true);

            Assert.NotNull(updatedCategory, "Category should not be null.");
            Assert.IsTrue(updatedCategory.Id == category.Id && updatedCategory.Unavailable == true);
        }
    }
}
