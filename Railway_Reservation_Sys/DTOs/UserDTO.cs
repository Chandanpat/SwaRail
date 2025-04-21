using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class UserDTO
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // "User" or "Admin"
    }
}