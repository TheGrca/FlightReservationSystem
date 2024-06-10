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
            // Add initial data to the lists for testing
            var airline1 = new Airline { Id = 1, Name = "Airline 1", Address = "123 Street", Contact = "123-456-7890" };
            var flight1 = new Flight
            {
                Id = 1,
                Airline = airline1,
                Departure = "City A",
                Destination = "City B",
                DateOfDeparture = DateTime.Now.AddHours(2),
                DateOfDestination = DateTime.Now.AddHours(5),
                AvailableSeats = 100,
                OccupiedSeats = 0,
                Price = 150,
                FlightStatus = FlightStatus.Active
            };

            airline1.Flights.Add(flight1);

            _airlines.Add(airline1);
            _flights.Add(flight1);

            var user1 = new User
            {
                Username = "user1",
                Password = "password",
                Name = "First",
                Lastname = "User",
                Email = "user1@example.com",
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = Gender.Male,
                TypeOfUser = TypeOfUser.Traveler
            };

            _users.Add(user1);
        }

        public Database() { }
    }
}