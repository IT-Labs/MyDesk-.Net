using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Utils;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository reservationRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)

        {
            _reservationRepository = reservationRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public CancelReservationResponse CancelReservation(int id)
        {
            CancelReservationResponse cancelReservationResponse = new CancelReservationResponse();

            Reservation reservationToDelete = _reservationRepository.Get(id, includeDesk: true, includeonferenceRoom: true, includeReviews: true);
            if (reservationToDelete == null)
            {
                cancelReservationResponse.Success = false;
                return cancelReservationResponse;
            }

            foreach (Review review in reservationToDelete.Reviews)
            {
                review.IsDeleted = true;
            }

            _reservationRepository.SoftDelete(reservationToDelete);

            cancelReservationResponse.Success = true;

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
                    ReservationDto reservation = new ReservationDto
                    {
                        Id = employeeReservation.Id,
                        EmployeeId = employeeReservation.EmployeeId,
                        DeskId = employeeReservation.DeskId,
                        ConfId = employeeReservation.ConferenceRoomId,
                        ReviewId = employeeReservation.Reviews.FirstOrDefault()?.Id,
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
                    ReservationDto reservation = new ReservationDto
                    {
                        Id = employeeReservation.Id,
                        EmployeeId = employeeReservation.EmployeeId,
                        DeskId = employeeReservation.DeskId,
                        ConfId = employeeReservation.ConferenceRoomId,
                        ReviewId = employeeReservation.Reviews.FirstOrDefault()?.Id,
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

        public AllReservationsResponse AllReservations(int? take = null, int? skip = null)
        {
            AllReservationsResponse response = new AllReservationsResponse();
            List<ReservationDto> newList = new List<ReservationDto>();

            List<Reservation> reservations = _reservationRepository.GetAll(includeEmployee: true, includeDesk: true, includeOffice: true, take: take, skip: skip);

            foreach (Reservation reservation in reservations)
            {
                ReservationDto newReservation = new ReservationDto
                {
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    ConferenceRoomId = reservation.ConferenceRoomId,
                    Id = reservation.Id,
                    OfficeName = reservation.Desk?.Office?.Name,
                    IndexForOffice = reservation.Desk?.IndexForOffice
                };

                if (reservation.Employee != null)
                {
                    newReservation.Employee = _mapper.Map<EmployeeDto>(reservation.Employee);
                }
                if (reservation.Desk != null)
                {
                    newReservation.Desk = _mapper.Map<DeskDto>(reservation.Desk);
                }
                if (reservation.Reviews != null && reservation.Reviews.Count > 0)
                {
                    newReservation.Review = _mapper.Map<ReviewDto>(reservation.Reviews.First());
                }

                newList.Add(newReservation);
            }

            response.Reservations = newList;
            response.TotalReservations = response.Reservations.Count();

            response.Success = true;

            return response;
        }

        public ReservationResponse CoworkerReserve(CoworkerReservationRequest request)
        {
            ReservationResponse response = new ReservationResponse();

            Employee employee = _employeeRepository.GetByEmail(request.CoworkerMail);
            List<Reservation> reservations = _reservationRepository.GetDeskReservations(request.DeskId);

            Reservation newReservation = new Reservation()
            {
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null),
                EmployeeId = employee.Id,
                DeskId = request.DeskId
            };

            foreach (Reservation reservation in reservations)
            {
                if (reservation.StartDate.IsInRange(newReservation.StartDate, newReservation.EndDate) || reservation.EndDate.IsInRange(newReservation.StartDate, newReservation.EndDate) ||
                    newReservation.StartDate.IsInRange(reservation.StartDate, reservation.EndDate) || newReservation.EndDate.IsInRange(reservation.StartDate, reservation.EndDate))
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
