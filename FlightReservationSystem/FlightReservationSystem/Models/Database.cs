using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightReservationSystem.Models
{
    public class Database
    {
        private readonly List<User> _users = new List<User>();
        private readonly List<Airline> _airlines = new List<Airline>();
        private readonly List<Flight> _flights = new List<Flight>();
        private readonly List<Reservation> _reservations = new List<Reservation>();
        private readonly List<Review> _reviews = new List<Review>();

        public List<User> Users => _users;
        public List<Airline> Airlines => _airlines;
        public List<Flight> Flights => _flights;
        public List<Reservation> Reservations => _reservations;
        public List<Review> Reviews => _reviews;

        public void SeedData()
        {

        }

        public Database() { }
    }
}