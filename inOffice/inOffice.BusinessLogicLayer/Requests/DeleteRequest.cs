using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Requests
{
    public class DeleteRequest
    {
        public int IdOfEntity { get; set; }
        public string TypeOfEntity { get; set; }
    }
}
