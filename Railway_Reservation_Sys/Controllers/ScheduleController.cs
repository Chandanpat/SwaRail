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
    public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _scheduleRepo;
        private readonly IMapper _map;

        public ScheduleController(IScheduleRepository scheduleRepo, IMapper mapper)
        {
            _scheduleRepo = scheduleRepo;
            _map = mapper;
        }

        [HttpPost("AddScheduleWithCoachesAndSeats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddScheduleWithCoachesAndSeats([FromBody] AddScheduleDTO scheduleDTO)
        {
            try
            {
                var schedule = await _scheduleRepo.AddScheduleWithCoachesAndSeatsAsync(scheduleDTO.TrainID, scheduleDTO.JourneyDate);
                return Ok(_map.Map<ScheduleDTO>(schedule));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }

        [HttpPost("AddMultipleSchedulesWithCoachesAndSeats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMultipleSchedulesWithCoachesAndSeats([FromBody] AddMultiScheduleDTO scheduleDTO)
        {
            try
            {
                var addedSchedules = await _scheduleRepo.AddMultipleSchedulesWithCoachesAndSeatsAsync(
                               scheduleDTO.TrainID, scheduleDTO.FromJourneyDate, scheduleDTO.ToJourneyDate);

                return Ok(_map.Map<List<ScheduleDTO>>(addedSchedules));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }

        [HttpDelete("DeleteScheduleWithCoachesAndSeats")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteScheduleWithCoachesAndSeats([FromBody] AddScheduleDTO scheduleDTO)
        {
            try
            {
                bool deleted = await _scheduleRepo.DeleteScheduleWithCoachesAndSeatsAsync(scheduleDTO.TrainID, scheduleDTO.JourneyDate);
                if (!deleted)
                    return NotFound($"Schedule for Train ID {scheduleDTO.TrainID} on {scheduleDTO.JourneyDate:yyyy-MM-dd} not found.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("DeleteMultipleSchedulesWithCoachesAndSeats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMultipleSchedulesWithCoachesAndSeats([FromBody] AddMultiScheduleDTO request)
        {
            try
            {
                bool deleted = await _scheduleRepo.DeleteMultipleSchedulesWithCoachesAndSeatsAsync(
                    request.TrainID, request.FromJourneyDate, request.ToJourneyDate);

                if (!deleted)
                    return NotFound("No schedules found in the specified range.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetScheduleByTrainID")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetScheduleByTrainID([FromQuery] int trainId)
        {
            var schedules = await _scheduleRepo.GetSchedulesByTrainIDAsync(trainId);
            if (schedules == null || !schedules.Any())
                return NotFound("No schedules found for given Train ID.");

            return Ok(_map.Map<List<ScheduleDTO>>(schedules));
        }

        [HttpGet("GetTrainsOnDate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTrainsOnDate([FromQuery] DateTime journeyDate)
        {
            var trains = await _scheduleRepo.GetTrainsOnDateAsync(journeyDate);
            if (trains == null || trains.Count == 0)
                return NotFound($"Trains on {journeyDate:yyyy-MM-dd} not scheduled.");

            return Ok(_map.Map<List<GetTrainDTO>>(trains));
        }

        [HttpGet("GetScheduleID")]
        [Authorize]
        public async Task<IActionResult> GetScheduleID([FromQuery] int trainId, [FromQuery] DateTime journeyDate)
        {
            if (trainId <= 0)
            {
                return BadRequest("Invalid Train ID.");
            }

            if (journeyDate.Date < DateTime.Now.Date)
            {
                return BadRequest("Journey Date cannot be in the past.");
            }

            var scheduleId = await _scheduleRepo.GetScheduleIdByTrainAndDateAsync(trainId, journeyDate);

            if (scheduleId == null)
            {
                return NotFound($"No schedule found for Train ID {trainId} on {journeyDate:yyyy-MM-dd}.");
            }

            return Ok(new { ScheduleID = scheduleId });
        }

        [HttpGet("GetScheduleByID")]
        [Authorize]
        public async Task<ActionResult> GetScheduleByID([FromQuery] int scheduleIdId)
        {
            var schedule = await _scheduleRepo.GetSchedulesByIDAsync(scheduleIdId);

            return Ok(_map.Map<ScheduleDTO>(schedule));
        }

        [HttpGet("GetAllSchedules")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllSchedules()
        {
            var schedule = await _scheduleRepo.GetAllSchedulesAsync();

            return Ok(_map.Map<List<ScheduleDTO>>(schedule));
        }
    }
}