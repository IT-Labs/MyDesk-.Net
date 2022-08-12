using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Newtonsoft.Json;
using System.Text;
using System.Transactions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class ReviewService: IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReservationRepository _reservationRepository;
        private HttpClient client = new HttpClient();

        public ReviewService(IReviewRepository reviewRepository, IReservationRepository reservationRepository)
        {
            _reviewRepository = reviewRepository;
            _reservationRepository = reservationRepository;
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

        public AllReviewsResponse AllReviews()
        {
            List<CustomReviews> list = new List<CustomReviews>();

            List<Review> reviews = _reviewRepository.GetAll();

            foreach (Review review in reviews)
            {
                Reservation reservation = _reservationRepository.Get(review.ReservationId, includeDesk: true, includeOffice: true);

                CustomReviews custom = new CustomReviews()
                {
                    OfficeName = reservation.Desk?.Office?.Name,
                    Review = review.Reviews,
                    ReviewOutput = review.ReviewOutput,
                    DeskIndex = reservation.Desk?.IndexForOffice
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
            Task<string> responseSentimentAnalysis = GetAnalysedReview(createReviewRequest.Review);

            try
            {
                Review review = new Review()
                {
                    Reviews = createReviewRequest.Review,
                    ReservationId = createReviewRequest.ReservationId,
                    ReviewOutput = responseSentimentAnalysis.Result
                };

                using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _reviewRepository.Insert(review);

                    reservation.ReviewId = review.Id;
                    _reservationRepository.Update(reservation);

                    transaction.Complete();
                }

                response.Success = true;
            }
            catch (Exception _)
            {
                response.Success = false;
            }

            return response;
        }

        private async Task<string> GetAnalysedReview(string textReview)
        {
            string review = string.Empty;
            Dictionary<string, string> dictionaryData = new Dictionary<string, string>();
            dictionaryData.Add("text", textReview);
            string data = JsonConvert.SerializeObject(dictionaryData);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://inofficenlpmodel.azurewebsites.net/api/get_sentiment?code=knpbFNoCymH02BtJtcO59H4mgbkRVbSBhSzlwuZmxXCtAzFuEqSMTA==", content);
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
