using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Railway_Reservation_Sys.Interfaces;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailService(IConfiguration config)
        {
            _smtpServer = config["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(config["EmailSettings:SmtpPort"]);
            _senderEmail = config["EmailSettings:SenderEmail"];
            _senderPassword = config["EmailSettings:SenderPassword"];
        }

        public async Task SendETicketAsync(User user, ReservationHeader reservation)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = $"Ticket Booking Confirmation - PNR {reservation.ReservationID}",
                    Body = GenerateETicket(reservation),
                    IsBodyHtml = true
                };
                mailMessage.To.Add(user.Email);

                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                }

                Console.WriteLine($"E-Ticket sent successfully to {user.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send e-ticket: {ex.Message}");
            }
        }

        private string GenerateETicket(ReservationHeader reservation)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h2>Railway Booking Confirmation</h2>");
            sb.AppendLine($"<p><strong>PNR:</strong> {reservation.ReservationID}</p>");
            sb.AppendLine($"<p><strong>Train:</strong> {reservation.Train.TrainName} ({reservation.Train.SourceStation.StationName} ➝ {reservation.Train.DestinationStation.StationName})</p>");
            sb.AppendLine($"<p><strong>Journey Date:</strong> {reservation.Schedule.JourneyDate:dd-MMM-yyyy}</p>");
            sb.AppendLine($"<p><strong>Source Station Departure Time:</strong> {reservation.Train.SourceDepartureTime:HH:mm:ss}</p>");
            sb.AppendLine($"<p><strong>Destination Station Arrival Time:</strong> {reservation.Train.DestiArrivalTime:HH:mm:ss}</p>");
            sb.AppendLine($"<p><strong>Total Fare:</strong> ₹{reservation.TotalFare}</p>");
            sb.AppendLine("<h3>Passenger Details:</h3>");
            sb.AppendLine("<table border='1'><tr><th>Name</th><th>Age</th><th>Sex</th><th>Coach</th><th>Seat</th></tr>");
            foreach (var detail in reservation.ReservationDetails)
            {
                string coachNumber = detail.Seat?.Coach?.CoachNo ?? "Not Assigned";  
                string seatNumber = detail.Seat?.SeatNo.ToString() ?? "Not Assigned";
                sb.AppendLine($"<tr><td>{detail.Passenger.Name}</td><td>{detail.Passenger.Age}</td><td>{detail.Passenger.Sex}</td><td>{coachNumber}</td><td>{seatNumber}</td></tr>");
            }
            sb.AppendLine("</table>");

            var payment = reservation.Payments.FirstOrDefault(p => p.AmtStatus == "Paid" && p.ReservationID == reservation.ReservationID);
            if (payment != null)
            {
                sb.AppendLine("<h3>Payment Details:</h3>");
                sb.AppendLine($"<p><strong>Payment ID:</strong> {payment.PaymentID}</p>");
                sb.AppendLine($"<p><strong>Payment Method:</strong> {payment.PaymentMethod}</p>");
                sb.AppendLine($"<p><strong>Payment Date:</strong> {payment.PaymentDate:dd-MMM-yyyy HH:mm:ss}</p>");
                sb.AppendLine($"<p><strong>Amount Paid:</strong> ₹{payment.Amount}</p>");
                sb.AppendLine($"<p><strong>Payment Status:</strong> {payment.AmtStatus}</p>");
            }

            sb.AppendLine("<p><em>* Please carry a valid ID proof along with this e-ticket.</em></p>");
            sb.AppendLine("<p>Thank you for booking with us! Have a safe journey! </p>");

            return sb.ToString();
        }

        public async Task SendCancellationEmailAsync(User user, ReservationHeader reservation)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = $"Booking Cancellation - PNR {reservation.ReservationID}",
                    Body = GenerateCancellationEmail(reservation),
                    IsBodyHtml = true
                };
                mailMessage.To.Add(user.Email);

                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                }

                Console.WriteLine($"Cancellation email sent to {user.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send cancellation email: {ex.Message}");
            }
        }
        private string GenerateCancellationEmail(ReservationHeader reservation)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h2>Railway Booking Cancellation</h2>");
            sb.AppendLine($"<p><strong>PNR:</strong> {reservation.ReservationID}</p>");
            sb.AppendLine($"<p><strong>Train:</strong> {reservation.Train.TrainName} ({reservation.Train.SourceStation.StationName} ➝ {reservation.Train.DestinationStation.StationName})</p>");
            sb.AppendLine($"<p><strong>Journey Date:</strong> {(reservation.Schedule != null ? reservation.Schedule.JourneyDate.ToString("dd-MMM-yyyy") : "Train Cancelled on Date.")}</p>");
            sb.AppendLine($"<p><strong>Source Station Departure Time:</strong> {reservation.Train.SourceDepartureTime:HH:mm:ss}</p>");
            sb.AppendLine($"<p><strong>Destination Station Arrival Time:</strong> {reservation.Train.DestiArrivalTime:HH:mm:ss}</p>");
            
            sb.AppendLine($"<p><strong>Total Refund:</strong> ₹{reservation.TotalRefund}</p>");

            sb.AppendLine("<h3>Cancelled Passengers:</h3>");
            sb.AppendLine("<table border='1'><tr><th>Name</th><th>Age</th><th>Sex</th></tr>");
            foreach (var detail in reservation.ReservationDetails.Where(d => d.Status == "Cancelled"))
            {
                sb.AppendLine($"<tr><td>{detail.Passenger.Name}</td><td>{detail.Passenger.Age}</td><td>{detail.Passenger.Sex}</td></tr>");
            }
            sb.AppendLine("</table>");

            var refund = reservation.Payments.FirstOrDefault(p => p.AmtStatus == "Refunded" && p.ReservationID == reservation.ReservationID);
            if (refund != null)
            {
                sb.AppendLine("<h3>Refund Details:</h3>");
                sb.AppendLine($"<p><strong>Payment ID:</strong> {refund.PaymentID}</p>");
                sb.AppendLine($"<p><strong>Refund Amount:</strong> ₹{refund.Amount}</p>");
                sb.AppendLine($"<p><strong>Refund Date:</strong> {refund.PaymentDate:dd-MMM-yyyy HH:mm:ss}</p>");
                sb.AppendLine($"<p><strong>Refund Method:</strong> {refund.PaymentMethod}</p>");
                sb.AppendLine($"<p><strong>Refund Status:</strong> {refund.AmtStatus}</p>");
            }

            sb.AppendLine("<p>We regret the inconvenience. You will receive your refund shortly.</p>");

            return sb.ToString();
        }

    }
}

