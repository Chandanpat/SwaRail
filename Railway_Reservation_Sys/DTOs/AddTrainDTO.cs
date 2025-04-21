using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.DTOs
{
    public class AddTrainDTO
    {
        
        // public int TrainID { get; set; }
        public string TrainName { get; set; }
        public int SourceID { get; set; }
        public int DestinationID { get; set; }
        public int TotalSeats { get; set; }
        public decimal Fare { get; set; }
        public string SourceDepartureTime { get; set; }
        public string DestiArrivalTime { get; set; }
    }
}