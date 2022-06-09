using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOfficeApplication.Data.Models
{
    public class Categories:BaseEntity
    {
        public int DeskId { get; set; }

        public  bool SingleMonitor { get; set; }

        public bool DoubleMonitor { get; set; }

        public bool NearWindow { get; set; }

        public bool Unavailable { get; set; }


        public virtual ICollection<DeskCategories> DeskCategories { get; set; }

    }

}
