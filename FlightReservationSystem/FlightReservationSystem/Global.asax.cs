using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FlightReservationSystem.Models;

namespace FlightReservationSystem
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database database = new Database();
            Application["Database"] = database;
            FillDatabaseWithSampleFlight(database);
        }

        private void FillDatabaseWithSampleFlight(Database database)
        {
            // Create a new Flight instance
            var flight = new Flight
            {
                Airline = new Airline { Name = "Sample Airline" }, // Replace with actual airline data
                Departure = "New York", // Replace with actual departure city
                Destination = "Los Angeles", // Replace with actual destination city
                DateOfDeparture = DateTime.UtcNow.AddDays(7), // Replace with actual departure date/time
                DateOfDestination = DateTime.UtcNow.AddDays(7).AddHours(6), // Replace with actual arrival date/time
                AvailableSeats = 100, // Replace with actual available seats count
                Price = 250, // Replace with actual price
                FlightStatus = FlightStatus.Active // Replace with actual flight status
            };

            // Add the flight to the database
            database.Flights.Add(flight);

        }
    }
}
