using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Railway_Reservation_Sys.Models
{

    public class ReservationDetails
    {
        [Key]
        public int ReservationDetailID { get; set; }

        [Required]
        public int ReservationID { get; set; }
        public ReservationHeader ReservationHeader { get; set; }

        [Required]
        public int PassengerID { get; set; }
        public Passenger Passenger { get; set; }

        public int? CoachID { get; set; }
        public Coach? Coach { get; set; }

        public int? SeatID { get; set; }
        public Seat? Seat { get; set; }

        [Required]
        public string Status { get; set; }    // "Pending", "Confirmed", "Cancelled"

        [Required]
        public decimal FarePerSeat { get; set; }

        public decimal RefundAmount { get; set; }
    }
}

