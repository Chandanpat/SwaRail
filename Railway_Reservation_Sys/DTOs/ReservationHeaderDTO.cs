using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class ReservationHeaderDTO
{
    public int ReservationID { get; set; }
    public Guid UserID { get; set; }
    public string TrainName { get; set; }
    public DateTime JourneyDate { get; set; }
    public decimal TotalFare { get; set; }
    public string Status { get; set; }
    public decimal TotalRefund { get; set; }

}
}