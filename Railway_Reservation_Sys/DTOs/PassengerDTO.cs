using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class PassengerDTO
    {
        // public int PassengerID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; } // "Male", "Female", "Other"
        public int Age { get; set; }
        public string Address { get; set; }
    }
}
