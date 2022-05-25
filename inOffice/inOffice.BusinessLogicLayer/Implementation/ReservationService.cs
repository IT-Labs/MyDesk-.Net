using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Newtonsoft.Json;
using System.Dynamic;
using System.Linq;
using System.Text;


namespace inOffice.BusinessLogicLayer.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<Desk> _deskRepository;
        private readonly IRepository<ConferenceRoom> _conferenceRoomRepository;
        private readonly IRepository<Office> _officeRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IEmployeeRepository _employeeRepository;
        static HttpClient client = new HttpClient() ;

        public ReservationService(IRepository<Reservation> reservation, 
            IRepository<Desk> desk, 
            IRepository<ConferenceRoom> conferenceRoomRepository,
            IRepository<Office> officeRepository,
            IRepository<Review> reviewRepository,
            IEmployeeRepository employeeRepository
            ) 

        { 
            _reservationRepository = reservation;
            _deskRepository = desk;
            _conferenceRoomRepository = conferenceRoomRepository;
            _officeRepository = officeRepository;
            _reviewRepository = reviewRepository;
            _employeeRepository = employeeRepository;  
        }

        static async Task<String> GetAnalysedReview(string textReview)
        {
            string review = ""; 
            Dictionary<String, String> data = new Dictionary<string, string>();
            data.Add("text", textReview);
            var dataTwo = JsonConvert.SerializeObject(data);

            var content = new StringContent(dataTwo, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://inofficenlpmodel.azurewebsites.net/api/get_sentiment",content);
            string str = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<ReviewAzureFunction>(str);
            if (response.IsSuccessStatusCode)
            {
                review = result.sentiment;
            }
            return review;
        }

        public CancelReservationResponse CancelReservation(int id)
        {
            CancelReservationResponse cancelReservationResponse = new CancelReservationResponse();  

            var reservationToDelete = _reservationRepository.Get(id);
            try
            {
                if (reservationToDelete.DeskId != null)
                {
                    var finddesk = _deskRepository.Get(reservationToDelete.DeskId);
                    finddesk.ReservationId = null;
                    _deskRepository.Update(finddesk);
                    _reservationRepository.Delete(reservationToDelete);
                    cancelReservationResponse.Success = true;
                }
                else if (reservationToDelete.ConferenceRoomId != null)
                {
                    var findconf = _conferenceRoomRepository.Get(reservationToDelete.ConferenceRoomId);
                    findconf.ReservationId = null;
                    _conferenceRoomRepository.Update(findconf);
                    _reservationRepository.Delete(reservationToDelete);
                    cancelReservationResponse.Success = true;
                }
                
            }
            catch(Exception _)
            {
                cancelReservationResponse.Success = false;
            }
            return cancelReservationResponse;
        }

        public CreateReviewResponse CreateReview(CreateReviewRequest createReviewRequest)
        {
            CreateReviewResponse response = new CreateReviewResponse();

            var reservation = _reservationRepository.Get(createReviewRequest.ReservationId);

            var responseSentimentAnalysis = GetAnalysedReview(createReviewRequest.Review);

            try
            {
                Review review = new Review();
                review.Reviews = createReviewRequest.Review;
                review.ReservationId=createReviewRequest.ReservationId;
                review.ReviewOutput = responseSentimentAnalysis.Result;

                _reviewRepository.Insert(review);

                reservation.ReviewId = review.Id;
                _reservationRepository.Update(reservation);

                response.Success = true;
            }
            catch(Exception _)
            {
                response.Success = false;
            }

            return response;

        }

        public ReviewResponse ShowReview(int id)
        {
            var review = _reviewRepository.Get(id);

            ReviewResponse reviewForGivenEntity = new ReviewResponse();

            reviewForGivenEntity.Review = review.Reviews;
            reviewForGivenEntity.Sucess = true;

            return reviewForGivenEntity;


        }

        public AllReviewsResponse AllReviews()
        {
            AllReviewsResponse allReviews = new AllReviewsResponse();
            var reviews = _reviewRepository.GetAll().ToList();
            List<CustomReviews> list = new List<CustomReviews>();
            
            foreach(var review in reviews)
            {

                var reviewText = review.Reviews;
                var reviewOutput = review.ReviewOutput;
                var reservationId = _reservationRepository.Get(review.ReservationId);
                var deskId = _deskRepository.Get(reservationId.DeskId);
                var deskIndex = deskId.IndexForOffice;
                var office = _officeRepository.Get(deskId.OfficeId);
                var officeName = office.Name;

                CustomReviews custom = new CustomReviews();

                custom.OfficeName = officeName;
                custom.Review = reviewText;
                custom.ReviewOutput = reviewOutput;
                custom.DeskIndex = deskIndex;

                list.Add(custom);

            }

            if (reviews != null)
            {
                allReviews.Success = true;
                allReviews.ListOfReviews = list;
            }
            else
            {
                allReviews.Success=false;
            }

            
            return allReviews;
        }

        public EmployeeReservationsResponse EmployeeReservations(Employee employee)
        {
            EmployeeReservationsResponse employeeReservationsResponse = new EmployeeReservationsResponse();
           
            var employeeReservations = _reservationRepository.GetAll()
                .Where(z => z.EmployeeId == employee.Id)
                .ToList();

            try
            {
                foreach (var item in employeeReservations)
                {

                    if (DateTime.Compare(item.StartDate, DateTime.Now)>0)
                    {
                        
                        var desk = _deskRepository.Get(item.DeskId);
                        if (desk == null)
                        {
                            var confroom = _conferenceRoomRepository.Get(item.ConferenceRoomId);
                            var office = _officeRepository.Get(confroom.OfficeId);
                            var reservation = new CustomReservationResponse { Id = item.Id, EmployeeId = item.EmployeeId, DeskId = item.DeskId, ConfId = item.ConferenceRoomId, ReviewId = item.ReviewId, StartDate = item.StartDate,  EndDate = item.EndDate, OfficeName = office.Name, ConfRoomIndex = confroom.IndexForOffice };
                            employeeReservationsResponse.CustomReservationResponses.Add(reservation);
                        }
                        else
                        {
                            var office = _officeRepository.Get(desk.OfficeId);
                            var reservation = new CustomReservationResponse { Id = item.Id, EmployeeId = item.EmployeeId, DeskId = item.DeskId, ConfId = item.ConferenceRoomId, ReviewId = item.ReviewId, StartDate = item.StartDate, EndDate = item.EndDate, OfficeName = office.Name, DeskIndex = desk.IndexForOffice };
                            employeeReservationsResponse.CustomReservationResponses.Add(reservation);
                        }
                        
                    }
                }
                employeeReservationsResponse.Success = true;
            }
            catch(Exception _)
            {
                employeeReservationsResponse.Success = false;
            }

            return employeeReservationsResponse;
        }

        public EmployeeReservationsResponse PastReservations(Employee employee)
        {
            EmployeeReservationsResponse employeeReservationsResponse = new EmployeeReservationsResponse();
            var employeeReservations = _reservationRepository.GetAll()
                .Where(z => z.EmployeeId == employee.Id)
                .ToList();

            try
            {
                foreach (var item in employeeReservations)
                {

                    if(DateTime.Compare(item.StartDate, DateTime.Now) < 0 && DateTime.Compare(item.EndDate,DateTime.Now)<0)
                    {

                        var desk = _deskRepository.Get(item.DeskId);
                        if (desk == null)
                        {
                            var confroom = _conferenceRoomRepository.Get(item.ConferenceRoomId);

                            var office = _officeRepository.Get(confroom.OfficeId);
                            var reservation = new CustomReservationResponse { Id = item.Id, EmployeeId = item.EmployeeId, DeskId = item.DeskId, ConfId = item.ConferenceRoomId, ReviewId = item.ReviewId, StartDate = item.StartDate,EndDate=item.EndDate ,OfficeName = office.Name, ConfRoomIndex = confroom.IndexForOffice };
                            employeeReservationsResponse.CustomReservationResponses.Add(reservation);
                        }
                        else
                        {
                            var office = _officeRepository.Get(desk.OfficeId);
                            var reservation = new CustomReservationResponse { Id = item.Id, EmployeeId = item.EmployeeId, DeskId = item.DeskId, ConfId = item.ConferenceRoomId, ReviewId = item.ReviewId, StartDate = item.StartDate,EndDate = item.EndDate, OfficeName = office.Name, DeskIndex = desk.IndexForOffice };
                            employeeReservationsResponse.CustomReservationResponses.Add(reservation);
                        }

                    }
                }
                employeeReservationsResponse.Success = true;
            }
            catch (Exception _)
            {
                employeeReservationsResponse.Success = false;
            }

            return employeeReservationsResponse;
        }

        public ReservationResponse Reserve(ReservationRequest o, Employee e)
        {
            ReservationResponse response = new ReservationResponse();
            
            
            if (o.Desk != null)
            {
                    Reservation NewReservation = new Reservation();
                    NewReservation.StartDate = DateTime.ParseExact(o.StartDate, "dd-MM-yyyy", null);
                    NewReservation.EndDate = DateTime.ParseExact(o.EndDate, "dd-MM-yyyy", null);
                    NewReservation.EmployeeId = e.Id;
                    NewReservation.DeskId = o.Desk.Id;

                    this._reservationRepository.Insert(NewReservation);

                    var Desk = _deskRepository.Get(o.Desk.Id);
                    Desk.Reservation = NewReservation;
                    Desk.ReservationId = NewReservation.Id;
                    _deskRepository.Update(Desk);
                    

                response.Success = true;
            }
            else if(o.ConferenceRoom != null)
            {
                    Reservation NewReservation = new Reservation();
                    NewReservation.StartDate = DateTime.ParseExact(o.StartDate, "dd-MM-yyyy", null);
                    NewReservation.EndDate = DateTime.ParseExact(o.EndDate, "dd-MM-yyyy", null);
             
                    NewReservation.EmployeeId = e.Id;
                    NewReservation.ConferenceRoomId=o.ConferenceRoom.Id;

                    this._reservationRepository.Insert(NewReservation);

                    var ConferenceRoom = _conferenceRoomRepository.Get(o.ConferenceRoom.Id);
                    ConferenceRoom.Reservation = NewReservation;
                    ConferenceRoom.ReservationId = NewReservation.Id;
                    _conferenceRoomRepository.Update(ConferenceRoom);
                    
                    
                
                response.Success = true;
            }
            else
            {
                response.Success = false;
            }

            return response;
        }

        public AllReservationsResponse AllReservations()
        {
            var response = new AllReservationsResponse();

            var reservations = _reservationRepository.GetAll().ToList();

            List<ReservationNew> newList = new List<ReservationNew>();
            
            foreach(var r in reservations)
            {
                var employee = _employeeRepository.GetById(r.EmployeeId);

                var newReservation = new ReservationNew
                {
                    Employee = employee,
                    Desk = r.Desk,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    ConferenceRoom = r.ConferenceRoom,
                    ConferenceRoomId = r.ConferenceRoomId,
                    Review = r.Review,
                    Id = r.Id,
                };

                newReservation.Employee.Reservations.Clear();
                newReservation.Employee.Password = null;
                var desk = _deskRepository.Get(r.DeskId);
                if (desk != null)
                {
                    var office = _officeRepository.Get(desk.OfficeId);
                    if (office != null)
                    {
                        newReservation.OfficeName = office.Name;
                        newReservation.IndexForOffice = desk.IndexForOffice;
                    }
                }
               
                newList.Add(newReservation);

            }

            response.Reservations = newList;
            response.TotalReservations = response.Reservations.Count();

            if (response.TotalReservations > 0)
            {
                response.Success = true;
            }
            else
            {
                response.Success=false;
            }
            

            return response;
        }
    }
}
