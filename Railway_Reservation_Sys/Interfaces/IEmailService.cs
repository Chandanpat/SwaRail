using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Interfaces
{
    public interface IEmailService
    {
        Task SendETicketAsync(User user, ReservationHeader reservation);
        Task SendCancellationEmailAsync(User user, ReservationHeader reservation);
    }
}