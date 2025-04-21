using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }

        public int TrainID { get; set; }
        [ForeignKey("TrainID")]
        public Train Train { get; set; }

        [Required]
        public DateTime JourneyDate { get; set; }

        public ICollection<Coach> Coaches { get; set; }
        public ICollection<ReservationHeader> ReservationHeaders { get; set; } 
    }
}



