using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReviewService
    {
        ReviewDto ShowReview(int id);
        List<ReviewDto> AllReviews(int? take = null, int? skip = null);
        void CreateReview(ReviewDto reviewDto);
    }
}
