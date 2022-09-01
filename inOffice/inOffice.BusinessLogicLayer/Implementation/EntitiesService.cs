using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using inOfficeApplication.Data.Utils;
using System.Transactions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class EntitiesService : IEntitiesService
    {
        private readonly IDeskRepository _deskRepository;
        private readonly IConferenceRoomRepository _conferenceRoomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IMapper _mapper;

        public EntitiesService(IDeskRepository deskRepository,
            IConferenceRoomRepository conferenceRoomRepository,
            IReservationRepository reservationRepository,
            ICategoriesRepository categoriesRepository,
            IOfficeRepository officeRepository,
            IMapper mapper)
        {
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;
            _reservationRepository = reservationRepository;
            _categoriesRepository = categoriesRepository;
            _officeRepository = officeRepository;
            _mapper = mapper;
        }

        public List<ReviewDto> AllReviewsForEntity(int id)
        {
            List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(id, includeReview: true);
            List<Review> reviews = deskReservations.SelectMany(x => x.Reviews).ToList();

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public void CreateNewDesks(int officeId, int numberOfInstancesToCreate)
        {
            Office existinOffice = _officeRepository.Get(officeId);
            if (existinOffice == null)
            {
                throw new NotFoundException($"Office with ID: {officeId} not found.");
            }

            int highestIndex = _deskRepository.GetHighestDeskIndexForOffice(officeId);

            List<Desk> desksToInsert = new List<Desk>();

            for (int i = 0; i < numberOfInstancesToCreate; i++)
            {
                Desk desk = new Desk()
                {
                    OfficeId = officeId,
                    IsDeleted = false,
                    Categories = "regular",
                    IndexForOffice = highestIndex + i + 1
                };

                desksToInsert.Add(desk);
            }

            _deskRepository.BulkInsert(desksToInsert);
        }

        public List<DeskDto> ListAllDesks(int id, int? take = null, int? skip = null)
        {
            List<DeskDto> result = new List<DeskDto>();

            List<Desk> desks = _deskRepository.GetOfficeDesks(id, includeCategory: true, take: take, skip: skip);

            foreach (Desk desk in desks)
            {
                List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(desk.Id, includeEmployee: true).Where(x => x.StartDate > DateTime.Today).ToList();

                deskReservations.ForEach(x =>
                {
                    if (x.Employee != null)
                    {
                        x.Employee.Password = null;
                    }
                });

                DeskDto deskDto = _mapper.Map<DeskDto>(desk);

                result.Add(deskDto);
            }

            return result;
        }

        public void DeleteEntity(DeleteRequest request)
        {
            if (request.Type == (int)EntityTypes.Desk)
            {
                Desk desk = _deskRepository.Get(request.Id.Value, includeReservations: true, includeReviews: true);

                if (desk == null)
                {
                    throw new NotFoundException($"Desk with ID: {request.Id} not found.");
                }

                MarkReservationsAsDeleted(desk.Reservations);

                _deskRepository.SoftDelete(desk);
            }
            else if (request.Type == (int)EntityTypes.ConferenceRoom)
            {
                ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(request.Id.Value, includeReservations: true, includeReviews: true);

                if (conferenceRoom == null)
                {
                    throw new NotFoundException($"Conference room with ID: {request.Id} not found.");
                }

                MarkReservationsAsDeleted(conferenceRoom.Reservations);

                _conferenceRoomRepository.SoftDelete(conferenceRoom);
            }
        }

        public List<ConferenceRoomDto> ListAllConferenceRooms(int id, int? take = null, int? skip = null)
        {
            List<ConferenceRoom> conferenceRooms = _conferenceRoomRepository.GetOfficeConferenceRooms(id, includeReservations: true, take: take, skip: skip);
            return _mapper.Map<List<ConferenceRoomDto>>(conferenceRooms);
        }

        public void UpdateDesks(List<DeskDto> desks)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (DeskDto deskToUpdate in desks)
                {
                    Desk desk = _deskRepository.Get(deskToUpdate.Id.Value);

                    if (desk == null)
                    {
                        continue;
                    }

                    Category existingDeskCategory = _categoriesRepository.Get(deskToUpdate.Category.DoubleMonitor, deskToUpdate.Category.NearWindow,
                        deskToUpdate.Category.SingleMonitor, deskToUpdate.Category.Unavailable);
                    if (existingDeskCategory != null)
                    {
                        desk.CategorieId = existingDeskCategory.Id;
                        _deskRepository.Update(desk);
                    }
                    else
                    {
                        Category category = new Category()
                        {
                            DoubleMonitor = deskToUpdate.Category.DoubleMonitor,
                            SingleMonitor = deskToUpdate.Category.SingleMonitor,
                            NearWindow = deskToUpdate.Category.NearWindow,
                            Unavailable = deskToUpdate.Category.Unavailable
                        };

                        category.Desks.Add(desk);
                        _categoriesRepository.Insert(category);
                    }
                }

                transaction.Complete();
            }
        }

        private void MarkReservationsAsDeleted(ICollection<Reservation> reservations)
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
