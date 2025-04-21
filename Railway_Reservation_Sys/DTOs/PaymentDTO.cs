using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class PaymentDTO
    {
        public int PaymentID { get; set; }
        public int ReservationID { get; set; } // PNR
        public Guid UserID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string AmtStatus { get; set; } // "Paid" or "Refunded"
    }


}