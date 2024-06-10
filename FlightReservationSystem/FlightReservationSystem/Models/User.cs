using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightReservationSystem.Models
{

    public enum Gender
    {
        Male,
        Female
    }
    public enum TypeOfUser 
    {
        Traveler,
        Administrator
    }
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public TypeOfUser TypeOfUser { get; set; }
        public List<Reservation> Reservations {  get; set; }

        public User() { }
    }
}