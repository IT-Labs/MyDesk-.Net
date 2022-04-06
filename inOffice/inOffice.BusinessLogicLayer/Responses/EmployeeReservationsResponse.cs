using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class EmployeeReservationsResponse
    {
        public List<CustomReservationResponse> CustomReservationResponses { get; set; }
        public bool Success { get; set; }

        public EmployeeReservationsResponse()
        {
            this.CustomReservationResponses = new List<CustomReservationResponse>();
        }
    }
}
