
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOfficeApplication.Data.Models
{
    public class ConferenceRoomMode : BaseEntity
    {
        public int ConferenceRoomId { get; set; }

        public virtual ConferenceRoom? ConferenceRoom { get; set; }

        public int ModeId { get; set; }

        public virtual Mode? Mode { get; set; }
       
    }
}
