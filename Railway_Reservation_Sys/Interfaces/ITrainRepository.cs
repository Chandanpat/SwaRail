using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Interfaces
{
    public interface ITrainRepository
    {
        
        Task<Train?> AddTrainAsync(Train train);
        Task<List<Train>> GetAllTrainsAsync();
        Task<Train> TrainExistsAsync(int trainId);
        Task<Train> GetTrainByTrainNameAsync(string trainName);
        Task<Train> GetTrainByIDAsync(int trainId);
        Task<Train> UpdateTrainAsync(int trainId, Train updatedTrain);
        Task<bool> DeleteTrainAsync(int trainId);
    }
}

