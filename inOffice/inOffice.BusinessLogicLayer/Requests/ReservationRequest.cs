using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Requests
{
    public class ReservationRequest
    {
        public String StartDate { get; set; }
        public String EndDate { get; set; }
        public ConferenceRoom ?ConferenceRoom { get; set; }
        public Desk ?Desk { get; set; } 
    }
}
