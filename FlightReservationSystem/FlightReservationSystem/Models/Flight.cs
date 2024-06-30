using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FlightReservationSystem.Models
{
    public enum FlightStatus
    {
        Active,
        Cancelled,
        Completed
    }
    public class Flight
    {
        public int Id { get; set; }
        public string Airline { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DateOfDeparture { get; set; }
        public DateTime DateOfDestination { get; set; }
        public int AvailableSeats { get; set; }
        public int OccupiedSeats { get; set; }
        public double Price { get; set; }
        [JsonConverter(typeof(EnumConverter))]
        public FlightStatus FlightStatus { get; set; }

        private static readonly string _flightsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "flights.json");
        public Flight() { }

        public static List<Flight> LoadFlights()
        {
            var jsonData = File.ReadAllText(_flightsFilePath);
            var flights = JsonConvert.DeserializeObject<List<Flight>>(jsonData);
            return flights;
        }
    }
}