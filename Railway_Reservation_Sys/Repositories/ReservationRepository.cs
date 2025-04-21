using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Railway_Reservation_Sys.DTOs;
using Railway_Reservation_Sys.Interfaces;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly RailwayReservationDbContext _context;
        private readonly IEmailService _emailService;

        public ReservationRepository(RailwayReservationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<Train>> SearchTrainsAsync(string sourceStationName, string destinationStationName, DateTime journeyDate)
        {
            return await _context.Trains
                .Include(t => t.Schedules)
                .Include(t => t.SourceStation)
                .Include(t => t.DestinationStation)
                .Where(t => t.SourceStation.StationName == sourceStationName &&
                            t.DestinationStation.StationName == destinationStationName &&
                            t.Schedules.Any(s => s.JourneyDate.Date == journeyDate.Date && journeyDate.Date >= DateTime.Now.Date))
                // t.Schedules.Any(s => s.JourneyDate.Date == journeyDate.Date))
                .ToListAsync();
        }

        public async Task<int> GetAvailableSleeperSeatsAsync(int scheduleId)
        {
            return await _context.Seats
                .Include(s => s.Coach)
                .Where(s => s.Coach.ScheduleID == scheduleId &&
                            s.Coach.CoachType == "Sleeper" &&
                            s.SeatStatus == "Available")
                .CountAsync();
        }

        public async Task<int> GetAvailableACSeatsAsync(int scheduleId)
        {
            return await _context.Seats
                .Include(s => s.Coach)
                .Where(s => s.Coach.ScheduleID == scheduleId &&
                            s.Coach.CoachType == "AC" &&
                            s.SeatStatus == "Available")
                .CountAsync();
        }

        // public async Task<ReservationHeader> BookTicketAsync(Guid userId, int trainId, int scheduleId, List<Passenger> passengers, string coachType)
        public async Task<ReservationHeader> BookTicketAsync(ClaimsPrincipal user, int trainId, int scheduleId, List<Passenger> passengers, string coachType)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");


            if (passengers == null || passengers.Count == 0)
            {
                throw new ArgumentException("At least one passenger is required.");
            }

            if (passengers.Count > 6)
            {
                throw new ArgumentException("Booking of only 6 passengers is allowed at a time.");
            }

            // Fetching Train & Schedule
            var train = await _context.Trains.FindAsync(trainId);
            var schedule = await _context.Schedules.FindAsync(scheduleId);

            if (train == null || schedule == null)
            {
                throw new ArgumentException("Invalid Train ID or Schedule ID.");
            }

            // Fetching the coach for the given train and schedule
            var coach = await _context.Coaches
                .Where(c => c.ScheduleID == scheduleId && c.CoachType == coachType)
                .FirstOrDefaultAsync();

            if (coach == null)
            {
                throw new ArgumentException($"No {coachType} coach available for the selected train and schedule.");
            }

            //Creating PNR Without Seat Allocation
            var reservationHeader = new ReservationHeader
            {
                UserID = Guid.Parse(userId),
                TrainID = trainId,
                ScheduleID = scheduleId,
                Status = "Pending",
                TotalFare = 0,
                TotalRefund = 0
            };

            _context.ReservationHeaders.Add(reservationHeader);
            await _context.SaveChangesAsync();

            decimal totalFare = 0;

            // Store Each Passenger Without Seat Allocation
            foreach (var passenger in passengers)
            {
                var existingPassenger = await _context.Passengers
                    .FirstOrDefaultAsync(p => p.Name == passenger.Name && p.Age == passenger.Age && p.Sex == passenger.Sex && p.Address == passenger.Address);

                if (existingPassenger == null)
                {

                    existingPassenger = new Passenger
                    {
                        Name = passenger.Name,
                        Sex = passenger.Sex,
                        Age = passenger.Age,
                        Address = passenger.Address,
                        UserID = Guid.Parse(userId)
                    };

                    _context.Passengers.Add(existingPassenger);
                    await _context.SaveChangesAsync();
                }

                decimal calculatedFare = 0;
                if (coach.CoachType == "Sleeper")
                {
                    calculatedFare = train.Fare;
                }
                else if (coach.CoachType == "AC")
                {
                    calculatedFare = train.Fare * 1.5m;
                }

                var reservationDetail = new ReservationDetails
                {
                    ReservationID = reservationHeader.ReservationID,
                    PassengerID = existingPassenger.PassengerID,
                    CoachID = coach.CoachID,
                    SeatID = null,
                    Status = "Pending",
                    FarePerSeat = calculatedFare,
                    RefundAmount = 0
                };

                _context.ReservationDetails.Add(reservationDetail);
                totalFare += reservationDetail.FarePerSeat;
            }

            reservationHeader.TotalFare = totalFare;
            _context.ReservationHeaders.Update(reservationHeader);

            await _context.SaveChangesAsync();

            return reservationHeader;
        }

        // public async Task<Payment> MakePaymentAsync(Payment payment)
        public async Task<Payment> MakePaymentAsync(ClaimsPrincipal user, Payment payment)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");

            // var reservation = await _context.ReservationHeaders
            //     .Include(r => r.ReservationDetails)
            //     .FirstOrDefaultAsync(r => r.ReservationID == payment.ReservationID && r.UserID == Guid.Parse(userId));

            var reservation = await _context.ReservationHeaders
                .Include(r => r.User)  // Include User for email
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Passenger)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Seat)
                        .ThenInclude(s => s.Coach)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Coach)
                .Include(r => r.Train)
                    .ThenInclude(t => t.SourceStation)
                .Include(r => r.Train)
                    .ThenInclude(t => t.DestinationStation)
                .Include(r => r.Schedule)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.ReservationID == payment.ReservationID && r.UserID == Guid.Parse(userId));

            if (reservation == null)
            {
                throw new ArgumentException("Invalid Reservation ID or User ID.");
            }

            if (reservation.Status == "Confirmed")
            {
                throw new InvalidOperationException("Payment already completed for this reservation.");
            }

            // if (payment.Amount < reservation.TotalFare)
            // {
            //     throw new ArgumentException("Insufficient payment amount.");
            // }

            var coachId = reservation.ReservationDetails.Select(rd => rd.CoachID).FirstOrDefault();
            var availableSeats = await _context.Seats
                .Where(s => s.CoachID == coachId && s.SeatStatus == "Available")
                .OrderBy(s => s.SeatID)
                .Take(reservation.ReservationDetails.Count)
                .ToListAsync();

            if (availableSeats.Count < reservation.ReservationDetails.Count)
            {
                foreach (var detail in reservation.ReservationDetails)
                {
                    detail.CoachID = null;
                    detail.Status = "Cancelled";
                    detail.FarePerSeat = 0;
                }
                reservation.TotalFare = 0;
                reservation.ScheduleID = null;
                reservation.Status = "Cancelled";

                _context.ReservationHeaders.Update(reservation);
                await _context.SaveChangesAsync();

                throw new InvalidOperationException("Not enough seats available.");
            }


            // Assigning Seats in ascending order to Passengers
            int index = 0;
            foreach (var detail in reservation.ReservationDetails)
            {
                detail.SeatID = availableSeats[index].SeatID;
                detail.Status = "Confirmed";
                availableSeats[index].SeatStatus = "Booked";
                index++;
            }

            payment.Amount = reservation.TotalFare;

            payment.UserID = Guid.Parse(userId);
            payment.PaymentDate = DateTime.Now;
            payment.AmtStatus = "Paid";
            _context.Payments.Add(payment);

            reservation.Status = "Confirmed";
            _context.ReservationHeaders.Update(reservation);
            await _context.SaveChangesAsync();

            // Sending Confirmation Email to User 
            await _emailService.SendETicketAsync(reservation.User, reservation);

            return payment;
        }

        public async Task<ReservationHeader> ShowTicketAsync(int reservationId)
        {
            var reservation = await _context.ReservationHeaders
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Passenger)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Seat)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Coach)
                .Include(r => r.Train)
                    .ThenInclude(t => t.SourceStation)
                .Include(r => r.Train)
                    .ThenInclude(t => t.DestinationStation)
                .Include(r => r.Schedule)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.ReservationID == reservationId);

            if (reservation == null)
            {
                throw new ArgumentException("Reservation not found.");
            }

            return reservation;
        }


        // public async Task<bool> CancelBookingAsync(int reservationID, List<int> passengerIDs)
        public async Task<bool> CancelBookingAsync(ClaimsPrincipal user, int reservationID, List<int> passengerIDs)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");

            // var reservation = await _context.ReservationHeaders
            //     .Include(r => r.User)
            //     .Include(r => r.ReservationDetails)
            //     .ThenInclude(rd => rd.Seat)
            //     .FirstOrDefaultAsync(r => r.ReservationID == reservationID && r.UserID == Guid.Parse(userId));

            var reservation = await _context.ReservationHeaders
                .Include(r => r.User)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Passenger)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Seat)
                        .ThenInclude(s => s.Coach)
                .Include(r => r.Train)
                    .ThenInclude(t => t.SourceStation)
                .Include(r => r.Train)
                    .ThenInclude(t => t.DestinationStation)
                .Include(r => r.Schedule)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.ReservationID == reservationID && r.UserID == Guid.Parse(userId));

            if (reservation == null)
            {
                throw new ArgumentException("Invalid Reservation ID.");
            }

            if (reservation.Status == "Cancelled")
            {
                throw new InvalidOperationException("This reservation has already been fully cancelled.");
            }

            bool fullCancellation = passengerIDs.Count == reservation.ReservationDetails.Count;

            decimal totalRefund = 0;

            foreach (var detail in reservation.ReservationDetails)
            {
                if (passengerIDs.Contains(detail.PassengerID))
                {
                    if (detail.SeatID != null)
                    {
                        var seat = await _context.Seats.FindAsync(detail.SeatID);
                        if (seat != null)
                        {
                            seat.SeatStatus = "Available";
                        }
                    }

                    // Remove CoachID & SeatID for canceled passengers
                    detail.SeatID = null;
                    detail.CoachID = null;
                    detail.Status = "Cancelled";

                    totalRefund += detail.FarePerSeat;
                    detail.RefundAmount = -detail.FarePerSeat;
                    detail.FarePerSeat = 0;
                }
            }

            //Processing Refund
            await ProcessRefundAsync(reservationID, totalRefund);

            reservation.TotalRefund += totalRefund;

            if (fullCancellation || (reservation.TotalRefund == reservation.TotalFare))
            {
                reservation.Status = "Cancelled";
            }
            else
            {
                reservation.Status = "Partially Cancelled";
            }

            _context.ReservationHeaders.Update(reservation);
            await _context.SaveChangesAsync();

            // Sending Cancellation Email to User 
            await _emailService.SendCancellationEmailAsync(reservation.User, reservation);

            return true;
        }

        public async Task<bool> ProcessRefundAsync(int reservationID, decimal refundAmount)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.ReservationID == reservationID && p.AmtStatus == "Paid");

            // if (payment == null)
            // {
            //     throw new ArgumentException("No valid payment found for this reservation.");
            // }

            if (refundAmount <= 0)
                throw new ArgumentException("Invalid refund amount.");

            var refundPayment = new Payment
            {
                ReservationID = reservationID,
                UserID = payment.UserID,
                Amount = -refundAmount, // Negative for refund
                PaymentDate = DateTime.Now,
                PaymentMethod = "NEFT",
                AmtStatus = "Refunded"
            };

            _context.Payments.Add(refundPayment);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ReservationHeader>> GetUserBookingsAsync(ClaimsPrincipal user)
        {
            // Extract UserID from Claims
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");

            var reservations = await _context.ReservationHeaders
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Passenger)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Seat)
                        .ThenInclude(s => s.Coach)
                .Include(r => r.Train)
                    .ThenInclude(t => t.SourceStation)
                .Include(r => r.Train)
                    .ThenInclude(t => t.DestinationStation)
                .Include(r => r.Schedule)
                .Include(r => r.Payments)
                // .Where(r => r.UserID == Guid.Parse(userId) && r.Status != "Pending")
                .Where(r => r.UserID == Guid.Parse(userId))
                .OrderByDescending(r => r.ReservationID)
                .ToListAsync();

            return reservations;
        }

        public async Task<bool> CancelBookingWithoutUserAsync(int reservationID)
        {
            var reservation = await _context.ReservationHeaders
                .Include(r => r.User)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Passenger)
                .Include(r => r.ReservationDetails)
                    .ThenInclude(rd => rd.Seat)
                        .ThenInclude(s => s.Coach)
                .Include(r => r.Train)
                    .ThenInclude(t => t.SourceStation)
                .Include(r => r.Train)
                    .ThenInclude(t => t.DestinationStation)
                .Include(r => r.Schedule)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.ReservationID == reservationID);

            if (reservation == null)
                throw new ArgumentException("Invalid Reservation ID.");

            if (reservation.Status == "Cancelled")
            {
                reservation.ScheduleID = null;
                _context.ReservationHeaders.Update(reservation);
                await _context.SaveChangesAsync();
                return true; // Already cancelled
            }

            if (reservation.Status == "Pending")
            {
                foreach (var detail in reservation.ReservationDetails)
                {
                    detail.CoachID = null;
                    detail.Status = "Cancelled";
                    detail.FarePerSeat = 0;
                }
                reservation.TotalFare = 0;
                reservation.ScheduleID = null;
                reservation.Status = "Cancelled";

                _context.ReservationHeaders.Update(reservation);
                await _context.SaveChangesAsync();
                return true;
            }

            // Identify all passengers in the reservation
            var passengerIDs = reservation.ReservationDetails.Select(rd => rd.PassengerID).ToList();
            bool fullCancellation = passengerIDs.Count == reservation.ReservationDetails.Count;

            decimal totalRefund = 0;

            foreach (var detail in reservation.ReservationDetails)
            {
                if (detail.SeatID != null)
                {
                    var seat = await _context.Seats.FindAsync(detail.SeatID);
                    if (seat != null)
                    {
                        seat.SeatStatus = "Available";
                    }
                }

                // Remove CoachID & SeatID for canceled passengers
                detail.SeatID = null;
                detail.CoachID = null;
                detail.Status = "Cancelled";

                totalRefund += detail.FarePerSeat;
                detail.RefundAmount = -detail.FarePerSeat;
                detail.FarePerSeat = 0;
            }

            // Process Refund
            await ProcessRefundAsync(reservationID, totalRefund);

            reservation.TotalRefund += totalRefund;
            reservation.ScheduleID = null;
            if (fullCancellation || (reservation.TotalRefund == reservation.TotalFare))
            {
                reservation.Status = "Cancelled";
            }
            else
            {
                reservation.Status = "Partially Cancelled";
            }

            _context.ReservationHeaders.Update(reservation);
            await _context.SaveChangesAsync();

            // Send cancellation email
            await _emailService.SendCancellationEmailAsync(reservation.User, reservation);

            return true;
        }

    }
}

