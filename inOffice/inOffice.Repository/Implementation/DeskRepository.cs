using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Implementation
{
    public class DeskRepository : IDeskRepository
    {
        private readonly ApplicationDbContext _context;

        public DeskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Desk Get(int id)
        {
            return _context.Desks.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<Desk> GetOfficeDesks(int officeId, int? take = null, int? skip = null)
        {
            IQueryable<Desk> query = _context.Desks.Where(x => x.OfficeId == officeId && !x.IsDeleted);

            if (take.HasValue && skip.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.ToList();
        }

        public void BulkInsert(List<Desk> desks)
        {
            _context.Desks.AddRange(desks);
            _context.SaveChanges();
        }

        public void Update(Desk desk)
        {
            _context.Desks.Update(desk);
            _context.SaveChanges();
        }

        public void SoftDelete(Desk desk)
        {
            desk.IsDeleted = true;
            _context.Desks.Update(desk);
            _context.SaveChanges();
        }
    }
}
