using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightReservationSystem.Models
{
    public class Airline
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Contact { get; set; }
        public List<Flight> Flights { get; set; }
        public List<Review> Reviews { get; set; }
    }
}