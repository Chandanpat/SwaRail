using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Railway_Reservation_Sys.Models
{
    public class Station
    {
        [Key]
        public int StationID { get; set; }

        [Required]
        public string StationName { get; set; }
    }
}