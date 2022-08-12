using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Implementation
{
    public class ReviewRepository: IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Review Get(int id)
        {
            return _context.Reviews.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<Review> GetAll()
        {
            return _context.Reviews.Where(x => !x.IsDeleted).ToList();
        }

        public void Insert(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }
    }
}
