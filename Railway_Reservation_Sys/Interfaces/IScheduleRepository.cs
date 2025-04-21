using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Interfaces
{
    public interface IScheduleRepository
    {
        Task<bool> IsScheduleExistsAsync(int trainId, DateTime journeyDate);
        Task<Schedule> AddScheduleWithCoachesAndSeatsAsync(int trainId, DateTime journeydate);
        Task<List<Schedule>> AddMultipleSchedulesWithCoachesAndSeatsAsync(int trainId, DateTime fromDate, DateTime toDate);
        Task<bool> DeleteScheduleWithCoachesAndSeatsAsync(int trainId, DateTime journeyDate);
        Task<bool> DeleteMultipleSchedulesWithCoachesAndSeatsAsync(int trainId, DateTime fromDate, DateTime toDate);
        Task<List<Schedule>> GetSchedulesByTrainIDAsync(int trainId);
        Task<List<Train>> GetTrainsOnDateAsync(DateTime journeyDate);
        Task<int?> GetScheduleIdByTrainAndDateAsync(int trainId, DateTime journeyDate);
        Task<Schedule> GetSchedulesByIDAsync(int scheduleId);
        Task<List<Schedule>> GetAllSchedulesAsync();
    }
}


