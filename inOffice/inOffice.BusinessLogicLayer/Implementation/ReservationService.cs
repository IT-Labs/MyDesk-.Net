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
    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<Desk> _deskRepository;
        private readonly IRepository<ConferenceRoom> _conferenceRoomRepository;


        public ReservationService(IRepository<Reservation> reservation, IRepository<Desk> desk, IRepository<ConferenceRoom> conferenceRoomRepository) { 
            _reservationRepository = reservation;
            _deskRepository = desk;
            _conferenceRoomRepository = conferenceRoomRepository;
        
        }
        public ReservationResponse Reserve(ReservationRequest o, Employee e)
        {
            ReservationResponse response = new ReservationResponse();

             
            if(o.Desk != null)
            {
                Reservation NewReservation = new Reservation();
                NewReservation.StartDate = DateTime.ParseExact(o.StartDate,"dd-MM-yyyy",null);
                NewReservation.EndDate = DateTime.ParseExact(o.EndDate, "dd-MM-yyyy", null);
                NewReservation.Employee = e;

                
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
                NewReservation.Employee = e;

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
                return response;
            }

            return response;
        }
    }
}
