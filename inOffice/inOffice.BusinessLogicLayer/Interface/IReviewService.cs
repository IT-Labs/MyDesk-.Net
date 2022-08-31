using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReviewService
    {
        ReviewResponse ShowReview(int id);
        List<ReviewDto> AllReviews(int? take = null, int? skip = null);
        CreateReviewResponse CreateReview(CreateReviewRequest createReviewRequest);
    }
}
