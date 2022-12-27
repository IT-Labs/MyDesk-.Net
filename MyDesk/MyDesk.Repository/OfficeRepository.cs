using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data;
using MyDesk.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyDesk.Repository
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly ApplicationDbContext _context;

        public OfficeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Office Get(int id,
            bool? includeDesks = null,
            bool? includeConferenceRooms = null)
        {
            IQueryable<Office> query = _context.Offices.Where(x => x.Id == id && x.IsDeleted == false);

            if (includeDesks == true)
            {
                query = query.Include(x => x.Desks.Where(y => y.IsDeleted == false));
            }

            if (includeConferenceRooms == true)
            {
                query = query.Include(x => x.ConferenceRooms.Where(y => y.IsDeleted == false));
            }

            return query.FirstOrDefault();
        }

        public Office GetByName(string name)
        {
            return _context.Offices.FirstOrDefault(x => x.Name == name && x.IsDeleted == false);
        }

        public List<Office> GetAll(int? take = null, int? skip = null)
        {
            IQueryable<Office> query = _context.Offices.Where(x => x.IsDeleted == false);

            if (take.HasValue && skip.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.ToList();
        }

        public void Insert(Office office)
        {
            _context.Offices.Add(office);
            _context.SaveChanges();
        }

        public void Update(Office office)
        {
            _context.Offices.Update(office);
            _context.SaveChanges();
        }

        public void SoftDelete(Office office)
        {
            office.IsDeleted = true;
            _context.Offices.Update(office);
            _context.SaveChanges();
        }
    }
}
