using AutoMapper;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;
using Newtonsoft.Json;
using System.Text;
using MyDesk.Core.Responses;
using Microsoft.Extensions.Configuration;
using MyDesk.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace MyDesk.BusinessLogicLayer
{
    public class ReviewService : IReviewService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IContext _context;

        public ReviewService(IConfiguration config,
            IMapper mapper,
            IHttpClientFactory clientFactory,
            IContext context)
        {
            _config = config;
            _mapper = mapper;
            _httpClient = clientFactory.CreateClient();
            _context = context;
        }

        public List<ReviewDto> GetReviewsForDesk(int id)
        {
            var deskReservations = _context
                .AsQueryable<Reservation>()
                .Where(x => x.DeskId == id && x.IsDeleted == false && x.StartDate < DateTime.Now.Date && x.EndDate < DateTime.Now.Date)
                .Include(x => x.Reviews.Where(y => y.IsDeleted == false));

            var reviews = deskReservations
                .SelectMany(x => x.Reviews)
                .ToList();

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public ReviewDto ShowReview(int id)
        {
            var review = _context
                .AsQueryable<Review>()
                .FirstOrDefault(x => x.Id == id && x.IsDeleted == false);

            if (review == null)
            {
                throw new NotFoundException($"Review with ID: {id} not found.");
            }

            return _mapper.Map<ReviewDto>(review);
        }

        public List<ReviewDto> AllReviews(int? take = null, int? skip = null)
        {
            var query = _context
                .AsQueryable<Review>()
                .Where(x => x.IsDeleted == false);

            var reviews = (take.HasValue && skip.HasValue) ?
                query.Skip(skip.Value).Take(take.Value).ToList() :
                query.ToList();

            foreach (var review in reviews)
            {
                // EF will handle references
                // We used foreach instead of SQL join in order to prevent inefficient queries
                review.Reservation = _context
                    .AsQueryable<Reservation>()
                    .Include(x => x.Desk)
                        .ThenInclude(x => x.Office)
                    .Include(x => x.ConferenceRoom)
                        .ThenInclude(x => x.Office)
                    .FirstOrDefault(x => x.Id == review.ReservationId && x.IsDeleted == false); 
            }

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public void CreateReview(ReviewDto reviewDto)
        {
            var reservation = _context
                .AsQueryable<Reservation>()
                .Where(x => x.Id == reviewDto.Reservation.Id && x.IsDeleted == false);

            if (reservation == null)
            {
                throw new NotFoundException($"Reservation with ID: {reviewDto?.Reservation?.Id ?? 0} not found");
            }

            Review review = new Review()
            {
                Reviews = reviewDto?.Reviews,
                ReservationId = reviewDto?.Reservation?.Id??0,
                ReviewOutput = GetAnalysedReview(reviewDto?.Reviews??string.Empty)
            };

            _context.Insert(review);
        }

        private string GetAnalysedReview(string textReview)
        {
            string review = string.Empty;
            string sentimentEndpoint = _config["SentimentEndpoint"];

            if (!string.IsNullOrEmpty(sentimentEndpoint))
            {
                Dictionary<string, string> dictionaryData = new () { { "text", textReview } };
                string data = JsonConvert.SerializeObject(dictionaryData);

                HttpRequestMessage request = new ()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(sentimentEndpoint),
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };
                HttpResponseMessage response = _httpClient.Send(request, CancellationToken.None);

                if (response.IsSuccessStatusCode)
                {
                    string stringResponse = response.Content.ReadAsStringAsync().Result;
                    ReviewAzureFunction? result = JsonConvert.DeserializeObject<ReviewAzureFunction>(stringResponse);

                    review = result?.Sentiment??string.Empty;
                }
            }

            return review;
        }
    }
}
