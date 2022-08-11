using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace inOffice.Repository.Implementation
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public Reservation Get(int id, 
            bool? includeDesk = null,
            bool? includeOffice = null,
            bool? includeonferenceRoom = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.Id == id && !x.IsDeleted);

            if (includeDesk.HasValue && !includeOffice.HasValue)
            {
                query = query.Include(x => x.Desk);
            }
            if (includeDesk.HasValue && includeOffice.HasValue)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }
            if (includeonferenceRoom.HasValue)
            {
                query = query.Include(x => x.ConferenceRoom);
            }

            return query.FirstOrDefault();
        }

        public List<Reservation> GetAll(bool? includeEmployee = null, 
            bool? includeDesk = null,
            bool? includeOffice = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => !x.IsDeleted);

            if (includeEmployee.HasValue)
            {
                query = query.Include(x => x.Employee);
            }
            if (includeDesk.HasValue && !includeOffice.HasValue)
            {
                query = query.Include(x => x.Desk);
            }
            if (includeDesk.HasValue && includeOffice.HasValue)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }

            List<Reservation> result = query.ToList();
            foreach (Reservation reservation in result)
            {
                if (reservation.Employee != null && reservation.Employee.IsDeleted)
                {
                    reservation.Employee = null;
                }
                if (reservation.Desk != null && reservation.Desk.IsDeleted)
                {
                    reservation.Desk = null;
                }
                else if (reservation.Desk != null && !reservation.Desk.IsDeleted && reservation.Desk.Office != null && reservation.Desk.Office.IsDeleted)
                {
                    reservation.Desk.Office = null;
                }
            }

            return result;
        }

        public List<Reservation> GetEmployeeReservations(int employeeId,
            bool? includeDesk = null,
            bool? includeOffice = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.EmployeeId == employeeId && !x.IsDeleted);

            if (includeDesk.HasValue && !includeOffice.HasValue)
            {
                query = query.Include(x => x.Desk);
            }
            if (includeDesk.HasValue && includeOffice.HasValue)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }

            return query.ToList();
        }

        public List<Reservation> GetDeskReservations(int deskId, 
            bool? includeReview = null,
            bool? includeEmployee = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.DeskId == deskId && !x.IsDeleted);

            if (includeReview.HasValue)
            {
                query = query.Include(x => x.Review);
            }
            if (includeEmployee.HasValue)
            {
                query = query.Include(x => x.Employee);
            }

            return query.ToList();
        }

        public void Insert(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        public void Update(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }
        
        public void Delete(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
        }
    }
}
