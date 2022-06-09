using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOfficeApplication.Data.Models
{
    public class DeskCategories:BaseEntity
    {
        public int DeskId { get; set; }

        public Desk Desk { get; set; }
        public int CategoryId { get; set; }

        public Categories Categorie { get; set; }
    }
}
