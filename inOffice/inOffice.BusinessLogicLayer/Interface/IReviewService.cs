using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReviewService
    {
        List<ReviewDto> GetReviewsForDesk(int id);
        ReviewDto ShowReview(int id);
        List<ReviewDto> AllReviews(int? take = null, int? skip = null);
        void CreateReview(ReviewDto reviewDto);
    }
}
