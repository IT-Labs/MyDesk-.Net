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

        public List<Review> GetAll(int? take = null, int? skip = null)
        {
            IQueryable<Review> query = _context.Reviews.Where(x => !x.IsDeleted);

            if (take.HasValue && skip.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.ToList();
        }

        public void Insert(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }
    }
}
