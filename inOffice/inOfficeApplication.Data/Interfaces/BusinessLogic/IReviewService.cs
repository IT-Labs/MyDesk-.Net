using inOfficeApplication.Data.DTO;

namespace inOfficeApplication.Data.Interfaces.BusinessLogic
{
    public interface IReviewService
    {
        List<ReviewDto> GetReviewsForDesk(int id);
        ReviewDto ShowReview(int id);
        List<ReviewDto> AllReviews(int? take = null, int? skip = null);
        void CreateReview(ReviewDto reviewDto);
    }
}
