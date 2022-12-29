using AutoMapper;
using MyDesk.BusinessLogicLayer;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data.DTO;
using MyDesk.Data.Entities;
using MyDesk.Data.Exceptions;
using NSubstitute;
using NUnit.Framework;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MyDesk.UnitTests.Service
{
    public class ReviewServiceTests
    {
        private IReviewService _reviewService;
        private IReviewRepository _reviewRepository;
        private IReservationRepository _reservationRepository;
        private IMapper _mapper;
        private IHttpClientFactory _clientFactory;
        private IConfiguration _config;

        [OneTimeSetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                  {"SentimentEndpoint", "http://test.com"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _reviewRepository = Substitute.For<IReviewRepository>();
            _reservationRepository = Substitute.For<IReservationRepository>();
            _mapper = Substitute.For<IMapper>();
            _clientFactory = Substitute.For<IHttpClientFactory>();

            _clientFactory.CreateClient().Returns(new ReviewMockedHttpClient());

            _reviewService = new ReviewService(_reviewRepository, _reservationRepository, _config, _mapper, _clientFactory);
        }

        [Test]
        [Order(1)]
        public void CreateReview_Success()
        {
            // Arrange
            ReviewDto reviewDto = new()
            {
                Reviews = "Bad review",
                Reservation = new ReservationDto() { Id = 1 }
            };

            Reservation reservation = new() { Id = 1 };

            _reservationRepository.Get(reviewDto.Reservation.Id).Returns(reservation);

            // Act
            _reviewService.CreateReview(reviewDto);

            // Assert
            _reviewRepository.Received(1).Insert(Arg.Is<Review>(x => x.Reviews == reviewDto.Reviews && x.ReviewOutput == "Neutral" && x.ReservationId == reservation.Id));
        }

        [Test]
        [Order(2)]
        public void CreateReview_ThrowsNotFoundException()
        {
            // Arrange
            ReviewDto reviewDto = new ReviewDto()
            {
                Reviews = "Bad review",
                Reservation = new ReservationDto() { Id = 11 }
            };

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reviewService.CreateReview(reviewDto));
            Assert.IsTrue(exception.Message == $"Reservation with ID: {reviewDto.Reservation?.Id} not found");
        }

        [Test]
        [Order(3)]
        public void ShowReview_Success()
        {
            // Arrange
            Review review = new Review()
            {
                Id = 2,
                Reviews = "Bad review",
                ReviewOutput = "Bad"
            };

            ReviewDto reviewDto = new ReviewDto()
            {
                Id = 2,
                Reviews = "Bad review",
                ReviewOutput = "Bad"
            };

            _reviewRepository.Get(review.Id).Returns(review);
            _mapper.Map<ReviewDto>(review).Returns(reviewDto);

            // Act
            ReviewDto result = _reviewService.ShowReview(review.Id);

            // Assert
            Assert.NotNull(result);
            _mapper.Received(1).Map<ReviewDto>(review);
        }

        [Test]
        [Order(4)]
        public void ShowReview_ThrowsNotFoundException()
        {
            // Arrange
            int id = 21;

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _reviewService.ShowReview(id));
            Assert.IsTrue(exception.Message == $"Review with ID: {id} not found.");
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(5)]
        public void AllReviews_Success(int? take, int? skip)
        {
            // Arrange
            List<Review> reviews = new List<Review>() { new Review() { Id = 5, ReservationId = 5 }, new Review() { Id = 6, ReservationId = 6 } };
            List<ReviewDto> reviewDtos = new List<ReviewDto>() { new ReviewDto() { Id = 5 }, new ReviewDto() { Id = 6 } };

            _reviewRepository.GetAll(take: take, skip: skip).Returns(reviews);
            _mapper.Map<List<ReviewDto>>(reviews).Returns(reviewDtos);

            // Act
            List<ReviewDto> result = _reviewService.AllReviews(take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.Count == 2 && result.Any(x => x.Id == 5) && result.Any(x => x.Id == 6));
            _reservationRepository.Received(1).Get(reviews.First().ReservationId, true, true, true);
            _reservationRepository.Received(1).Get(reviews.Last().ReservationId, true, true, true);
            _mapper.Received(1).Map<List<ReviewDto>>(reviews);
            _reservationRepository.ClearReceivedCalls();
        }

        [Test]
        [Order(6)]
        public void GetReviewsForDesk_Success()
        {
            // Arrange
            int deskId = 32;
            List<Reservation> reservations = new List<Reservation>() 
            { 
                new Reservation() 
                { 
                    Reviews = new List<Review>()
                    {
                        new Review()
                        {
                            Reviews = "Good review"
                        },
                        new Review()
                        {
                            Reviews = "Neutral review"
                        }
                    }
                },
                new Reservation()
                {
                    Reviews = new List<Review>()
                    {
                        new Review()
                        {
                            Reviews = "Bad review"
                        }
                    }
                }
            };
            _reservationRepository.GetPastDeskReservations(deskId, includeReview: true).Returns(reservations);

            // Act
            _reviewService.GetReviewsForDesk(deskId);

            // Assert
            _mapper.Received(1).Map<List<ReviewDto>>(Arg.Is<List<Review>>(x => x.Count == 3 && 
                x.Any(y => y.Reviews == "Good review") && x.Any(y => y.Reviews == "Neutral review") && x.Any(y => y.Reviews == "Bad review")));
        }
    }

    public class ReviewMockedHttpClient : HttpClient
    {
        public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new HttpResponseMessage() { Content = new StringContent("{\"Sentiment\": \"Neutral\", \"Text\": \"\"}", Encoding.UTF8, "application/json") };
        }
    }
}
