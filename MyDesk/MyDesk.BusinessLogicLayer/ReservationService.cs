using AutoMapper;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;
using MyDesk.Core.Requests;
using MyDesk.Core.Database;
using Microsoft.EntityFrameworkCore;
using MyDesk.BusinessLogicLayer.Utils;

namespace MyDesk.BusinessLogicLayer
{
    public class ReservationService : IReservationService
    {
        private readonly IMapper _mapper;
        private readonly IContext _context;

        public ReservationService(IMapper mapper,
            IContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public void CancelReservation(int id)
        {
            var reservationToDelete = _context
                .AsQueryable<Reservation>()
                .Where(x => x.Id == id && x.IsDeleted == false)
                .Include(x => x.Reviews.Where(y => y.IsDeleted == false))
                .FirstOrDefault();
            if (reservationToDelete == null)
            {
                throw new NotFoundException($"Reservation with ID: {id} not found.");
            }

            foreach (Review review in reservationToDelete.Reviews)
            {
                review.IsDeleted = true;
            }

            reservationToDelete.IsDeleted= true;
            _context.Modify(reservationToDelete);
        }

        public PaginationDto<ReservationDto> FutureReservations(string employeeEmail, int? take = null, int? skip = null)
        {
            Employee? employee = null;
            if (!string.IsNullOrEmpty(employeeEmail))
            {
                employee = GetEmployeeByEmail(employeeEmail);
                if (employee == null)
                {
                    throw new NotFoundException($"Employee with email: {employeeEmail} not found.");
                }
            }

            Tuple<int?, List<Reservation>> result = GetFutureReservations(employee?.Id, includeEmployee: true, includeDesk: true, includeConferenceRoom: true,
                includeOffice: true, take: take, skip: skip);

            return new PaginationDto<ReservationDto>()
            {
                Values = _mapper.Map<List<ReservationDto>>(result.Item2),
                TotalCount = result.Item1.HasValue ? result.Item1.Value : result.Item2.Count()
            };
        }

        public PaginationDto<ReservationDto> PastReservations(string employeeEmail, int? take = null, int? skip = null)
        {
            Employee? employee = null;
            if (!string.IsNullOrEmpty(employeeEmail))
            {
                employee = GetEmployeeByEmail(employeeEmail);
                if (employee == null)
                {
                    throw new NotFoundException($"Employee with email: {employeeEmail} not found.");
                }
            }

            Tuple<int?, List<Reservation>> result = GetPastReservations(employee?.Id, includeEmployee: true, includeDesk: true, includeConferenceRoom: true,
                includeOffice: true, includeReviews: true, take: take, skip: skip);

            return new PaginationDto<ReservationDto>()
            {
                Values = _mapper.Map<List<ReservationDto>>(result.Item2),
                TotalCount = result.Item1.HasValue ? result.Item1.Value : result.Item2.Count()
            };
        }

        public void CoworkerReserve(ReservationRequest request)
        {
            var desk = _context
                .AsQueryable<Desk>()
                .FirstOrDefault(x => x.Id == request.DeskId && x.IsDeleted == false);
            if (desk == null)
            {
                throw new NotFoundException($"Desk with ID: {request.DeskId} not found.");
            }

            var employee = GetEmployeeByEmail(request.EmployeeEmail);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with email: {request.EmployeeEmail} not found.");
            }

            var deskReservations = _context
                .AsQueryable<Reservation>()
                .Where(x => x.DeskId == request.DeskId && x.IsDeleted == false && (x.StartDate >= DateTime.Now.Date || x.EndDate >= DateTime.Now.Date));

            var newReservation = new Reservation
            {
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null),
                EmployeeId = employee.Id,
                DeskId = desk.Id
            };

            // Check if there are existing reservations for that desk in that time-frame
            foreach (Reservation reservation in deskReservations)
            {
                if (reservation.StartDate.IsInRange(newReservation.StartDate, newReservation.EndDate) || reservation.EndDate.IsInRange(newReservation.StartDate, newReservation.EndDate) ||
                    newReservation.StartDate.IsInRange(reservation.StartDate, reservation.EndDate) || newReservation.EndDate.IsInRange(reservation.StartDate, reservation.EndDate))
                {
                    throw new ConflictException($"Reservation for that time period already exists for desk {desk.Id}");
                }
            }

            // Check if there are existing reservations for that employee in that time-frame for current office
            var employeeReservations = _context
                .AsQueryable<Reservation>()
                .Where(x => x.EmployeeId == employee.Id && x.IsDeleted == false && (x.StartDate >= DateTime.Now.Date || x.EndDate >= DateTime.Now.Date))
                .Include(x => x.Desk)
                .Include(x => x.ConferenceRoom);

            foreach (Reservation reservation in employeeReservations)
            {
                if (desk.OfficeId == GetOfficeId(reservation) && (reservation.StartDate.IsInRange(newReservation.StartDate, newReservation.EndDate) || reservation.EndDate.IsInRange(newReservation.StartDate, newReservation.EndDate) ||
                    newReservation.StartDate.IsInRange(reservation.StartDate, reservation.EndDate) || newReservation.EndDate.IsInRange(reservation.StartDate, reservation.EndDate)))
                {
                    throw new ConflictException($"Reservation for that time period already exists for {employee.Email}");
                }
            }

            _context.Insert(newReservation);
        }

        private int GetOfficeId(Reservation reservation)
        {
            if (reservation.Desk != null)
            {
                return reservation.Desk.OfficeId;
            }
            else
            {
                return reservation.ConferenceRoom.OfficeId;
            }
        }

        private Employee? GetEmployeeByEmail(string employeeEmail)
        {
            return _context
                .AsQueryable<Employee>()
                .FirstOrDefault(x => x.Email.ToLower() == employeeEmail.ToLower() && x.IsDeleted == false);
        }

        public Tuple<int?, List<Reservation>> GetFutureReservations(int? employeeId = null,
            bool? includeEmployee = null,
            bool? includeDesk = null,
            bool? includeConferenceRoom = null,
            bool? includeOffice = null,
            int? take = null,
            int? skip = null)
        {
            int? totalCount = null;
            var query = _context
                .AsQueryable<Reservation>()
                .Where(x => x.IsDeleted == false && x.StartDate >= DateTime.Now.Date);

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            if (includeEmployee == true)
            {
                query = query.Include(x => x.Employee);
            }

            if (includeDesk == true && includeOffice != true)
            {
                query = query.Include(x => x.Desk);
            }
            else if (includeDesk == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }

            if (includeConferenceRoom == true && includeOffice != true)
            {
                query = query.Include(x => x.ConferenceRoom);
            }
            else if (includeConferenceRoom == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.ConferenceRoom)
                    .ThenInclude(x => x.Office);
            }

            if (take.HasValue && skip.HasValue)
            {
                totalCount = query.Count();
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return Tuple.Create(totalCount, query.ToList());
        }

        public Tuple<int?, List<Reservation>> GetPastReservations(int? employeeId = null,
            bool? includeEmployee = null,
            bool? includeDesk = null,
            bool? includeConferenceRoom = null,
            bool? includeOffice = null,
            bool? includeReviews = null,
            int? take = null,
            int? skip = null)
        {
            int? totalCount = null;
            var query = _context
                .AsQueryable<Reservation>()
                .Where(x => x.IsDeleted == false && x.StartDate < DateTime.Now.Date && x.EndDate < DateTime.Now.Date);

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            if (includeEmployee == true)
            {
                query = query.Include(x => x.Employee);
            }

            if (includeDesk == true && includeOffice != true)
            {
                query = query.Include(x => x.Desk);
            }
            else if (includeDesk == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.Desk)
                    .ThenInclude(x => x.Office);
            }

            if (includeConferenceRoom == true && includeOffice != true)
            {
                query = query.Include(x => x.ConferenceRoom);
            }
            else if (includeConferenceRoom == true && includeOffice == true)
            {
                query = query
                    .Include(x => x.ConferenceRoom)
                    .ThenInclude(x => x.Office);
            }

            if (includeReviews == true)
            {
                query = query.Include(x => x.Reviews.Where(y => y.IsDeleted == false));
            }

            if (take.HasValue && skip.HasValue)
            {
                totalCount = query.Count();
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return Tuple.Create(totalCount, query.ToList());
        }

    }
}
