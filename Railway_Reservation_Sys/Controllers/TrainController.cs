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
    public class TrainController : Controller
    {
        private readonly ITrainRepository _trainRepo;
        private readonly IMapper _map;

        public TrainController(ITrainRepository trainRepo, IMapper mapper)
        {
            _trainRepo = trainRepo;
            _map = mapper;
        }


        [HttpPost("AddTrain")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTrain([FromBody] AddTrainDTO trainDTO)
        {
            if (trainDTO == null)
                return BadRequest("Invalid train data.");

            var train = _map.Map<Train>(trainDTO);

            var addedTrain = await _trainRepo.AddTrainAsync(train);
            return Ok(_map.Map<GetTrainDTO>(addedTrain));
        }

        [HttpGet("GetAllTrains")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTrains()
        {
            var trains = await _trainRepo.GetAllTrainsAsync();
            return Ok(_map.Map<List<GetTrainDTO>>(trains));
        }

        [HttpGet("TrainExists/{trainId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TrainExists(int trainId)
        {
            var exists = await _trainRepo.TrainExistsAsync(trainId);
            if (exists == null)
                return NotFound("Train not found.");

            return Ok(_map.Map<GetTrainDTO>(exists));
        }

        [HttpGet("GetTrainByID")]
        [Authorize]
        public async Task<IActionResult> GetTrainByID([FromQuery] int trainId)
        {
            try
            {
                var train = await _trainRepo.GetTrainByIDAsync(trainId);
                return Ok(_map.Map<GetTrainDTO>(train));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetTrainByTrainName")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTrainByTrainName([FromQuery] string trainName)
        {
            try
            {
                var train = await _trainRepo.GetTrainByTrainNameAsync(trainName);
                return Ok(_map.Map<GetTrainDTO>(train));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("UpdateTrain/{trainId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTrain(int trainId, [FromBody] AddTrainDTO trainDTO)
        {
            var train = _map.Map<Train>(trainDTO);
            var updatedTrain = await _trainRepo.UpdateTrainAsync(trainId, train);

            if (updatedTrain == null)
                return NotFound("Train not found.");

            return Ok(_map.Map<GetTrainDTO>(updatedTrain));
        }

        // [HttpDelete("DeleteTrain/{trainId}")]
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> DeleteTrain(int trainId)
        // {
        //     bool success = await _trainRepo.DeleteTrainAsync(trainId);
        //     if (!success) return NotFound("Train not found.");
        //     return Ok("Train deleted successfully.");
        // }

        // [HttpDelete("delete-train/{trainId}")]
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> DeleteTrainWithSchedules(int trainId, [FromQuery] DateTime deletionDate)
        // {
        //     try
        //     {
        //         bool isDeleted = await _trainRepo.DeleteTrainWithSchedulesAsync(trainId, deletionDate);

        //         if (isDeleted)
        //             return Ok(new { message = $"Train ID {trainId} and all future schedules deleted successfully." });

        //         return BadRequest(new { error = "Failed to delete train or schedules." });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new { error = ex.Message });
        //     }
        // }

    }
}

