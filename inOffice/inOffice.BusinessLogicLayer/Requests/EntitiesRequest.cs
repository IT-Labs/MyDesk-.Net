using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Requests
{
    public class EntitiesRequest
    {
        public int Id { get; set; }
        public int NumberOfDesks { get; set; }
        public int NumberOfConferenceRooms { get; set; }
    }
}
