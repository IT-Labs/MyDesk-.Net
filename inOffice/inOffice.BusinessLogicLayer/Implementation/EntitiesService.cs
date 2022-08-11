using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class EntitiesService : IEntitiesService
    {
        private readonly IRepository<Desk> _deskRepository;
        private readonly IRepository<ConferenceRoom> _conferenceRoomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IRepository<Categories> _categoriesRepository;

        public EntitiesService(IRepository<Desk> deskRepository, 
            IRepository<ConferenceRoom> conferenceRoomRepository,
            IReservationRepository reservationRepository,
            IRepository<Categories> categoriesRepository)
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

        public EntitiesResponse CreateNewEntities(EntitiesRequest o)
        {

            EntitiesResponse response = new EntitiesResponse();
            var indexForDesk = this._deskRepository.GetAll().Where(x => x.OfficeId == o.Id).ToList().Count();

            try
            {
                for (int i = 0; i < o.NumberOfDesks; i++)
                {
                    Desk desk = new Desk();

                    desk.OfficeId = o.Id;
                    desk.IsDeleted = false;
                    desk.Categories = "regular";
                    desk.IndexForOffice = indexForDesk + i + 1;

                    this._deskRepository.Insert(desk);
                }

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
                List<Desk> desks = _deskRepository.GetAll().Where(x => x.OfficeId == id).ToList();

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

                    Categories? findCategories = _categoriesRepository.GetAll().Where(x => x.DeskId == desk.Id).FirstOrDefault();
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

        public DeleteResponse DeleteEntity(DeleteRequest o)
        {
            DeleteResponse deleteResponse = new DeleteResponse();

            try
            {

                if (o.TypeOfEntity == "D")
                {
                    var desk = _deskRepository.Get(o.IdOfEntity);
                    this._deskRepository.SoftDelete(desk);
                    deleteResponse.Success = true;

                }
                else
                {
                    var conferenceRoomToDelete = _conferenceRoomRepository.Get(o.IdOfEntity);
                    this._conferenceRoomRepository.Delete(conferenceRoomToDelete);
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
                responseConferenceRoom.ConferenceRoomsList = _conferenceRoomRepository.GetAll().Where(x => x.OfficeId == id).ToList();

                foreach (ConferenceRoom conferenceRoom in responseConferenceRoom.ConferenceRoomsList)
                {
                    if (conferenceRoom.ReservationId.HasValue)
                    {
                        Reservation reservation = _reservationRepository.Get(conferenceRoom.ReservationId.Value);
                        if (DateTime.Compare(reservation.StartDate, DateTime.Now) < 0 && DateTime.Compare(reservation.EndDate, DateTime.Now) < 0)
                        {
                            conferenceRoom.ReservationId = null;
                            _conferenceRoomRepository.Update(conferenceRoom);
                        }
                    }
                }

                responseConferenceRoom.Sucess = true;

                return responseConferenceRoom;
            }
            catch (Exception _)
            {
                responseConferenceRoom.Sucess = false;

                return responseConferenceRoom;
            }
        }

        public EntitiesResponse UpdateEntities(UpdateRequest o)
        {

            var entities = o.ListOfDesksToUpdate;
            EntitiesResponse entitiesResponse = new EntitiesResponse();

            try
            {
                foreach (var item in entities)
                {
                    var desk = _deskRepository.Get(item.DeskId);

                    Categories categories = new Categories();

                    categories.DeskId = item.DeskId;
                    categories.DoubleMonitor = item.DualMonitor;
                    categories.SingleMonitor = item.SingleMonitor;
                    categories.NearWindow = item.NearWindow;
                    categories.Unavailable = item.Unavailable;

                    var DeskAllreadyHasCategories = _categoriesRepository.GetAll().Where(x => x.DeskId == item.DeskId).FirstOrDefault();
                    if (DeskAllreadyHasCategories != null)
                    {
                        DeskAllreadyHasCategories.DoubleMonitor = item.DualMonitor;
                        DeskAllreadyHasCategories.SingleMonitor = item.SingleMonitor;
                        DeskAllreadyHasCategories.NearWindow = item.NearWindow;
                        DeskAllreadyHasCategories.Unavailable = item.Unavailable;

                        _categoriesRepository.Update(DeskAllreadyHasCategories);
                    }
                    else
                    {
                        _categoriesRepository.Insert(categories);

                        desk.CategorieId = categories.Id;

                        _deskRepository.Update(desk);
                    }

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
