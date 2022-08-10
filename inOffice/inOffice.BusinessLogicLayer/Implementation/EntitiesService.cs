using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class EntitiesService : IEntitiesService
    {
        private readonly IRepository<Desk> _deskRepository;
        private readonly IRepository<ConferenceRoom> _conferenceRoomRepository;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Categories> _categoriesRepository;
        private readonly IRepository<DeskCategories> _deskCategoriesRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EntitiesService(IRepository<Desk> deskRepository, IRepository<ConferenceRoom> conferenceRoomRepository, IRepository<Reservation> reservationRepository, IRepository<Review> reviewRepository, IEmployeeRepository employeeRepository, IRepository<Categories> categoriesRepository, IRepository<DeskCategories> deskCategories)
        {
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;
            _reservationRepository = reservationRepository;
            _reviewRepository = reviewRepository;
            _employeeRepository = employeeRepository;
            _categoriesRepository = categoriesRepository;
            _deskCategoriesRepository = deskCategories;
        }

        public AllReviewsForEntity AllReviewsForEntity(int id)
        {
            var reservationsOfDesk = _reservationRepository.GetAll().Where(x => x.DeskId == id && x.ReviewId != null).ToList();

            AllReviewsForEntity response = new AllReviewsForEntity();

            List<string> list = new List<string>();

            foreach (var r in reservationsOfDesk)
            {
                var review = _reviewRepository.Get(r.ReviewId);
                var output = review.Reviews;
                list.Add(output);
            }

            response.AllReviews = list;
            if (response.AllReviews.Count > 0)
            {
                response.Success = true;
            }
            else
            {
                response.Success = false;
            }
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
                var desks = this._deskRepository.GetAll().Where(x => x.OfficeId == id).ToList();

                foreach (var item in desks)
                {
                    var reservationsForDesk = _reservationRepository.GetAll().Where(x => x.DeskId == item.Id && x.StartDate > DateTime.Today).ToList();

                    DeskCustom custom = new DeskCustom();

                    reservationsForDesk.ForEach(x => x.Employee = _employeeRepository.GetById(x.EmployeeId));
                    reservationsForDesk.ForEach(x => x.Employee.Reservations.Clear());
                    reservationsForDesk.ForEach(x => x.Employee.Password = null);

                    custom.Id = item.Id;
                    custom.IndexForOffice = item.IndexForOffice;
                    custom.Reservations = reservationsForDesk;
                    var findCategories = _categoriesRepository.GetAll().Where(x => x.DeskId == item.Id).FirstOrDefault();
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

                responseConferenceRoom.ConferenceRoomsList = this._conferenceRoomRepository.GetAll().Where(x => x.OfficeId == id).ToList();

                foreach (var item in responseConferenceRoom.ConferenceRoomsList)
                {
                    if (item.ReservationId != null)
                    {
                        var reservation = _reservationRepository.Get(item.ReservationId);
                        if (DateTime.Compare(reservation.StartDate, DateTime.Now) < 0 && DateTime.Compare(reservation.EndDate, DateTime.Now) < 0)
                        {
                            item.ReservationId = null;
                            _conferenceRoomRepository.Update(item);
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
