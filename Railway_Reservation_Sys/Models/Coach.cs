using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Railway_Reservation_Sys.Models
{
    public class Coach
    {
        [Key]
        public int CoachID { get; set; }

        public int ScheduleID { get; set; }
        [ForeignKey("ScheduleID")]
        public Schedule Schedule { get; set; }

        [Required]
        public string CoachNo { get; set; } 

        [Required]
        public string CoachType { get; set; } 

        public ICollection<Seat> Seats { get; set; }
        public ICollection<ReservationDetails> ReservationDetails { get; set; }
    }
}

