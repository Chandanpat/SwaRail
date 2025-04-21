using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Railway_Reservation_Sys.Models
{
    public class Payment
    {
        
    [Key]
    public int PaymentID { get; set; }

    [Required]
    public int ReservationID { get; set; }
    
    [ForeignKey("ReservationID")]
    public ReservationHeader ReservationHeader { get; set; }

    [Required]
    public Guid UserID { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public string PaymentMethod { get; set; }

    [Required]
    public string AmtStatus { get; set; } //Pending, Paid And Refunded

    }
}