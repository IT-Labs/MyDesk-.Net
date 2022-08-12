using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace inOffice.Repository.Implementation
{
    public class ConferenceRoomRepository: IConferenceRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public ConferenceRoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ConferenceRoom Get(int id, bool? includeReservation = null)
        {
            IQueryable<ConferenceRoom> query = _context.ConferenceRooms.Where(x => x.Id == id && !x.IsDeleted);

            if (includeReservation.HasValue)
            {
                query = query.Include(x => x.Reservation);
            }

            return query.FirstOrDefault();
        }

        public List<ConferenceRoom> GetOfficeConferenceRooms(int officeId)
        {
            return _context.ConferenceRooms.Where(x => x.OfficeId == officeId && !x.IsDeleted).ToList();
        }

        public void Update(ConferenceRoom conferenceRoom)
        {
            _context.ConferenceRooms.Update(conferenceRoom);
            _context.SaveChanges();
        }

        public void BulkUpdate(List<ConferenceRoom> conferenceRooms)
        {
            _context.ConferenceRooms.UpdateRange(conferenceRooms);
            _context.SaveChanges();
        }

        public void Delete(ConferenceRoom conferenceRoom)
        {
            _context.ConferenceRooms.Remove(conferenceRoom);
            _context.SaveChanges();
        }
    }
}
