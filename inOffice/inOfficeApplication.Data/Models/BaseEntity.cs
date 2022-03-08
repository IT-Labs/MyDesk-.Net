using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOfficeApplication.Data.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}
