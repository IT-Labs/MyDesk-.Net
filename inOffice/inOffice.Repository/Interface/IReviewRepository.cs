using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Interface
{
    public interface IReviewRepository
    {
        Review Get(int id);
        List<Review> GetAll();
        void Insert(Review review);
    }
}
