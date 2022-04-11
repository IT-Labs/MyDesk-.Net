using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Requests
{
    public class CreateReviewRequest
    {
        public int ReservationId { get; set; }
        public string Review { get; set; }
    }
}
