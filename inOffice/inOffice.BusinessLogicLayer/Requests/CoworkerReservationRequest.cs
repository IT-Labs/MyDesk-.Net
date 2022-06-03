using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Requests
{
    public class CoworkerReservationRequest
    {
        public String StartDate { get; set; }
        public String EndDate { get; set; }

        public String CoworkerMail { get; set; }

        public int DeskId { get; set; }
    }
}
