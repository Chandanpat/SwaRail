using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Railway_Reservation_Sys.DTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName {get; set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        public string[] Roles {get; set;}
    }
}