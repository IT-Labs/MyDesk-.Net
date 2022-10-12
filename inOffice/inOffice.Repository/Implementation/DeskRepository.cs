using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace inOffice.Repository.Implementation
{
    public class DeskRepository : IDeskRepository
    {
        private readonly ApplicationDbContext _context;

        public DeskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Desk Get(int id, bool? includeReservations = null, bool? includeReviews = null)
        {
            IQueryable<Desk> query = _context.Desks.Where(x => x.Id == id && x.IsDeleted == false);

            if (includeReservations == true && includeReviews != true)
            {
                query = query.Include(x => x.Reservations.Where(y => y.IsDeleted == false));
            }
            else if (includeReservations == true && includeReviews == true)
            {
                query = query
                    .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                    .ThenInclude(x => x.Reviews.Where(y => y.IsDeleted == false));
            }

            return query.FirstOrDefault();
        }

        public int GetHighestDeskIndexForOffice(int officeId)
        {
            int? result = _context.Desks.Where(x => x.OfficeId == officeId && x.IsDeleted == false).OrderByDescending(x => x.IndexForOffice).Select(x => x.IndexForOffice).FirstOrDefault();
            return result.HasValue ? result.Value : 0;
        }

        public List<Desk> GetOfficeDesks(int officeId, bool? includeCategory = null, bool? includeReservations = null, bool? includeEmployees = null, int? take = null, int? skip = null)
        {
            IQueryable<Desk> query = _context.Desks.Where(x => x.OfficeId == officeId && x.IsDeleted == false);

            if (includeCategory == true)
            {
                query = query.Include(x => x.Categorie);
            }

            if (includeReservations == true && includeEmployees != true)
            {
                query = query.Include(x => x.Reservations.Where(y => y.IsDeleted == false));
            }
            else if (includeReservations == true && includeEmployees == true)
            {
                query = query
                    .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                    .ThenInclude(x => x.Employee);
            }

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
