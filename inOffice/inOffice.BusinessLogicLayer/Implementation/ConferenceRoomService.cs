using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class ConferenceRoomService : IConferenceRoomService
    {
        private readonly IConferenceRoomRepository _conferenceRoomRepository;
        private readonly IMapper _mapper;

        public ConferenceRoomService(IConferenceRoomRepository conferenceRoomRepository, IMapper mapper)
        {
            _conferenceRoomRepository = conferenceRoomRepository;
            _mapper = mapper;
        }

        public List<ConferenceRoomDto> GetOfficeConferenceRooms(int id, int? take = null, int? skip = null)
        {
            List<ConferenceRoom> conferenceRooms = _conferenceRoomRepository.GetOfficeConferenceRooms(id, includeReservations: true, take: take, skip: skip);
            return _mapper.Map<List<ConferenceRoomDto>>(conferenceRooms);
        }

        public void Delete(int id)
        {
            ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(id, includeReservations: true, includeReviews: true);

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

            _conferenceRoomRepository.SoftDelete(conferenceRoom);
        }
    }
}
