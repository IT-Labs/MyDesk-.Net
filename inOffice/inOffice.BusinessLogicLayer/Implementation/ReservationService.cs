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

            List<Reservation> employeeReservations = _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeConferenceRoom: true, includeOffice: true);
            foreach (Reservation employeeReservation in employeeReservations)
            {
                if (DateTime.Compare(employeeReservation.StartDate, DateTime.Now) > 0)
                {
                    ReservationDto reservation = _mapper.Map<ReservationDto>(employeeReservation);                    
                    employeeReservationsResponse.CustomReservationResponses.Add(reservation);
                }
            }
            employeeReservationsResponse.Success = true;

            return employeeReservationsResponse;
        }

        public EmployeeReservationsResponse PastReservations(Employee employee)
        {
            EmployeeReservationsResponse employeeReservationsResponse = new EmployeeReservationsResponse();
            List<Reservation> employeeReservations = _reservationRepository.GetEmployeeReservations(employee.Id, includeDesk: true, includeConferenceRoom: true, includeOffice: true, includeReviews: true);

            foreach (Reservation employeeReservation in employeeReservations)
            {
                if (DateTime.Compare(employeeReservation.StartDate, DateTime.Now) < 0 && DateTime.Compare(employeeReservation.EndDate, DateTime.Now) < 0)
                {
                    ReservationDto reservation = _mapper.Map<ReservationDto>(employeeReservation);
                    employeeReservationsResponse.CustomReservationResponses.Add(reservation);
                }
            }
            employeeReservationsResponse.Success = true;

            return employeeReservationsResponse;
        }

        public AllReservationsResponse AllReservations(int? take = null, int? skip = null)
        {
            AllReservationsResponse response = new AllReservationsResponse();

            Tuple<int?, List<Reservation>> result = _reservationRepository.GetAll(includeEmployee: true, includeDesk: true, includeOffice: true, take: take, skip: skip);

            response.Reservations = _mapper.Map<List<ReservationDto>>(result.Item2);
            response.TotalReservations = result.Item1.HasValue ? result.Item1.Value : response.Reservations.Count();

            response.Success = true;

            return response;
        }

        public ReservationResponse CoworkerReserve(CoworkerReservationRequest request)
        {
            ReservationResponse response = new ReservationResponse();

            Employee employee = _employeeRepository.GetByEmail(request.CoworkerMail);
            List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(request.DeskId);

            Reservation newReservation = new Reservation()
            {
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null),
                EmployeeId = employee.Id,
                DeskId = request.DeskId
            };

            // Check if there are existing reservations for that desk in that time-frame
            foreach (Reservation reservation in deskReservations)
            {
                if (reservation.StartDate.IsInRange(newReservation.StartDate, newReservation.EndDate) || reservation.EndDate.IsInRange(newReservation.StartDate, newReservation.EndDate) ||
                    newReservation.StartDate.IsInRange(reservation.StartDate, reservation.EndDate) || newReservation.EndDate.IsInRange(reservation.StartDate, reservation.EndDate))
                {
                    response.Success = false;
                    return response;
                }
            }

            // Check if there are existing reservations for that employee in that time-frame
            List<Reservation> employeeReservations = _reservationRepository.GetEmployeeReservations(employee.Id);
            foreach (Reservation reservation in employeeReservations)
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
