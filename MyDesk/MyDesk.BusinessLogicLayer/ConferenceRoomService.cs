using AutoMapper;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;
using MyDesk.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace MyDesk.BusinessLogicLayer
{
    public class ConferenceRoomService : IConferenceRoomService
    {
        private readonly IMapper _mapper;
        private readonly IContext _context;

        public ConferenceRoomService(IMapper mapper, IContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public List<ConferenceRoomDto> GetOfficeConferenceRooms(int id, int? take = null, int? skip = null)
        {
            var query = _context
                .AsQueryable<ConferenceRoom>()
                .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                .Where(x => x.Id == id && x.IsDeleted == false);

            var conferenceRooms = (take.HasValue && skip.HasValue) ?
                query.Skip(skip.Value).Take(take.Value).ToList() : 
                query.ToList();

            return _mapper.Map<List<ConferenceRoomDto>>(conferenceRooms);
        }

        public void Delete(int id)
        {
            var conferenceRoom = _context
                .AsQueryable<ConferenceRoom>()
                .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                    .ThenInclude(x => x.Reviews.Where(y => y.IsDeleted == false))
                .FirstOrDefault(x => x.Id == id && x.IsDeleted == false);

            if (conferenceRoom == null)
            {
                throw new NotFoundException($"Conference room with ID: {id} not found.");
            }

            foreach (Reservation reservation in conferenceRoom.Reservations)
            {
                reservation.IsDeleted = true;
                foreach (Review review in reservation.Reviews)
                {
                    review.IsDeleted = true;
                }
            }

            conferenceRoom.IsDeleted = true;
            _context.Modify(conferenceRoom);
        }
    }
}
