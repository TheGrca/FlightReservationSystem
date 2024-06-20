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

    
    }
}
