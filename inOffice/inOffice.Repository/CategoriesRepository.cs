using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Entities;

namespace inOffice.Repository
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Category Get(bool? doubleMonitor, bool? nearWindow, bool? singleMonitor, bool? unavailable)
        {
            return _context.Categories.FirstOrDefault(x => x.DoubleMonitor == doubleMonitor && x.NearWindow == nearWindow &&
            x.SingleMonitor == singleMonitor && x.Unavailable == unavailable && x.IsDeleted == false);
        }

        public void Insert(Category categories)
        {
            _context.Categories.Add(categories);
            _context.SaveChanges();
        }

        public void Update(Category categories)
        {
            _context.Categories.Update(categories);
            _context.SaveChanges();
        }
    }
}
