using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Railway_Reservation_Sys.Interfaces;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly RailwayReservationDbContext _context;

        public StationRepository(RailwayReservationDbContext context)
        {
            _context = context;
        }

        public async Task<Station> AddStationAsync(Station station)
        {
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();
            return station;
        }

        public async Task<List<Station>> GetAllStationsAsync()
        {
            return await _context.Stations.ToListAsync();
        }

        public async Task<Station> GetStationByNameAsync(string stationName)
        {
            var station = await _context.Stations
                .FirstOrDefaultAsync(t => t.StationName.Trim().ToLower() == stationName.Trim().ToLower());

            if (station == null)
                throw new Exception($"Train '{stationName}' not found.");

            return station;
        }

        public async Task<Station> UpdateStationAsync(int stationId, Station station)
        {
            var existingStation = await _context.Stations.FirstOrDefaultAsync(s => s.StationID == stationId);

            if (existingStation == null)
            {
                throw new ArgumentException("Station not found.");
            }

            existingStation.StationName = station.StationName;

            _context.Stations.Update(existingStation);
            await _context.SaveChangesAsync();

            return existingStation;
        }


        public async Task<bool> DeleteStationAsync(int stationId)
        {
            var station = await _context.Stations.FindAsync(stationId);
            if (station == null) return false;

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}