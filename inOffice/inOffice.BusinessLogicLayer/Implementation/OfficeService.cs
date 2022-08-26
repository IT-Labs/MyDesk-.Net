using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _officeRepository;
        private readonly IDeskRepository _deskRepository;
        private readonly IConferenceRoomRepository _conferenceRoomRepository;
        private readonly IMapper _mapper;

        public OfficeService(IOfficeRepository officeRepository,
            IDeskRepository deskRepository,
            IConferenceRoomRepository conferenceRoomRepository,
            IMapper mapper)
        {
            _officeRepository = officeRepository;
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;
            _mapper = mapper;
        }

        public OfficeResponse CreateNewOffice(NewOfficeRequest request)
        {
            OfficeResponse response = new OfficeResponse();
            Office office = new Office()
            {
                Name = request.OfficeName,
                OfficeImage = string.Empty
            };

            Office existingOffice = _officeRepository.GetByName(office.Name);

            if (existingOffice != null)
            {
                response.Success = false;
            }
            else
            {
                _officeRepository.Insert(office);
                response.Success = true;
            }

            return response;
        }

        public OfficeResponse UpdateOffice(OfficeRequest request)
        {
            OfficeResponse response = new OfficeResponse();

            Office office = _officeRepository.Get(request.Id);

            if (office == null)
            {
                response.Success = false;
                return response;
            }

            office.Name = request.OfficeName;
            office.OfficeImage = request.OfficePlan;

            _officeRepository.Update(office);
            response.Success = true;

            return response;
        }

        public OfficeResponse DeleteOffice(int id)
        {
            OfficeResponse response = new OfficeResponse();

            Office office = _officeRepository.Get(id, includeDesks: true, includeConferenceRooms: true);

            if (office == null)
            {
                response.Success = false;
                return response;
            }

            for (int i = 0; i < office.Desks.Count; i++)
            {
                Desk desk = office.Desks.ElementAt(i);
                desk = _deskRepository.Get(desk.Id, includeReservations: true, includeReviews: true);
                desk.IsDeleted = true;

                MarkAsSoftDeleted(desk.Reservations);
            }

            for (int i = 0; i < office.ConferenceRooms.Count; i++)
            {
                ConferenceRoom conferenceRoom = office.ConferenceRooms.ElementAt(i);
                conferenceRoom = _conferenceRoomRepository.Get(conferenceRoom.Id, includeReservations: true, includeReviews: true);
                conferenceRoom.IsDeleted = true;

                MarkAsSoftDeleted(conferenceRoom.Reservations);
            }

            _officeRepository.SoftDelete(office);
            response.Success = true;

            return response;
        }

        public OfficeListResponse GetAllOffices(int? take = null, int? skip = null)
        {
            List<Office> offices = _officeRepository.GetAll(take: take, skip: skip);

            return new OfficeListResponse()
            {
                Offices = _mapper.Map<List<OfficeDto>>(offices),
                Success = true
            };
        }

        public Office GetDetailsForOffice(int id)
        {
            return _officeRepository.Get(id);
        }

        private void MarkAsSoftDeleted(ICollection<Reservation> reservations)
        {
            foreach (Reservation reservation in reservations)
            {
                reservation.IsDeleted = true;
                foreach (Review review in reservation.Reviews)
                {
                    review.IsDeleted = true;
                }
            }
        }
    }
}
