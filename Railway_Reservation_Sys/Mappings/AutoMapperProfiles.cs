using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Railway_Reservation_Sys.DTOs;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDTO>()
                .ReverseMap();

            CreateMap<Station, StationDTO>()
                .ReverseMap();

            CreateMap<Station, AddStationDTO>()
                .ReverseMap();

            CreateMap<Train, TrainDTO>()
                .ForMember(dt => dt.SourceStation, opt => opt.MapFrom(t => t.SourceStation.StationName))
                .ForMember(dt => dt.DestinationStation, opt => opt.MapFrom(t => t.DestinationStation.StationName))
                .ForMember(dt => dt.SourceDepartureTime, opt => opt.MapFrom(t => t.SourceDepartureTime.ToString("HH:mm:ss")))
                .ForMember(dt => dt.DestiArrivalTime, opt => opt.MapFrom(t => t.DestiArrivalTime.ToString("HH:mm:ss")))
                .ReverseMap();

            CreateMap<AddTrainDTO, Train>()
                .ForMember(dest => dest.SourceDepartureTime, opt => opt.MapFrom(src => DateTime.Parse(src.SourceDepartureTime)))
                .ForMember(dest => dest.DestiArrivalTime, opt => opt.MapFrom(src => DateTime.Parse(src.DestiArrivalTime)))
                .ReverseMap();

            CreateMap<Train, GetTrainDTO>()
                .ForMember(dt => dt.SourceStation, opt => opt.MapFrom(t => t.SourceStation.StationName))
                .ForMember(dt => dt.DestinationStation, opt => opt.MapFrom(t => t.DestinationStation.StationName))
                .ForMember(dt => dt.SourceDepartureTime, opt => opt.MapFrom(t => t.SourceDepartureTime.ToString("HH:mm:ss")))
                .ForMember(dt => dt.DestiArrivalTime, opt => opt.MapFrom(t => t.DestiArrivalTime.ToString("HH:mm:ss")))
                .ReverseMap();

            CreateMap<ReservationHeader, ReservationHeaderDTO>()
                .ForMember(dest => dest.TrainName, opt => opt.MapFrom(src => src.Train.TrainName))
                .ForMember(dest => dest.JourneyDate, opt => opt.MapFrom(src => src.Schedule.JourneyDate))
                .ReverseMap();

            CreateMap<ReservationDetails, ReservationDetailsDTO>()
                .ForMember(dest => dest.Passenger, opt => opt.MapFrom(src => src.Passenger))
                .ReverseMap();

            CreateMap<Passenger, PassengerDTO>()
                .ReverseMap();

            CreateMap<Payment, PaymentDTO>()
                .ReverseMap();

            CreateMap<Payment, GetPaymentDTO>()
                .ReverseMap();

            CreateMap<ReservationHeader, TicketDTO>()
                .ForMember(dest => dest.PNR, opt => opt.MapFrom(src => src.ReservationID))
                .ForMember(dest => dest.TrainName, opt => opt.MapFrom(src => src.Train.TrainName))
                .ForMember(dest => dest.JourneyDate, opt => opt.MapFrom(src => src.Schedule.JourneyDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.SourceStation, opt => opt.MapFrom(src => src.Train.SourceStation.StationName))
                .ForMember(dest => dest.SourceDepartureTime, opt => opt.MapFrom(src => src.Train.SourceDepartureTime.ToString("HH:mm:ss")))
                .ForMember(dest => dest.DestinationStation, opt => opt.MapFrom(src => src.Train.DestinationStation.StationName))
                .ForMember(dest => dest.DestiArrivalTime, opt => opt.MapFrom(src => src.Train.DestiArrivalTime.ToString("HH:mm:ss")))
                .ForMember(dest => dest.Passengers, opt => opt.MapFrom(src => src.ReservationDetails))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PaymentID, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().PaymentID))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().PaymentMethod))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().PaymentDate.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().Amount))
                .ForMember(dest => dest.AmtStatus, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().AmtStatus))
                .ForMember(dest => dest.TotalRefund, opt => opt.MapFrom(src => src.TotalRefund));


            CreateMap<ReservationDetails, TicketPassengerDTO>()
                .ForMember(dest => dest.PassengerID, opt => opt.MapFrom(src => src.Passenger.PassengerID))
                .ForMember(dest => dest.PassengerName, opt => opt.MapFrom(src => src.Passenger.Name))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Passenger.Age))
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Passenger.Sex))
                .ForMember(dest => dest.SeatNo, opt => opt.MapFrom(src => src.Seat.SeatNo))
                .ForMember(dest => dest.CoachNo, opt => opt.MapFrom(src => src.Seat.Coach.CoachNo));

            CreateMap<Schedule, ScheduleDTO>()
                .ReverseMap();

            CreateMap<Schedule, AddScheduleDTO>()
                .ReverseMap();

        }
    }
}