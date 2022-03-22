using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class ConferenceRoomsResponse
    {
       public List<ConferenceRoom> ConferenceRoomsList { get; set; }

       public bool Sucess { get; set; }

    }
}
