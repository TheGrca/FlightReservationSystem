using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlightReservationSystem.Models;

namespace FlightReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private  Database database;

        public HomeController()
        {
            database = (Database)System.Web.HttpContext.Current.Application["Database"];
        }

        public ActionResult Index()
        {
            var flights = database.Flights.Where(f => f.FlightStatus == FlightStatus.Active).ToList();
            return View(flights);
        }

        public ActionResult Search(string departure, string destination, DateTime? departureDate, DateTime? returnDate, string airline)
        {
            var query = database.Flights.Where(f => f.FlightStatus == FlightStatus.Active).AsQueryable();

            if (!string.IsNullOrEmpty(departure))
                query = query.Where(f => f.Departure == departure);

            if (!string.IsNullOrEmpty(destination))
                query = query.Where(f => f.Destination == destination);

            if (departureDate.HasValue)
                query = query.Where(f => f.DateOfDeparture.Date == departureDate.Value.Date);

            if (returnDate.HasValue)
                query = query.Where(f => f.DateOfDestination.Date == returnDate.Value.Date);

            if (!string.IsNullOrEmpty(airline))
                query = query.Where(f => f.Airline.Name == airline);

            var flights = query.ToList();
            return View("Index", flights);
        }
    }
}
