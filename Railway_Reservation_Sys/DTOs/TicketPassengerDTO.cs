using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class TicketPassengerDTO
    {
        public int PassengerID { get; set; }
        public string PassengerName { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public int? SeatNo { get; set; }
        public string? CoachNo { get; set; }
    }
}