using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public Airline Airline { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DateOfDeparture { get; set; }
        public DateTime DateOfDestination { get; set; }
        public Tuple<int,int> NumberOfAvailableAndUnavailableSeats { get; set; }
        public double Price { get; set; }
        public FlightStatus FlightStatus { get; set; }
    }
}