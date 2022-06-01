using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class DesksResponse
    {

        public List<DeskCustom> DeskList { get; set; }

        public bool sucess;

    }

    public class DeskCustom
    {
        public int Id { get; set; }

        public int? IndexForOffice { get; set; }

        public List<Reservation>? Reservations { get; set; }

    }
}
