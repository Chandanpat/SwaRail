using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class SeatDTO
    {

        public int SeatID { get; set; }
        public int SeatNo { get; set; }
        public string SeatStatus { get; set; } // "Available" or "Booked"
    }


}