using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class CustomEmployee
    {

        public CustomEmployee(int id, string? firstName, string? lastname, string? email, string? jobtitle)
        {
            Id = id;
            this.FirstName = firstName;
            this.LastName = lastname;
            Email = email;
            JobTitle = jobtitle;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }

        public string JobTitle { get; set; }    

        
    }
}
