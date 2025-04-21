using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Railway_Reservation_Sys.Models
{
    public class User
    {
        [Key]
        // public int UserID { get; set; }
        public Guid UserID { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }    //User Or Admin

        public ICollection<ReservationHeader> ReservationHeaders { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}