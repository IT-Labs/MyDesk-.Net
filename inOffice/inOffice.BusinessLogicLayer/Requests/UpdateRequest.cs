using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Requests
{
    public class UpdateRequest
    {
        public List<int> CheckedDesks { get; set; }

        public List<int> UncheckedDesks { get; set; }

/*        public List<ConfRoomToUpdate> ConferenceRoomCapacity { get; set; }
*/
        public bool Sucess;
    }
}
