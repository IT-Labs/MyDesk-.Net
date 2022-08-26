using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
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

        public AllReviewsForEntity AllReviewsForEntity(int id)
        {
            List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(id, includeReview: true);

            return new AllReviewsForEntity()
            {
                AllReviews = deskReservations.SelectMany(x => x.Reviews.Select(y => y.Reviews)).ToList(),
                Success = true
            };
        }

        public EntitiesResponse CreateNewDesks(int officeId, int numberOfInstancesToCreate)
        {
            EntitiesResponse response = new EntitiesResponse();

            Office existinOffice = _officeRepository.Get(officeId);
            if (existinOffice == null)
            {
                response.Success = false;
                return response;
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

            response.Success = true;

            return response;
        }

        public DesksResponse ListAllDesks(int id, int? take = null, int? skip = null)
        {
            DesksResponse responseDeskList = new DesksResponse();
            List<DeskDto> list = new List<DeskDto>();

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

                DeskDto custom = new DeskDto()
                {
                    Id = desk.Id,
                    IndexForOffice = desk.IndexForOffice,
                    Reservations = _mapper.Map<List<ReservationDto>>(deskReservations)
                };

                if (desk.Categorie != null)
                {
                    custom.Categories = _mapper.Map<CategoryDto>(desk.Categorie);
                }

                list.Add(custom);
            }

            responseDeskList.sucess = true;
            responseDeskList.DeskList = list;

            return responseDeskList;
        }

        public DeleteResponse DeleteEntity(DeleteRequest request)
        {
            DeleteResponse deleteResponse = new DeleteResponse();

            if (request.TypeOfEntity == "D")
            {
                Desk desk = _deskRepository.Get(request.IdOfEntity, includeReservations: true, includeReviews: true);

                if (desk == null)
                {
                    deleteResponse.Success = false;
                    return deleteResponse;
                }

                MarkReservationsAsDeleted(desk.Reservations);

                _deskRepository.SoftDelete(desk);
            }
            else
            {
                ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(request.IdOfEntity, includeReservations: true, includeReviews: true);

                if (conferenceRoom == null)
                {
                    deleteResponse.Success = false;
                    return deleteResponse;
                }

                MarkReservationsAsDeleted(conferenceRoom.Reservations);

                _conferenceRoomRepository.SoftDelete(conferenceRoom);
            }

            deleteResponse.Success = true;

            return deleteResponse;
        }

        public ConferenceRoomsResponse ListAllConferenceRooms(int id, int? take = null, int? skip = null)
        {
            List<ConferenceRoom> conferenceRooms = _conferenceRoomRepository.GetOfficeConferenceRooms(id, includeReservations: true, take: take, skip: skip);

            return new ConferenceRoomsResponse()
            {
                ConferenceRoomsList = _mapper.Map<List<ConferenceRoomDto>>(conferenceRooms),
                Sucess = true
            };
        }

        public EntitiesResponse UpdateDesks(UpdateRequest request)
        {
            EntitiesResponse entitiesResponse = new EntitiesResponse();

            using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (DeskToUpdate deskToUpdate in request.ListOfDesksToUpdate)
                {
                    Desk desk = _deskRepository.Get(deskToUpdate.DeskId);

                    if (desk == null)
                    {
                        continue;
                    }

                    Category existingDeskCategory = _categoriesRepository.Get(deskToUpdate.DualMonitor, deskToUpdate.NearWindow, deskToUpdate.SingleMonitor, deskToUpdate.Unavailable);
                    if (existingDeskCategory != null)
                    {
                        desk.CategorieId = existingDeskCategory.Id;
                        _deskRepository.Update(desk);
                    }
                    else
                    {
                        Category category = new Category()
                        {
                            DoubleMonitor = deskToUpdate.DualMonitor,
                            SingleMonitor = deskToUpdate.SingleMonitor,
                            NearWindow = deskToUpdate.NearWindow,
                            Unavailable = deskToUpdate.Unavailable
                        };

                        category.Desks.Add(desk);
                        _categoriesRepository.Insert(category);
                    }
                }

                transaction.Complete();
            }

            entitiesResponse.Success = true;

            return entitiesResponse;
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
