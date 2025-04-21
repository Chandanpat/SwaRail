using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// using System.ComponentModel.DataAnnotations.Schema;


namespace Railway_Reservation_Sys.Models
{

    public class ReservationHeader
    {
        [Key]
        public int ReservationID { get; set; }  // PNR  Guid

        [Required]
        public Guid UserID { get; set; }
        public User User { get; set; }

        [Required]
        public int TrainID { get; set; }

        [ForeignKey("TrainID")]
        public Train Train { get; set; }

        // [Required]
        public int? ScheduleID { get; set; }
        public Schedule Schedule { get; set; }

        [Required]
        public decimal TotalFare { get; set; }

        public decimal TotalRefund { get; set; }

        [Required]
        // [RegularExpression("Pending|Confirmed|Partially Cancelled|Cancelled")]
        public string Status { get; set; }

        public ICollection<ReservationDetails> ReservationDetails { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }

}