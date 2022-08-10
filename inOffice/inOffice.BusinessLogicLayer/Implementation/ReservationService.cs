using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Newtonsoft.Json;
using System.Text;
using System.Transactions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRepository<Desk> _deskRepository;
        private readonly IRepository<ConferenceRoom> _conferenceRoomRepository;
        private readonly IRepository<Office> _officeRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IEmployeeRepository _employeeRepository;
        static HttpClient client = new HttpClient();

        public ReservationService(IReservationRepository reservationRepository,
            IRepository<Desk> deskRepository,
            IRepository<ConferenceRoom> conferenceRoomRepository,
            IRepository<Office> officeRepository,
            IRepository<Review> reviewRepository,
            IEmployeeRepository employeeRepository
            )

        {
            _reservationRepository = reservationRepository;
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;
            _officeRepository = officeRepository;
            _reviewRepository = reviewRepository;
            _employeeRepository = employeeRepository;
        }

        public CancelReservationResponse CancelReservation(int id)
        {
            CancelReservationResponse cancelReservationResponse = new CancelReservationResponse();

            Reservation reservationToDelete = _reservationRepository.Get(id, includeDesk: true, includeonferenceRoom: true);
            try
            {
                if (reservationToDelete.Desk != null)
                {
                    reservationToDelete.Desk.ReservationId = null;

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        _deskRepository.Update(reservationToDelete.Desk);
                        _reservationRepository.Delete(reservationToDelete);

                        transaction.Complete();
                    }

                    cancelReservationResponse.Success = true;
                }
                else if (reservationToDelete.ConferenceRoom != null)
                {
                    reservationToDelete.ConferenceRoom.ReservationId = null;

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        _conferenceRoomRepository.Update(reservationToDelete.ConferenceRoom);
                        _reservationRepository.Delete(reservationToDelete);

                        transaction.Complete();
                    }

                    cancelReservationResponse.Success = true;
                }

            }
            catch (Exception _)
            {
                cancelReservationResponse.Success = false;
            }

            return cancelReservationResponse;
        }

        public CreateReviewResponse CreateReview(CreateReviewRequest createReviewRequest)
        {
            CreateReviewResponse response = new CreateReviewResponse();

            Reservation reservation = _reservationRepository.Get(createReviewRequest.ReservationId);
            Task<string> responseSentimentAnalysis = GetAnalysedReview(createReviewRequest.Review);

            try
            {
                Review review = new Review()
                {
                    Reviews = createReviewRequest.Review,
                    ReservationId = createReviewRequest.ReservationId,
                    ReviewOutput = responseSentimentAnalysis.Result
                };

                using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _reviewRepository.Insert(review);

                    reservation.ReviewId = review.Id;
                    _reservationRepository.Update(reservation);

                    transaction.Complete();
                }

                response.Success = true;
            }
            catch (Exception _)
            {
                response.Success = false;
            }

            return response;
        }

        public ReviewResponse ShowReview(int id)
        {
            Review review = _reviewRepository.Get(id);

            ReviewResponse reviewForGivenEntity = new ReviewResponse()
            {
                Review = review.Reviews,
                Sucess = true
            };

            return reviewForGivenEntity;
        }

        public AllReviewsResponse AllReviews()
        {            
            List<CustomReviews> list = new List<CustomReviews>();

            List<Review> reviews = _reviewRepository.GetAll().ToList();

            foreach (Review review in reviews)
            {
                Reservation reservation = _reservationRepository.Get(review.ReservationId, includeDesk: true, includeOffice: true);

                CustomReviews custom = new CustomReviews()
                {
                    OfficeName = reservation.Desk?.Office?.Name,
                    Review = review.Reviews,
                    ReviewOutput = review.ReviewOutput,
                    DeskIndex = reservation.Desk?.IndexForOffice
                };

                list.Add(custom);
            }

            AllReviewsResponse allReviews = new AllReviewsResponse()
            {
                ListOfReviews = list,
                Success = true
            };

            return allReviews;
        }

        public EmployeeReservationsResponse EmployeeReservations(Employee employee)
        {
            EmployeeReservationsResponse employeeReservationsResponse = new EmployeeReservationsResponse();

            List<Reservation> employeeReservations = _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeOffice: true);

            try
            {
                foreach (Reservation employeeReservation in employeeReservations)
                {
                    if (DateTime.Compare(employeeReservation.StartDate, DateTime.Now) > 0)
                    {
                        CustomReservationResponse reservation = new CustomReservationResponse
                        {
                            Id = employeeReservation.Id,
                            EmployeeId = employeeReservation.EmployeeId,
                            DeskId = employeeReservation.DeskId,
                            ConfId = employeeReservation.ConferenceRoomId,
                            ReviewId = employeeReservation.ReviewId,
                            StartDate = employeeReservation.StartDate,
                            EndDate = employeeReservation.EndDate,
                            OfficeName = employeeReservation.Desk?.Office?.Name,
                            DeskIndex = employeeReservation.Desk?.IndexForOffice
                        };
                        employeeReservationsResponse.CustomReservationResponses.Add(reservation);
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

        public EmployeeReservationsResponse PastReservations(Employee employee)
        {
            EmployeeReservationsResponse employeeReservationsResponse = new EmployeeReservationsResponse();
            List<Reservation> employeeReservations = _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeOffice: true);

            try
            {
                foreach (Reservation employeeReservation in employeeReservations)
                {
                    if (DateTime.Compare(employeeReservation.StartDate, DateTime.Now) < 0 && DateTime.Compare(employeeReservation.EndDate, DateTime.Now) < 0)
                    {
                        CustomReservationResponse reservation = new CustomReservationResponse
                        {
                            Id = employeeReservation.Id,
                            EmployeeId = employeeReservation.EmployeeId,
                            DeskId = employeeReservation.DeskId,
                            ConfId = employeeReservation.ConferenceRoomId,
                            ReviewId = employeeReservation.ReviewId,
                            StartDate = employeeReservation.StartDate,
                            EndDate = employeeReservation.EndDate,
                            OfficeName = employeeReservation.Desk?.Office?.Name,
                            DeskIndex = employeeReservation.Desk?.IndexForOffice
                        };
                        employeeReservationsResponse.CustomReservationResponses.Add(reservation);
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

        public ReservationResponse Reserve(ReservationRequest request, Employee employee)
        {
            ReservationResponse response = new ReservationResponse();

            Reservation newReservation = new Reservation();
            newReservation.StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null);
            newReservation.EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null);
            newReservation.EmployeeId = employee.Id;

            if (request.Desk != null)
            {
                Desk desk = _deskRepository.Get(request.Desk.Id);

                newReservation.DeskId = desk.Id;
                newReservation.Desk = desk;

                _reservationRepository.Insert(newReservation);

                response.Success = true;
            }
            else if (request.ConferenceRoom != null)
            {
                ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(request.ConferenceRoom.Id);

                newReservation.ConferenceRoomId = conferenceRoom.Id;
                newReservation.ConferenceRoom = conferenceRoom;

                _reservationRepository.Insert(newReservation);

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
            AllReservationsResponse response = new AllReservationsResponse();
            List<ReservationNew> newList = new List<ReservationNew>();

            List<Reservation> reservations = _reservationRepository.GetAll(includeEmployee: true, includeDesk: true, includeOffice: true);

            foreach (Reservation reservation in reservations)
            {
                ReservationNew newReservation = new ReservationNew
                {
                    Employee = reservation.Employee,
                    Desk = reservation.Desk,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    ConferenceRoom = reservation.ConferenceRoom,
                    ConferenceRoomId = reservation.ConferenceRoomId,
                    Review = reservation.Review,
                    Id = reservation.Id,
                    OfficeName = reservation.Desk?.Office?.Name,
                    IndexForOffice = reservation.Desk?.IndexForOffice
            };

                if (newReservation.Employee != null)
                {
                    newReservation.Employee.Reservations?.Clear();
                    newReservation.Employee.Password = null;
                }

                newList.Add(newReservation);
            }

            response.Reservations = newList;
            response.TotalReservations = response.Reservations.Count();

            response.Success = response.TotalReservations > 0;

            return response;
        }

        public ReservationResponse CoworkerReserve(CoworkerReservationRequest request)
        {
            Reservation newReservation = new Reservation();
            ReservationResponse response = new ReservationResponse();

            Employee employee = _employeeRepository.GetByEmail(request.CoworkerMail);
            Desk desk = _deskRepository.Get(request.DeskId);
            List<Reservation> reservations = _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true);

            newReservation.StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null);
            newReservation.EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null);
            newReservation.EmployeeId = employee.Id;
            newReservation.DeskId = desk.Id;
            newReservation.Desk = desk;

            foreach (Reservation reservation in reservations)
            {
                if (reservation.Desk?.OfficeId == desk.OfficeId && newReservation.StartDate.Ticks >= reservation.StartDate.Ticks && newReservation.EndDate.Ticks <= reservation.EndDate.Ticks)
                {
                    response.Success = false;
                    return response;
                }
            }

            _reservationRepository.Insert(newReservation);

            response.Success = true;
            return response;
        }

        private async Task<string> GetAnalysedReview(string textReview)
        {
            string review = string.Empty;
            Dictionary<string, string> dictionaryData = new Dictionary<string, string>();
            dictionaryData.Add("text", textReview);
            string data = JsonConvert.SerializeObject(dictionaryData);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://inofficenlpmodel.azurewebsites.net/api/get_sentiment?code=knpbFNoCymH02BtJtcO59H4mgbkRVbSBhSzlwuZmxXCtAzFuEqSMTA==", content);
            string stringResponse = response.Content.ReadAsStringAsync().Result;
            ReviewAzureFunction result = JsonConvert.DeserializeObject<ReviewAzureFunction>(stringResponse);
            if (response.IsSuccessStatusCode)
            {
                review = result.Sentiment;
            }
            return review;
        }
    }
}
