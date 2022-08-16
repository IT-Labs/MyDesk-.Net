using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using System.Transactions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IDeskRepository _deskRepository;
        private readonly IConferenceRoomRepository _conferenceRoomRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ReservationService(IReservationRepository reservationRepository,
            IDeskRepository deskRepository,
            IConferenceRoomRepository conferenceRoomRepository,
            IEmployeeRepository employeeRepository
            )

        {
            _reservationRepository = reservationRepository;
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;
            _employeeRepository = employeeRepository;
        }

        public CancelReservationResponse CancelReservation(int id)
        {
            CancelReservationResponse cancelReservationResponse = new CancelReservationResponse();

            Reservation reservationToDelete = _reservationRepository.Get(id, includeDesk: true, includeonferenceRoom: true);
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

            return cancelReservationResponse;
        }

        public EmployeeReservationsResponse EmployeeReservations(Employee employee)
        {
            EmployeeReservationsResponse employeeReservationsResponse = new EmployeeReservationsResponse();

            List<Reservation> employeeReservations = _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeOffice: true);
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

            return employeeReservationsResponse;
        }

        public EmployeeReservationsResponse PastReservations(Employee employee)
        {
            EmployeeReservationsResponse employeeReservationsResponse = new EmployeeReservationsResponse();
            List<Reservation> employeeReservations = _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeOffice: true);

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

        public AllReservationsResponse AllReservations(int? take = null, int? skip = null)
        {
            AllReservationsResponse response = new AllReservationsResponse();
            List<ReservationNew> newList = new List<ReservationNew>();

            List<Reservation> reservations = _reservationRepository.GetAll(includeEmployee: true, includeDesk: true, includeOffice: true, take: take, skip: skip);

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
    }
}
