using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class CustomReservationResponse
    {
        public int Id { get; set; }
        public int? DeskId { get; set; }
        public int? ConfId { get; set; }
        public int? ReviewId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ConfRoomIndex { get; set; }
        public int? DeskIndex { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OfficeName { get; set; }
    }
}
