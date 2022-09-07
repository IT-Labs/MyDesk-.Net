using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using inOfficeApplication.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private HttpClient client = new HttpClient();

        public ReviewService(IReviewRepository reviewRepository,
            IReservationRepository reservationRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reservationRepository = reservationRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        public List<ReviewDto> GetReviewsForDesk(int id)
        {
            List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(id, includeReview: true);
            List<Review> reviews = deskReservations.SelectMany(x => x.Reviews).ToList();

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public ReviewDto ShowReview(int id)
        {
            Review review = _reviewRepository.Get(id);
            if (review == null)
            {
                throw new NotFoundException($"Review with ID: {id} not found.");
            }

            return _mapper.Map<ReviewDto>(review);
        }

        public List<ReviewDto> AllReviews(int? take = null, int? skip = null)
        {
            List<Review> reviews = _reviewRepository.GetAll(take: take, skip: skip);

            foreach (Review review in reviews)
            {
                // EF will handle references
                // We used foreach instead of SQL join in order to prevent inefficient queries
                _reservationRepository.Get(review.ReservationId, includeDesk: true, includeonferenceRoom: true, includeOffice: true);
            }

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public void CreateReview(ReviewDto reviewDto)
        {
            Reservation reservation = _reservationRepository.Get(reviewDto.Reservation.Id);
            if (reservation == null)
            {
                throw new NotFoundException($"Reservation with ID: {reviewDto.Reservation?.Id} not found");
            }

            Task<string> responseSentimentAnalysis = GetAnalysedReview(reviewDto.Reviews);
            Review review = new Review()
            {
                Reviews = reviewDto.Reviews,
                ReservationId = reviewDto.Reservation.Id,
                ReviewOutput = responseSentimentAnalysis.Result,
                Reservation = reservation
            };

            _reviewRepository.Insert(review);
        }

        private async Task<string> GetAnalysedReview(string textReview)
        {
            string review = string.Empty;
            Dictionary<string, string> dictionaryData = new Dictionary<string, string>();
            dictionaryData.Add("text", textReview);
            string data = JsonConvert.SerializeObject(dictionaryData);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(_configuration["Settings:SentimentEndpoint"], content);
            string stringResponse = response.Content.ReadAsStringAsync().Result;
            ReviewAzureFunction result = JsonConvert.DeserializeObject<ReviewAzureFunction>(stringResponse);
            if (response.IsSuccessStatusCode)
            {
                review = result.Sentiment;
            }
            return review;
        }
    }
}
