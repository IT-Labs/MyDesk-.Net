using inOfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOfficeApplication.Data.Models
{
    public class DeskMode
    {
        
        public int DeskId { get; set; }
       
        public Desk? Desk { get; set; }
        
        public int ModeId { get; set; }

        public Mode? Mode { get; set; }


    }
}
