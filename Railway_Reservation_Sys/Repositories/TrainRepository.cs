using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Railway_Reservation_Sys.Interfaces;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Repositories
{
    public class TrainRepository : ITrainRepository
    {
        private readonly RailwayReservationDbContext _context;
        public TrainRepository(RailwayReservationDbContext context)
        {
            _context = context;
        }




        public async Task<Train?> AddTrainAsync(Train train)
        {
            _context.Trains.Add(train);
            await _context.SaveChangesAsync();

            // return train;
            return await _context.Trains
                .Include(t => t.SourceStation)
                .Include(t => t.DestinationStation)
                .FirstOrDefaultAsync(t => t.TrainID == train.TrainID);
        }

        public async Task<List<Train>> GetAllTrainsAsync()
        {
            return await _context.Trains
                .Include(t => t.SourceStation)
                .Include(t => t.DestinationStation)
                .ToListAsync();
        }

        public async Task<Train> TrainExistsAsync(int trainId)
        {
            return await _context.Trains
                    .Include(t => t.SourceStation)
                    .Include(t => t.DestinationStation)
                    .FirstOrDefaultAsync(t => t.TrainID == trainId);
        }

        public async Task<Train> GetTrainByTrainNameAsync(string trainName)
        {
            var train = await _context.Trains
                .Include(t => t.SourceStation)
                .Include(t => t.DestinationStation)
                .FirstOrDefaultAsync(t => t.TrainName.Trim().ToLower() == trainName.Trim().ToLower());

            if (train == null)
                throw new Exception($"Train '{trainName}' not found.");

            return train;
        }

        public async Task<Train> GetTrainByIDAsync(int trainId)
        {
            var train = await _context.Trains
                .Include(t => t.SourceStation)
                .Include(t => t.DestinationStation)
                .FirstOrDefaultAsync(t => t.TrainID == trainId);

            if (train == null)
                throw new Exception($"Train not found.");

            return train;
        }

        public async Task<Train> UpdateTrainAsync(int trainId, Train updatedTrain)
        {
            var train = await _context.Trains.FindAsync(trainId);
            if (train == null) return null;

            train.TrainName = updatedTrain.TrainName;
            train.SourceID = updatedTrain.SourceID;
            train.DestinationID = updatedTrain.DestinationID;
            train.TotalSeats = updatedTrain.TotalSeats;
            train.Fare = updatedTrain.Fare;
            train.SourceDepartureTime = updatedTrain.SourceDepartureTime;
            train.DestiArrivalTime = updatedTrain.DestiArrivalTime;

            await _context.SaveChangesAsync();
            // return train;
            return await _context.Trains
                .Include(t => t.SourceStation)
                .Include(t => t.DestinationStation)
                .FirstOrDefaultAsync(t => t.TrainID == train.TrainID);
        }

        public async Task<bool> DeleteTrainAsync(int trainId)
        {
            var train = await _context.Trains.FindAsync(trainId);
            if (train == null) return false;

            _context.Trains.Remove(train);
            await _context.SaveChangesAsync();
            return true;
        }

        // public async Task<bool> DeleteTrainWithSchedulesAsync(int trainId, DateTime deletionDate)
        // {
        //     var train = await _context.Trains.FindAsync(trainId);
        //     if (train == null)
        //         throw new Exception($"Train ID {trainId} not found.");

        //     var schedules = await _context.Schedules
        //         .Where(s => s.TrainID == trainId && s.JourneyDate.Date >= deletionDate.AddDays(1).Date)
        //         .Include(s => s.Coaches)
        //         .ThenInclude(c => c.Seats)
        //         .ToListAsync();

        //     if (schedules.Count == 0)
        //         throw new Exception($"No schedules found for Train ID {trainId} from {deletionDate.AddDays(1):yyyy-MM-dd} onwards.");

        //     var reservations = await _context.ReservationHeaders
        //         .Where(r => schedules.Select(s => s.ScheduleID).Contains(r.ScheduleID) && r.Status != "Cancelled")
        //         .Include(r => r.ReservationDetails)
        //         .ThenInclude(rd => rd.Seat)
        //         .Include(r => r.Payments)
        //         .ToListAsync();

        //     foreach (var reservation in reservations)
        //     {
        //         await usedReserveRepo.CancelBookingWithoutUserAsync(reservation.ReservationID);
        //     }

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

        //     _context.Trains.Remove(train);
        //     await _context.SaveChangesAsync();

        //     return true;
        // }



    }
}


