using inOfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOfficeApplication.Data.Models
{
    public class ConferenceRoomMode
    {
        public int ConferenceRoomId { get; set; }

        public ConferenceRoom? ConferenceRoom { get; set; }

        public int ModeId { get; set; }

        public Mode? Mode { get; set; }
    }
}
