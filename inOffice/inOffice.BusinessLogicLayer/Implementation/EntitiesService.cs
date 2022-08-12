using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using System.Transactions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class EntitiesService : IEntitiesService
    {
        private readonly IDeskRepository _deskRepository;
        private readonly IConferenceRoomRepository _conferenceRoomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public EntitiesService(IDeskRepository deskRepository,
            IConferenceRoomRepository conferenceRoomRepository,
            IReservationRepository reservationRepository,
            ICategoriesRepository categoriesRepository)
        {
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;
            _reservationRepository = reservationRepository;
            _categoriesRepository = categoriesRepository;
        }

        public AllReviewsForEntity AllReviewsForEntity(int id)
        {
            AllReviewsForEntity response = new AllReviewsForEntity();

            List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(id, includeReview: true);

            response.AllReviews = deskReservations.Where(x => x.Review != null).Select(x => x.Review.Reviews).ToList();
            response.Success = response.AllReviews.Count > 0;

            return response;
        }

        public EntitiesResponse CreateNewDesks(EntitiesRequest request)
        {
            EntitiesResponse response = new EntitiesResponse();

            int deskCount = _deskRepository.GetOfficeDesks(request.Id).Count();

            try
            {
                List<Desk> desksToInsert = new List<Desk>();

                for (int i = 0; i < request.NumberOfDesks; i++)
                {
                    Desk desk = new Desk()
                    {
                        OfficeId = request.Id,
                        IsDeleted = false,
                        Categories = "regular",
                        IndexForOffice = deskCount + i + 1
                    };

                    desksToInsert.Add(desk);
                }

                _deskRepository.BulkInsert(desksToInsert);

                response.Success = true;
            }
            catch (Exception _)
            {
                response.Success = false;
            }

            return response;
        }

        public DesksResponse ListAllDesks(int id)
        {
            DesksResponse responseDeskList = new DesksResponse();
            List<DeskCustom> list = new List<DeskCustom>();
            try
            {
                List<Desk> desks = _deskRepository.GetOfficeDesks(id);

                foreach (Desk desk in desks)
                {
                    List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(desk.Id, includeEmployee: true).Where(x => x.StartDate > DateTime.Today).ToList();

                    deskReservations.ForEach(x => x.Employee?.Reservations?.Clear());
                    deskReservations.ForEach(x =>
                    {
                        if (x.Employee != null)
                        {
                            x.Employee.Password = null;
                        }
                    });

                    DeskCustom custom = new DeskCustom()
                    {
                        Id = desk.Id,
                        IndexForOffice = desk.IndexForOffice,
                        Reservations = deskReservations
                    };

                    Categories? findCategories = _categoriesRepository.GetDeskCategories(desk.Id);
                    custom.Categories = findCategories;
                    list.Add(custom);
                }

                responseDeskList.sucess = true;
                responseDeskList.DeskList = list;

                return responseDeskList;
            }
            catch (Exception _)
            {
                responseDeskList.sucess = false;
                return responseDeskList;
            }
        }

        public DeleteResponse DeleteEntity(DeleteRequest request)
        {
            DeleteResponse deleteResponse = new DeleteResponse();

            try
            {
                if (request.TypeOfEntity == "D")
                {
                    Desk desk = _deskRepository.Get(request.IdOfEntity);

                    if (desk == null)
                    {
                        deleteResponse.Success = false;
                        return deleteResponse;
                    }

                    _deskRepository.SoftDelete(desk);

                    deleteResponse.Success = true;
                }
                else
                {
                    ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(request.IdOfEntity, includeReservation: true);

                    if (conferenceRoom == null)
                    {
                        deleteResponse.Success = false;
                        return deleteResponse;
                    }

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        if (conferenceRoom.Reservation != null)
                        {
                            _reservationRepository.Delete(conferenceRoom.Reservation);
                        }

                        _conferenceRoomRepository.Delete(conferenceRoom);

                        transaction.Complete();
                    }

                    deleteResponse.Success = true;
                }

                return deleteResponse;
            }
            catch (Exception _)
            {
                deleteResponse.Success = false;
                return deleteResponse;
            }
        }

        public ConferenceRoomsResponse ListAllConferenceRooms(int id)
        {
            ConferenceRoomsResponse responseConferenceRoom = new ConferenceRoomsResponse();

            try
            {
                responseConferenceRoom.ConferenceRoomsList = _conferenceRoomRepository.GetOfficeConferenceRooms(id);

                List<ConferenceRoom> roomsToUpdate = new List<ConferenceRoom>();
                foreach (ConferenceRoom conferenceRoom in responseConferenceRoom.ConferenceRoomsList)
                {
                    if (conferenceRoom.ReservationId.HasValue)
                    {
                        Reservation reservation = _reservationRepository.Get(conferenceRoom.ReservationId.Value);
                        if (DateTime.Compare(reservation.StartDate, DateTime.Now) < 0 && DateTime.Compare(reservation.EndDate, DateTime.Now) < 0)
                        {
                            conferenceRoom.ReservationId = null;
                            roomsToUpdate.Add(conferenceRoom);
                        }
                    }
                }
                _conferenceRoomRepository.BulkUpdate(roomsToUpdate);

                responseConferenceRoom.Sucess = true;

                return responseConferenceRoom;
            }
            catch (Exception _)
            {
                responseConferenceRoom.Sucess = false;

                return responseConferenceRoom;
            }
        }

        public EntitiesResponse UpdateDesks(UpdateRequest request)
        {
            EntitiesResponse entitiesResponse = new EntitiesResponse();

            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (DeskToUpdate deskToUpdate in request.ListOfDesksToUpdate)
                    {
                        Categories existingDeskCategories = _categoriesRepository.GetDeskCategories(deskToUpdate.DeskId);
                        if (existingDeskCategories != null)
                        {
                            existingDeskCategories.DoubleMonitor = deskToUpdate.DualMonitor;
                            existingDeskCategories.SingleMonitor = deskToUpdate.SingleMonitor;
                            existingDeskCategories.NearWindow = deskToUpdate.NearWindow;
                            existingDeskCategories.Unavailable = deskToUpdate.Unavailable;

                            _categoriesRepository.Update(existingDeskCategories);
                        }
                        else
                        {
                            Categories categories = new Categories()
                            {
                                DeskId = deskToUpdate.DeskId,
                                DoubleMonitor = deskToUpdate.DualMonitor,
                                SingleMonitor = deskToUpdate.SingleMonitor,
                                NearWindow = deskToUpdate.NearWindow,
                                Unavailable = deskToUpdate.Unavailable
                            };

                            Desk desk = _deskRepository.Get(deskToUpdate.DeskId);

                            if (desk == null)
                            {
                                continue;
                            }

                            _categoriesRepository.Insert(categories);
                            desk.CategorieId = categories.Id;

                            _deskRepository.Update(desk);
                        }
                    }

                    transaction.Complete();
                }

                entitiesResponse.Success = true;

                return entitiesResponse;
            }
            catch (Exception _)
            {
                entitiesResponse.Success = false;
                return entitiesResponse;
            }
        }
    }
}
