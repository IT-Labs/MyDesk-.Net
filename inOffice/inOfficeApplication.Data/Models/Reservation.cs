﻿using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class Reservation : BaseEntity
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int EmployeeId { get; set;}
        public Employee? Employee { get; set; }
        public Desk? Desk { get; set; }
        public ConferenceRoom? ConferenceRoom { get; set; }
        public Review? Review { get; set; }
    }
}
