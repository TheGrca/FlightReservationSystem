using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FlightReservationSystem.Models
{
    public class Airline
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public List<int> Flights { get; set; }
        public List<Review> Reviews { get; set; }
        private static readonly string _airlinesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "airlines.json");
        public Airline() { }

        public static List<Airline> LoadAirlines()
        {
            var jsonData = File.ReadAllText(_airlinesFilePath);
            var airlines = JsonConvert.DeserializeObject<List<Airline>>(jsonData);
            return airlines;
        }
    }
}