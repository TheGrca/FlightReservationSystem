using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public User User { get; set; }
        public Flight Flight { get; set; }
        public int NumberOfPassengers { get; set; }
        public double TotalPrice { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
    }
}