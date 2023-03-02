using AutoMapper;
using MyDesk.Core.DTO;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;
using MyDesk.Core.Database;
using Microsoft.EntityFrameworkCore;
using MyDesk.Core.Interfaces.BusinessLogic;

namespace MyDesk.BusinessLogicLayer
{
    public class OfficeService : IOfficeService
    {
        private readonly IMapper _mapper;
        private readonly IContext _context;

        public OfficeService(IMapper mapper, IContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public void CreateNewOffice(OfficeDto officeDto)
        {
            if (string.IsNullOrEmpty(officeDto?.Name))
            {
                throw new ConflictException("Office name cannot be empty");
            }

            var existingOffice = GetOfficeByName(officeDto.Name);
            if (existingOffice != null)
            {
                throw new ConflictException("There is already office with the same name");
            }

            var office = new Office()
            {
                Name = officeDto.Name,
                OfficeImage = officeDto.OfficeImage
            };

            _context.Insert(office);
        }

        public void UpdateOffice(OfficeDto officeDto)
        {
            if (officeDto?.Id == null)
            {
                throw new NotFoundException($"Office Id is not provided");
            }

            if (officeDto?.Name == null)
            {
                throw new NotFoundException($"Office Id is not provided");
            }

            var office = GetOfficeById(officeDto.Id.Value);

            if (office == null)
            {
                throw new NotFoundException($"Office with ID: {officeDto.Id} not found.");
            }

            var existingOffice = GetOfficeByName(officeDto.Name);
            if (existingOffice != null && existingOffice.Id != officeDto?.Id)
            {
                throw new ConflictException("There is already office with the same name.");
            }

            office.Name = officeDto?.Name;
            office.OfficeImage = officeDto?.OfficeImage;

            _context.Modify(office);
        }

        public void DeleteOffice(int id)
        {
            var office1 = _context
                .AsQueryable<Office>();
            var office = office1
                .Include(x => x.Desks.Where(y => y.IsDeleted == false))
                .Include(x => x.ConferenceRooms.Where(y => y.IsDeleted == false))
                .FirstOrDefault(x => x.Id == id && x.IsDeleted == false);

            if (office == null)
            {
                throw new NotFoundException($"Office with ID: {id} not found.");
            }

            for (int i = 0; i < office.Desks.Count; i++)
            {
                var officeDesk = office.Desks.ElementAt(i);
                var desk = _context
                    .AsQueryable<Desk>()
                    .Where(x => x.Id == officeDesk.Id && x.IsDeleted == false)
                    .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                        .ThenInclude(x => x.Reviews.Where(y => y.IsDeleted == false))
                    .FirstOrDefault();

                if (desk != null)
                {
                    desk.IsDeleted = true;
                    MarkAsSoftDeleted(desk.Reservations);
                }
            }

            for (int i = 0; i < office.ConferenceRooms.Count; i++)
            {
                var officeConferenceRoom = office.ConferenceRooms.ElementAt(i);
                var conferenceRoom = _context
                    .AsQueryable<ConferenceRoom>()
                    .Where(x => x.Id == officeConferenceRoom.Id && x.IsDeleted == false)
                    .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                        .ThenInclude(x => x.Reviews.Where(y => y.IsDeleted == false))
                    .FirstOrDefault();

                if (conferenceRoom != null)
                {
                    conferenceRoom.IsDeleted = true;
                    MarkAsSoftDeleted(conferenceRoom.Reservations);
                }
            }

            office.IsDeleted = true;
            _context.Modify(office);
        }

        public List<OfficeDto> GetAllOffices(int? take = null, int? skip = null)
        {
           var query = _context
                .AsQueryable<Office>()
                .Where(x => x.IsDeleted == false);

            var offices = (take.HasValue && skip.HasValue) ?
                query.Skip(skip.Value).Take(take.Value).ToList() :
                query.ToList();

            var officeDtos = _mapper.Map<List<OfficeDto>>(offices);
            return officeDtos;
        }

        public OfficeDto GetDetailsForOffice(int id)
        {
            var office = GetOfficeById(id);
            if (office == null)
            {
                throw new NotFoundException($"Office with ID: {id} not found.");
            }

            var officeDto = _mapper.Map<OfficeDto>(office);
            return officeDto;
        }

        private static void MarkAsSoftDeleted(ICollection<Reservation> reservations)
        {
            foreach (var reservation in reservations)
            {
                reservation.IsDeleted = true;
                foreach (var review in reservation.Reviews)
                {
                    review.IsDeleted = true;
                }
            }
        }

        private Office? GetOfficeById(int id)
        {
            var office = _context
                .AsQueryable<Office>()
                .FirstOrDefault(x => x.Id == id && x.IsDeleted == false);

            return office;
        }

        private Office? GetOfficeByName(string name)
        {
            var office = _context
                .AsQueryable<Office>()
                .FirstOrDefault(x => x.Name == name && x.IsDeleted == false);

            return office;
        }
    }
}
