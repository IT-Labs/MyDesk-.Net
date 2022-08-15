using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReviewService
    {
        ReviewResponse ShowReview(int id);
        AllReviewsResponse AllReviews(int? take = null, int? skip = null);
        CreateReviewResponse CreateReview(CreateReviewRequest createReviewRequest);
    }
}
