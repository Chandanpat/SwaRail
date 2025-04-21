using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Railway_Reservation_Sys.Models
{
    public class Passenger
    {[Key]
    public int PassengerID { get; set; } 

    [Required]
    public string Name { get; set; }

    [Required]
    // [RegularExpression("Male|Female|Other")]
    public string Sex { get; set; }

    [Required]
    public int Age { get; set; }

    // [Required]
    public string Address { get; set; }

    public Guid? UserID { get; set; } // Nullable
    public User? User { get; set; }

    public ICollection<ReservationDetails> ReservationDetails { get; set; }
    }
}