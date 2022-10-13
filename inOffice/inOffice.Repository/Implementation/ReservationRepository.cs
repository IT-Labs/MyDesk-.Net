using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Entities;
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
            bool? includeonferenceRoom = null,
            bool? includeReviews = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.Id == id && x.IsDeleted == false);

            if (includeDesk == true && includeOffice != true)
            {
                query = query.Include(x => x.Desk);
            }
            else if (includeDesk == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }

            if (includeonferenceRoom == true && includeOffice != true)
            {
                query = query.Include(x => x.ConferenceRoom);
            }
            else if (includeonferenceRoom == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.ConferenceRoom)
                    .ThenInclude(x => x.Office);
            }

            if (includeReviews == true)
            {
                query = query.Include(x => x.Reviews.Where(y => y.IsDeleted == false));
            }

            return query.FirstOrDefault();
        }

        public Tuple<int?, List<Reservation>> GetAll(bool? includeEmployee = null,
            bool? includeDesk = null,
            bool? includeOffice = null,
            int? take = null,
            int? skip = null)
        {
            int? totalCount = null;
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.IsDeleted == false);

            if (includeEmployee == true)
            {
                query = query.Include(x => x.Employee);
            }
            if (includeDesk == true && includeOffice != true)
            {
                query = query.Include(x => x.Desk);
            }
            if (includeDesk == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }
            if (take.HasValue && skip.HasValue)
            {
                totalCount = query.Count();
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return Tuple.Create(totalCount, query.ToList());
        }

        public List<Reservation> GetEmployeeReservations(int employeeId,
            bool? includeDesk = null,
            bool? includeConferenceRoom = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.EmployeeId == employeeId && x.IsDeleted == false && (x.StartDate >= DateTime.Now.Date || x.EndDate >= DateTime.Now.Date));

            if (includeDesk == true)
            {
                query = query.Include(x => x.Desk);
            }

            if (includeConferenceRoom == true)
            {
                query = query.Include(x => x.ConferenceRoom);
            }

            return query.ToList();
        }

        public List<Reservation> GetEmployeeFutureReservations(int employeeId,
            bool? includeDesk = null,
            bool? includeConferenceRoom = null,
            bool? includeOffice = null,
            int? take = null,
            int? skip = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.EmployeeId == employeeId && x.IsDeleted == false && x.StartDate > DateTime.Now.Date && x.EndDate > DateTime.Now.Date);

            if (includeDesk == true && includeOffice != true)
            {
                query = query.Include(x => x.Desk);
            }
            else if (includeDesk == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }

            if (includeConferenceRoom == true && includeOffice != true)
            {
                query = query.Include(x => x.ConferenceRoom);
            }
            else if (includeConferenceRoom == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.ConferenceRoom)
                    .ThenInclude(x => x.Office);
            }

            if (take.HasValue && skip.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.ToList();
        }

        public List<Reservation> GetEmployeePastReservations(int employeeId,
            bool? includeDesk = null,
            bool? includeConferenceRoom = null,
            bool? includeOffice = null,
            bool? includeReviews = null,
            int? take = null,
            int? skip = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.EmployeeId == employeeId && x.IsDeleted == false && x.StartDate < DateTime.Now.Date && x.EndDate < DateTime.Now.Date);

            if (includeDesk == true && includeOffice != true)
            {
                query = query.Include(x => x.Desk);
            }
            else if (includeDesk == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }

            if (includeConferenceRoom == true && includeOffice != true)
            {
                query = query.Include(x => x.ConferenceRoom);
            }
            else if (includeConferenceRoom == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.ConferenceRoom)
                    .ThenInclude(x => x.Office);
            }

            if (includeReviews == true)
            {
                query = query.Include(x => x.Reviews.Where(y => y.IsDeleted == false));
            }

            if (take.HasValue && skip.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.ToList();
        }

        public List<Reservation> GetDeskReservations(int deskId, bool? includeEmployee = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.DeskId == deskId && x.IsDeleted == false && (x.StartDate >= DateTime.Now.Date || x.EndDate >= DateTime.Now.Date));

            if (includeEmployee == true)
            {
                query = query.Include(x => x.Employee);
            }

            return query.ToList();
        }

        public List<Reservation> GetPastDeskReservations(int deskId, bool? includeReview = null)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(x => x.DeskId == deskId && x.IsDeleted == false && x.StartDate < DateTime.Now.Date && x.EndDate < DateTime.Now.Date);

            if (includeReview == true)
            {
                query = query.Include(x => x.Reviews.Where(y => y.IsDeleted == false));
            }

            return query.ToList();
        }

        public void Insert(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        public void SoftDelete(Reservation reservation)
        {
            reservation.IsDeleted = true;
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }
    }
}
