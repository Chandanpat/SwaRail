using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Railway_Reservation_Sys.Models
{
    public class Seat
    {
        [Key]
        public int SeatID { get; set; }

        [Required]
        public int SeatNo { get; set; }

        public int CoachID { get; set; }
        [ForeignKey("CoachID")]
        public Coach Coach { get; set; }

        [Required]
        // [RegularExpression("Available|Pending|Booked")]
        public string SeatStatus { get; set; }
    }

}