using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class GetTrainDTO
    {
        public int TrainID { get; set; }
        public string TrainName { get; set; }
        public string SourceStation { get; set; }
        public string DestinationStation { get; set; }
        public int TotalSeats { get; set; }
        public decimal Fare { get; set; }
        public string SourceDepartureTime { get; set; }
        public string DestiArrivalTime { get; set; }
    }
}