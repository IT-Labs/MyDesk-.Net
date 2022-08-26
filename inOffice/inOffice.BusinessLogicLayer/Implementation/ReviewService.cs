using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Entities;
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
        private HttpClient client = new HttpClient();

        public ReviewService(IReviewRepository reviewRepository,
            IReservationRepository reservationRepository,
            IConfiguration configuration)
        {
            _reviewRepository = reviewRepository;
            _reservationRepository = reservationRepository;
            _configuration = configuration;
        }

        public ReviewResponse ShowReview(int id)
        {
            Review review = _reviewRepository.Get(id);

            ReviewResponse reviewForGivenEntity = new ReviewResponse()
            {
                Review = review.Reviews,
                Sucess = true
            };

            return reviewForGivenEntity;
        }

        public AllReviewsResponse AllReviews(int? take = null, int? skip = null)
        {
            List<CustomReviews> list = new List<CustomReviews>();

            List<Review> reviews = _reviewRepository.GetAll(take: take, skip: skip);

            foreach (Review review in reviews)
            {
                Reservation reservation = _reservationRepository.Get(review.ReservationId, includeDesk: true, includeonferenceRoom: true, includeOffice: true);

                if (reservation == null)
                {
                    continue;
                }

                string officeName = string.Empty;
                int? deskIndex = null;
                if (reservation.Desk != null)
                {
                    officeName = reservation.Desk.Office?.Name;
                    deskIndex = reservation.Desk.IndexForOffice;
                }
                else if (reservation.ConferenceRoom != null)
                {
                    officeName = reservation.ConferenceRoom.Office?.Name;
                }

                CustomReviews custom = new CustomReviews()
                {
                    OfficeName = officeName,
                    Review = review.Reviews,
                    ReviewOutput = review.ReviewOutput,
                    DeskIndex = deskIndex
                };

                list.Add(custom);
            }

            AllReviewsResponse allReviews = new AllReviewsResponse()
            {
                ListOfReviews = list,
                Success = true
            };

            return allReviews;
        }

        public CreateReviewResponse CreateReview(CreateReviewRequest createReviewRequest)
        {
            CreateReviewResponse response = new CreateReviewResponse();

            Reservation reservation = _reservationRepository.Get(createReviewRequest.ReservationId);

            if (reservation == null)
            {
                response.Success = false;
                return response;
            }

            Task<string> responseSentimentAnalysis = GetAnalysedReview(createReviewRequest.Review);
            Review review = new Review()
            {
                Reviews = createReviewRequest.Review,
                ReservationId = createReviewRequest.ReservationId,
                ReviewOutput = responseSentimentAnalysis.Result,
                Reservation = reservation
            };

            _reviewRepository.Insert(review);

            response.Success = true;

            return response;
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
