using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class ReservationDetailsDTO
    {
        public int ReservationDetailID { get; set; }
        public int ReservationID { get; set; }
        public PassengerDTO Passenger { get; set; }
        public int? CoachID { get; set; }
        public int? SeatID { get; set; }
        public string Status { get; set; } // "Pending" or "Confirmed"
        public decimal FarePerSeat { get; set; }
        // public decimal RefundAmount { get; set; }
    }
}