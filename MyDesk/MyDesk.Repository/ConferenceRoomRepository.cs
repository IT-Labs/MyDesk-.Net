using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data;
using MyDesk.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyDesk.Repository
{
    public class ConferenceRoomRepository : IConferenceRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public ConferenceRoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ConferenceRoom Get(int id,
            bool? includeReservations = null,
            bool? includeReviews = null)
        {
            IQueryable<ConferenceRoom> query = _context.ConferenceRooms.Where(x => x.Id == id && x.IsDeleted == false);

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

        public List<ConferenceRoom> GetOfficeConferenceRooms(int officeId,
            bool? includeReservations = null,
            int? take = null,
            int? skip = null)
        {
            IQueryable<ConferenceRoom> query = _context.ConferenceRooms.Where(x => x.OfficeId == officeId && x.IsDeleted == false);

            if (includeReservations == true)
            {
                query = query.Include(x => x.Reservations.Where(y => y.IsDeleted == false));
            }
            if (take.HasValue && skip.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.ToList();
        }

        public void SoftDelete(ConferenceRoom conferenceRoom)
        {
            conferenceRoom.IsDeleted = true;
            _context.ConferenceRooms.Update(conferenceRoom);
            _context.SaveChanges();
        }
    }
}
