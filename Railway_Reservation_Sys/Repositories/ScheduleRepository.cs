using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Railway_Reservation_Sys.Interfaces;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly RailwayReservationDbContext _context;
        private readonly IReservationRepository usedReserveRepo;

        public ScheduleRepository(RailwayReservationDbContext context, IReservationRepository reserveRepo)
        {
            _context = context;
            usedReserveRepo = reserveRepo;
        }

        public async Task<bool> IsScheduleExistsAsync(int trainId, DateTime journeyDate)
        {
            return await _context.Schedules
                .AnyAsync(s => s.TrainID == trainId && s.JourneyDate.Date == journeyDate.Date);
        }

        public async Task<Schedule> AddScheduleWithCoachesAndSeatsAsync(int trainId, DateTime journeydate)
        {
            var train = await _context.Trains.FindAsync(trainId);
            if (train == null)
                throw new Exception("Train not found.");

            if (await IsScheduleExistsAsync(trainId, journeydate))
                throw new Exception($"Schedule for train on {journeydate:yyyy-MM-dd} already exists.");

            // Adding Schedule
            var schedule = new Schedule
            {
                TrainID = trainId,
                JourneyDate = journeydate
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            var coaches = new List<Coach>();
            var seats = new List<Seat>();

            // Adding Coaches
            string[] coachNumbers = { "A1", "A2", "S1", "S2" };

            for (int i = 0; i < coachNumbers.Length; i++)
            {
                var coach = new Coach
                {
                    ScheduleID = schedule.ScheduleID,
                    CoachNo = coachNumbers[i],
                    CoachType = (i < 2) ? "AC" : "Sleeper"
                };

                _context.Coaches.Add(coach);
                coaches.Add(coach);
            }

            await _context.SaveChangesAsync();

            // Adding Seats
            int seatPerCoach = 10;
            foreach (var coach in coaches)
            {
                for (int j = 1; j <= seatPerCoach; j++)
                {
                    seats.Add(new Seat
                    {
                        SeatNo = j,
                        CoachID = coach.CoachID,
                        SeatStatus = "Available"
                    });
                }
            }

            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return schedule;
        }

        public async Task<List<Schedule>> AddMultipleSchedulesWithCoachesAndSeatsAsync(int trainId, DateTime fromDate, DateTime toDate)
        {
            var train = await _context.Trains.FindAsync(trainId);
            if (train == null)
                throw new Exception("Train not found.");

            List<Schedule> addedSchedules = new List<Schedule>();

            for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                var schedule = await AddScheduleWithCoachesAndSeatsAsync(trainId, date);
                addedSchedules.Add(schedule);
            }

            return addedSchedules;
        }


        // public async Task<bool> DeleteScheduleWithCoachesAndSeatsAsync(int trainId, DateTime journeyDate)
        // {
        //     var train = await _context.Trains.FindAsync(trainId);
        //     if (train == null)
        //         throw new Exception($"Train ID {trainId} not found.");

        //     var schedule = await _context.Schedules
        //         .Where(s => s.TrainID == trainId && s.JourneyDate.Date == journeyDate.Date)
        //         .Include(s => s.Coaches)
        //         .ThenInclude(c => c.Seats)
        //         .FirstOrDefaultAsync();

        //     if (schedule == null)
        //         throw new Exception($"Schedule for Train ID {trainId} on {journeyDate:yyyy-MM-dd} not found.");

        //     foreach (var coach in schedule.Coaches)
        //     {
        //         _context.Seats.RemoveRange(coach.Seats);
        //     }

        //     _context.Coaches.RemoveRange(schedule.Coaches);

        //     _context.Schedules.Remove(schedule);

        //     await _context.SaveChangesAsync();
        //     return true;
        // }

        public async Task<bool> DeleteScheduleWithCoachesAndSeatsAsync(int trainId, DateTime journeyDate)
        {
            var train = await _context.Trains.FindAsync(trainId);
            if (train == null)
                throw new Exception($"Train ID {trainId} not found.");


            var schedule = await _context.Schedules
                .Where(s => s.TrainID == trainId && s.JourneyDate.Date == journeyDate.Date)
                .Include(s => s.Coaches)
                    .ThenInclude(c => c.Seats)
                .FirstOrDefaultAsync();

            if (schedule == null)
                throw new Exception($"Schedule for Train ID {trainId} on {journeyDate:yyyy-MM-dd} not found.");

            // Cancel all reservations before proceeding
            var reservations = await _context.ReservationHeaders
                .Where(r => r.ScheduleID == schedule.ScheduleID)
                .ToListAsync();

            foreach (var reservation in reservations)
            {
                await usedReserveRepo.CancelBookingWithoutUserAsync(reservation.ReservationID);
            }

            // Deleting Seats, Coaches and Schedule Stuff
            foreach (var coach in schedule.Coaches)
            {
                _context.Seats.RemoveRange(coach.Seats);
            }

            _context.Coaches.RemoveRange(schedule.Coaches);

            _context.Schedules.Remove(schedule);

            await _context.SaveChangesAsync();
            return true;
        }


        // public async Task<bool> DeleteMultipleSchedulesWithCoachesAndSeatsAsync(int trainId, DateTime fromDate, DateTime toDate)
        // {
        //     var train = await _context.Trains.FindAsync(trainId);
        //     if (train == null)
        //         throw new Exception($"Train ID {trainId} not found.");

        //     var schedules = await _context.Schedules
        //         .Where(s => s.TrainID == trainId && s.JourneyDate.Date >= fromDate.Date && s.JourneyDate.Date <= toDate.Date)
        //         .Include(s => s.Coaches)
        //         .ThenInclude(c => c.Seats)
        //         .ToListAsync();

        //     if (schedules.Count == 0)
        //         throw new Exception("No schedules found in the specified range.");

        //     foreach (var schedule in schedules)
        //     {
        //         foreach (var coach in schedule.Coaches)
        //         {
        //             _context.Seats.RemoveRange(coach.Seats);
        //         }
        //         _context.Coaches.RemoveRange(schedule.Coaches);
        //     }

        //     _context.Schedules.RemoveRange(schedules);

        //     await _context.SaveChangesAsync();
        //     return true;
        // }

        public async Task<bool> DeleteMultipleSchedulesWithCoachesAndSeatsAsync(int trainId, DateTime fromDate, DateTime toDate)
        {
            var train = await _context.Trains.FindAsync(trainId);
            if (train == null)
                throw new Exception($"Train ID {trainId} not found.");

            // Get all schedules for the train between given dates
            var schedules = await _context.Schedules
                .Where(s => s.TrainID == trainId && s.JourneyDate.Date >= fromDate.Date && s.JourneyDate.Date <= toDate.Date)
                .Include(s => s.Coaches)
                    .ThenInclude(c => c.Seats)
                .ToListAsync();

            if (schedules.Count == 0)
                throw new Exception("No schedules found in the specified range.");

            foreach (var schedule in schedules)
            {
                // Cancel reservations for each schedule
                var reservations = await _context.ReservationHeaders
                    .Where(r => r.ScheduleID == schedule.ScheduleID)
                    .ToListAsync();

                foreach (var reservation in reservations)
                {
                    await usedReserveRepo.CancelBookingWithoutUserAsync(reservation.ReservationID);
                }

                // Remove seats, coaches, and schedule
                foreach (var coach in schedule.Coaches)
                {
                    _context.Seats.RemoveRange(coach.Seats);
                }

                _context.Coaches.RemoveRange(schedule.Coaches);
            }

            _context.Schedules.RemoveRange(schedules);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<List<Schedule>> GetSchedulesByTrainIDAsync(int TrainId)
        {
            var schedule = await _context.Schedules
                .Where(s => s.TrainID == TrainId)
                .AsNoTracking()
                .ToListAsync();

            return schedule;
        }

        public async Task<List<Train>> GetTrainsOnDateAsync(DateTime journeyDate)
        {
            var trainIds = await _context.Schedules
                .Where(s => s.JourneyDate.Date == journeyDate.Date)
                .Select(s => s.TrainID)
                .Distinct()
                .ToListAsync();

            var trains = await _context.Trains
                .Where(t => trainIds.Contains(t.TrainID))
                .Include(t => t.SourceStation)
                .Include(t => t.DestinationStation)
                .ToListAsync();

            return trains;
        }

        public async Task<int?> GetScheduleIdByTrainAndDateAsync(int trainId, DateTime journeyDate)
        {
            var schedule = await _context.Schedules
                .Where(s => s.TrainID == trainId && s.JourneyDate.Date == journeyDate.Date)
                .Select(s => s.ScheduleID)
                .FirstOrDefaultAsync();

            return schedule == 0 ? (int?)null : schedule;
        }

        public async Task<Schedule> GetSchedulesByIDAsync(int scheduleId)
        {
            var schedule = await _context.Schedules
                .Where(s => s.ScheduleID == scheduleId)
                .FirstOrDefaultAsync();

            return schedule;
        }

        public async Task<List<Schedule>> GetAllSchedulesAsync()
        {
            return await _context.Schedules
                .Include(s => s.Train)
                .OrderByDescending(s => s.ScheduleID)
                .ToListAsync();
        }


    }
}