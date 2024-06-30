using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FlightReservationSystem.Models
{
    public enum ReviewStatus
    {
        Created,
        Approved,
        Rejected
    }
    public class Review
    {
        public int Id { get; set; }
        public User Reviewer { get; set; }
        public Airline Airline { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        [JsonConverter(typeof(EnumConverter))]
        public ReviewStatus ReviewStatus { get; set; }

        private static readonly string _reviewsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "reviews.json");
        public Review() { }

        public static List<Review> LoadReviews()
        {
            var jsonData = File.ReadAllText(_reviewsFilePath);
            var reviews = JsonConvert.DeserializeObject<List<Review>>(jsonData);
            return reviews;
        }
    }
}