using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Railway_Reservation_Sys.DTOs;
using Railway_Reservation_Sys.Interfaces;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private readonly IReservationRepository _reserveRepo;
        private readonly IMapper _map;

        public ReservationController(IReservationRepository reserveRepo, IMapper mapper)
        {
            _reserveRepo = reserveRepo;
            _map = mapper;
        }

        [HttpGet("SearchTrains")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SearchTrains([FromQuery] string sourceStation, [FromQuery] string destinationStation, [FromQuery] DateTime journeyDate)
        {
            if (string.IsNullOrEmpty(sourceStation))
            {
                return BadRequest("Invalid Source Station.");
            }

            if (string.IsNullOrEmpty(destinationStation))
            {
                return BadRequest("Invalid Destination Station");
            }

            if (string.IsNullOrEmpty(sourceStation) && string.IsNullOrEmpty(destinationStation))
            {
                return BadRequest("Invalid Stations Inputs.");
            }


            var trains = await _reserveRepo.SearchTrainsAsync(sourceStation, destinationStation, journeyDate);

            if (trains == null || trains.Count == 0)
            {
                return NotFound("No Trains Found.");
            }

            var trainDTOs = new List<TrainDTO>();

            foreach (var train in trains)
            {
                var schedule = train.Schedules.FirstOrDefault(s => s.JourneyDate.Date == journeyDate.Date && journeyDate.Date >= DateTime.Now.Date);
                // var schedule = train.Schedules.FirstOrDefault(s => s.JourneyDate.Date == journeyDate.Date);
                if (schedule == null) continue;

                // available seats
                int availableSleeperSeats = await _reserveRepo.GetAvailableSleeperSeatsAsync(schedule.ScheduleID);
                int availableACSeats = await _reserveRepo.GetAvailableACSeatsAsync(schedule.ScheduleID);

                var trainDTO = _map.Map<TrainDTO>(train);
                trainDTO.AvailableSleeperSeats = availableSleeperSeats;
                trainDTO.AvailableACSeats = availableACSeats;

                trainDTOs.Add(trainDTO);
            }

            return Ok(trainDTOs);
        }

        [HttpPost("BookTicket")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookTicket([FromBody] ReservationRequestDTO request)
        {
            if (request == null || request.Passengers.Count == 0)
            {
                return BadRequest("Invalid booking request.");
            }

            if (request.Passengers.Count > 6)
            {
                return BadRequest("Booking of only 6 passengers is allowed at a time.");
            }

            var passengers = _map.Map<List<Passenger>>(request.Passengers);

            var reservation = await _reserveRepo.BookTicketAsync(User, request.TrainID, request.ScheduleID, passengers, request.CoachType);

            var reservationDTO = _map.Map<ReservationHeaderDTO>(reservation);

            return Ok(new { ReservationID = reservationDTO.ReservationID, Message = "Reservation created. Proceed to payment." });
        }

        [HttpPost("MakePayment")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MakePayment([FromBody] GetPaymentDTO paymentDto)
        {
            try
            {
                var paymentModel = _map.Map<Payment>(paymentDto);
                var payment = await _reserveRepo.MakePaymentAsync(User, paymentModel);
                var ticket = await _reserveRepo.ShowTicketAsync(payment.ReservationID);
                var ticketDto = _map.Map<TicketDTO>(ticket);
                return Ok(ticketDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("ShowTicket/{reservationId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ShowTicket(int reservationId)
        {
            var reservation = await _reserveRepo.ShowTicketAsync(reservationId);
            var ticketDto = _map.Map<TicketDTO>(reservation);

            if (reservation.Status == "Partially Cancelled" || reservation.Status == "Cancelled")
            {
                // Fetching Refund Details
                var refundPayment = reservation.Payments.FirstOrDefault(p => p.AmtStatus == "Refunded");
                var refundDTO = refundPayment != null ? _map.Map<PaymentDTO>(refundPayment) : null;
                return Ok(new
                {
                    UpdatedTicket = ticketDto,
                    RefundDetails = refundDTO
                });
            }

            return Ok(new
            {
                UpdatedTicket = ticketDto
            });
        }

        [HttpPost("CancelBooking")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequestDTO request)
        {
            if (request == null || request.PassengerIDs.Count == 0)
            {
                return BadRequest("Invalid cancellation request.");
            }

            bool success = await _reserveRepo.CancelBookingAsync(User, request.ReservationID, request.PassengerIDs);

            if (!success)
            {
                return BadRequest("Cancellation failed.");
            }

            // Fetching Ticket
            var reservation = await _reserveRepo.ShowTicketAsync(request.ReservationID);
            var ticketDTO = _map.Map<TicketDTO>(reservation);

            // Fetching Refund
            var refundPayment = reservation.Payments.FirstOrDefault(p => p.AmtStatus == "Refunded");
            var refundDTO = refundPayment != null ? _map.Map<PaymentDTO>(refundPayment) : null;

            return Ok(new
            {
                Message = "Booking cancelled successfully.",
                UpdatedTicket = ticketDTO,
                RefundDetails = refundDTO
            });
        }

        [HttpGet("UserBookings")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserBookings()
        {
            try
            {
                var bookings = await _reserveRepo.GetUserBookingsAsync(User);

                if (bookings == null || bookings.Count == 0)
                {
                    return NotFound("No bookings found.");
                }

                var ticketDTOs = _map.Map<List<TicketDTO>>(bookings);
                return Ok(ticketDTOs);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // [HttpPost("admin-cancel-reservation/{reservationId}")]
        // public async Task<IActionResult> AdminCancelReservation(int reservationId)
        // {
        //     try
        //     {
        //         bool result = await _reserveRepo.CancelBookingWithoutUserAsync(reservationId);

        //         if (result)
        //             return Ok(new { success = true, message = $"Reservation {reservationId} cancelled successfully." });

        //         return BadRequest(new { success = false, message = "Cancellation failed." });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new { success = false, error = ex.Message });
        //     }
        // }
    }
}