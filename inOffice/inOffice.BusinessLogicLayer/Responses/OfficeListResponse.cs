using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class OfficeListResponse
    {
        public List<Office> Offices { get; set; }
        public bool Success { get; set; }
    }
}
