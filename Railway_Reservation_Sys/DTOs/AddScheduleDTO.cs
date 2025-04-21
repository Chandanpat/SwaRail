using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class AddScheduleDTO
    {
        //  public int ScheduleID { get; set; }
        public int TrainID { get; set; }
        public DateTime JourneyDate { get; set; }
    }
}