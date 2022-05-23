using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace inOffice.BusinessLogicLayer.Responses
{
    public class AllReservationsResponse
    {
        public List<ReservationNew>? Reservations { get; set; }

        public int TotalReservations { get; set; }

        public bool Success { get; set; }
    }


    public class ReservationNew : Reservation
    {
        public string? OfficeName { get; set; }

        public int? IndexForOffice { get; set; }    
    }
}
