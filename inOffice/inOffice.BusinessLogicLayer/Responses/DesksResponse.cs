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

        public List<Desk> DeskList { get; set; }

        public bool sucess;

    }
}
