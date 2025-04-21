using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Railway_Reservation_Sys.DTOs;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Train>> SearchTrainsAsync(string sourceStationName, string destinationStationName, DateTime journeyDate);
        Task<int> GetAvailableSleeperSeatsAsync(int scheduleId);
        Task<int> GetAvailableACSeatsAsync(int scheduleId);
        Task<ReservationHeader> BookTicketAsync(ClaimsPrincipal user, int trainId, int scheduleId, List<Passenger> passengers, string coachType);
        Task<Payment> MakePaymentAsync(ClaimsPrincipal user, Payment payment);
        Task<ReservationHeader> ShowTicketAsync(int reservationId);
        Task<bool> CancelBookingAsync(ClaimsPrincipal user, int reservationID, List<int> passengerIDs);
        Task<bool> ProcessRefundAsync(int reservationID, decimal refundAmount);
        Task<List<ReservationHeader>> GetUserBookingsAsync(ClaimsPrincipal user);
        Task<bool> CancelBookingWithoutUserAsync(int reservationID);

    }
}