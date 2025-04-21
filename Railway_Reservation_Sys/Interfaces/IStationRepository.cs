using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Interfaces
{
    public interface IStationRepository
    {
        Task<Station> AddStationAsync(Station station);
        Task<List<Station>> GetAllStationsAsync();
        Task<Station> GetStationByNameAsync(string stationName);
        Task<Station> UpdateStationAsync(int stationId, Station station);
        Task<bool> DeleteStationAsync(int stationId);
        
    }
}