using AutoMapper;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data.DTO;
using MyDesk.Data.Entities;
using MyDesk.Data.Exceptions;

namespace MyDesk.BusinessLogicLayer
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

        public void CreateNewOffice(OfficeDto officeDto)
        {
            if (string.IsNullOrEmpty(officeDto?.Name))
            {
                throw new ConflictException("Office name cannot be empty");
            }

            Office existingOffice = _officeRepository.GetByName(officeDto.Name);

            if (existingOffice != null)
            {
                throw new ConflictException("There is allready office with the same name");
            }

            var office = new Office()
            {
                Name = officeDto.Name,
                OfficeImage = officeDto.OfficeImage
            };

            _officeRepository.Insert(office);
        }

        public void UpdateOffice(OfficeDto officeDto)
        {

            if (officeDto?.Id == null)
            {
                throw new NotFoundException($"Cannot update null office");
            }

            Office office = _officeRepository.Get(officeDto.Id.Value);

            if (office == null)
            {
                throw new NotFoundException($"Office with ID: {officeDto.Id} not found.");
            }

            Office existingOffice = _officeRepository.GetByName(officeDto?.Name??String.Empty);

            if (existingOffice != null && existingOffice.Id != officeDto?.Id)
            {
                throw new ConflictException("There is already office with the same name.");
            }

            office.Name = officeDto?.Name;
            office.OfficeImage = officeDto?.OfficeImage;

            _officeRepository.Update(office);
        }

        public void DeleteOffice(int id)
        {
            Office office = _officeRepository.Get(id, includeDesks: true, includeConferenceRooms: true);

            if (office == null)
            {
                throw new NotFoundException($"Office with ID: {id} not found.");
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
        }

        public List<OfficeDto> GetAllOffices(int? take = null, int? skip = null)
        {
            List<Office> offices = _officeRepository.GetAll(take: take, skip: skip);
            List<OfficeDto> officeDtos = _mapper.Map<List<OfficeDto>>(offices);

            return officeDtos;
        }

        public OfficeDto GetDetailsForOffice(int id)
        {
            Office office = _officeRepository.Get(id);
            if (office == null)
            {
                throw new NotFoundException($"Office with ID: {id} not found.");
            }

            OfficeDto officeDto = _mapper.Map<OfficeDto>(office);

            return officeDto;
        }

        private static void MarkAsSoftDeleted(ICollection<Reservation> reservations)
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
