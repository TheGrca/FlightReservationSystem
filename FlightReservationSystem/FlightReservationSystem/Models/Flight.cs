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
        public int Id { get; set; }
        public Airline Airline { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DateOfDeparture { get; set; }
        public DateTime DateOfDestination { get; set; }
        public int AvailableSeats { get; set; }
        public int OccupiedSeats { get; set; }
        public double Price { get; set; }
        public FlightStatus FlightStatus { get; set; }

        public Flight() { }
    }
}