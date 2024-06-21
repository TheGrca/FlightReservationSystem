using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FlightReservationSystem.Models
{
    public enum ReservationStatus
    {
        Created,
        Approved,
        Canceled,
        Completed
    }
    public class Reservation
    {
        public string User { get; set; }
        public Flight Flight { get; set; }
        public int NumberOfPassengers { get; set; }
        public double TotalPrice { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
        private static readonly string _reservationsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "reservations.json");
        public Reservation() { }
        public static List<Reservation> LoadReservations()
        {
            var jsonData = File.ReadAllText(_reservationsFilePath);
            var reservations = JsonConvert.DeserializeObject<List<Reservation>>(jsonData);
            return reservations;
        }
    }
}