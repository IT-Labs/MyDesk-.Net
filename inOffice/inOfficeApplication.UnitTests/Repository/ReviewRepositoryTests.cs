using inOffice.Repository.Implementation;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Entities;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Repository
{
    public class ReviewRepositoryTests : RepositoryBaseTest
    {
        private IReviewRepository _reviewRepository;

        [SetUp]
        public void Setup()
        {
            _reviewRepository = new ReviewRepository(base._dbContext);
        }

        [TearDown]
        public void CleanUp()
        {
            base.CleanDbContext();
        }

        [Test]
        [Order(1)]
        public void Get_Success()
        {
            // Arrange
            int id = 2;

            // Act
            Review review = _reviewRepository.Get(id);

            // Assert
            Assert.NotNull(review, "Review should exist.");
            Assert.IsTrue(review.Id == id);
        }

        [Test]
        [Order(2)]
        public void Get_Failure()
        {
            // Arrange + Act
            Review review = _reviewRepository.Get(1);

            // Assert
            Assert.IsNull(review, "Review shouldn't exist.");
        }

        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(1, 1)]
        [Order(3)]
        public void GetAll_Success(int? take, int? skip)
        {
            // Arrange + Act
            List<Review> reviews = _reviewRepository.GetAll(take: take, skip: skip);

            // Assert
            if (skip.HasValue)
            {
                Assert.IsTrue(reviews.Count == take);
            }
            else
            {
                Assert.IsTrue(reviews.Count == 2);
            }
            
            reviews.ForEach(x => Assert.IsTrue(x.IsDeleted == false));
        }

        [Test]
        [Order(4)]
        public void Insert_Success()
        {
            // Arrange
            Review review = new Review()
            {
                Reviews = "Bad review",
                ReviewOutput = "Bad",
                ReservationId = 3,
                IsDeleted = false
            };

            // Act
            _reviewRepository.Insert(review);

            // Assert
            Review createdReview = _reviewRepository.Get(review.Id);

            Assert.NotNull(createdReview, "Review should not be null.");
            Assert.IsTrue(createdReview.Id == review.Id);
        }
    }
}
