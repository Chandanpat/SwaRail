using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class TicketDTO
    {
        public int PNR { get; set; }
        public string TrainName { get; set; }
        public string JourneyDate { get; set; }
        public string SourceStation { get; set; }
        public string SourceDepartureTime { get; set; }
        public string DestinationStation { get; set; }
        public string DestiArrivalTime { get; set; }
        public List<TicketPassengerDTO> Passengers { get; set; }
        public string Status { get; set; }
        public decimal TotalFare { get; set; }
        // public string PaymentStatus { get; set; }
        public int PaymentID { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string AmtStatus { get; set; }
        public decimal TotalRefund { get; set; }
    }
}