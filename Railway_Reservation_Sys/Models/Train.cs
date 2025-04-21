using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Railway_Reservation_Sys.Models
{
    public class Train
    {
        [Key]
        public int TrainID { get; set; }

        [Required]
        public string TrainName { get; set; }

        [Required]
        public int SourceID { get; set; }
        [ForeignKey("SourceID")]
        public Station SourceStation { get; set; }

        [Required]
        public int DestinationID { get; set; }
        [ForeignKey("DestinationID")]
        public Station DestinationStation { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        [Required]
        public decimal Fare { get; set; }

        public DateTime SourceDepartureTime { get; set; }
        public DateTime DestiArrivalTime { get; set; }

        public ICollection<Schedule> Schedules { get; set; }
        public ICollection<ReservationHeader> ReservationHeaders { get; set; }
    }

}