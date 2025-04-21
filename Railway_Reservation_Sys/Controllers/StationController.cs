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
    public class StationController : Controller
    {
        private readonly IStationRepository _stationRepo;
        private readonly IMapper _map;

        public StationController(IStationRepository stationRepo, IMapper mapper)
        {
            _stationRepo = stationRepo;
            _map = mapper;
        }

        [HttpPost("AddStation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddStation([FromBody] AddStationDTO stationDTO)
        {
            var station = _map.Map<Station>(stationDTO);
            var addedStation = await _stationRepo.AddStationAsync(station);
            return Ok(_map.Map<StationDTO>(addedStation));
        }

        [HttpGet("GetAllStations")]
        [Authorize]
        public async Task<IActionResult> GetAllStations()
        {
            var stations = await _stationRepo.GetAllStationsAsync();
            return Ok(_map.Map<List<StationDTO>>(stations));
        }

        [HttpGet("GetStationByName")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStationByName([FromQuery] string stationName)
        {
            try
            {
                var train = await _stationRepo.GetStationByNameAsync(stationName);
                return Ok(_map.Map<StationDTO>(train));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("UpdateStation/{stationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTrain(int stationId, [FromBody] AddStationDTO stationDTO)
        {
             var station = _map.Map<Station>(stationDTO);
            var updatedStation = await _stationRepo.UpdateStationAsync(stationId, station);
            return Ok(_map.Map<StationDTO>(updatedStation));
        }

        [HttpDelete("DeleteStation/{stationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStation(int stationId)
        {
            bool success = await _stationRepo.DeleteStationAsync(stationId);
            if (!success) return NotFound("Station not found.");
            return Ok("Station deleted successfully.");
        }

        
    }
}