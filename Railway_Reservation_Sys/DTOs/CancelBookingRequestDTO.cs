using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class CancelBookingRequestDTO
    {
        public int ReservationID { get; set; }
        public List<int> PassengerIDs { get; set; }
    }
}