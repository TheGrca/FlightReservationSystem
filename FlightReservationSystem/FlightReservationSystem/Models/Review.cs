using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public ReviewStatus ReviewStatus { get; set; }
        public Review() { }
    }
}