using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class ReservationRequestDTO
    {
        // public Guid UserID { get; set; }
        public int TrainID { get; set; }
        public int ScheduleID { get; set; }
        public string CoachType { get; set; }
        public List<PassengerDTO> Passengers { get; set; }
    }
}