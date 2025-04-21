using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class AddMultiScheduleDTO
    {
        public int TrainID { get; set; }
        public DateTime FromJourneyDate { get; set; }
        public DateTime ToJourneyDate { get; set; }

    }
}

