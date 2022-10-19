using inOfficeApplication.Data.Entities;

namespace inOfficeApplication.Data.Interfaces.Repository
{
    public interface IReviewRepository
    {
        Review Get(int id);
        List<Review> GetAll(int? take = null, int? skip = null);
        void Insert(Review review);
    }
}
