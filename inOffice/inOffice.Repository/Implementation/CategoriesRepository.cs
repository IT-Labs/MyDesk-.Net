using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Implementation
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Categories GetDeskCategories(int deskId)
        {
            return _context.Categories.FirstOrDefault(x => x.DeskId == deskId && !x.IsDeleted);
        }

        public void Insert(Categories categories)
        {
            _context.Categories.Add(categories);
            _context.SaveChanges();
        }

        public void Update(Categories categories)
        {
            _context.Categories.Update(categories);
            _context.SaveChanges();
        }
    }
}
