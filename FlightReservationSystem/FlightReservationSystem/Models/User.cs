using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

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

        private static readonly string _usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "users.json");
        public User() { }

        public static List<User> LoadUsers()
        {
            var jsonData = File.ReadAllText(_usersFilePath);
            var users = JsonConvert.DeserializeObject<List<User>>(jsonData);
            return users;
        }

        public static void SaveUsersToJson(List<User> users)
        {
            var jsonData = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_usersFilePath, jsonData);
        }
    }
}