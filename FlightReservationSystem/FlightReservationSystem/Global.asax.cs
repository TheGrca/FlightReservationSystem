using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using FlightReservationSystem.Controllers;
using FlightReservationSystem.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FlightReservationSystem
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        //Defined timer for saving objects to json file, and for checking if the flight is finished
        private Timer saveTimer;
        private readonly object saveLock = new object();

        private Timer flightStatusCheckTimer;
        private readonly object timerLock = new object();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Loading databases
            Application["Users"] = Models.User.LoadUsers() ?? new List<User>();
            Application["Flights"] = Models.Flight.LoadFlights() ?? new List<Flight>();
            Application["Airlines"] = Models.Airline.LoadAirlines() ?? new List<Airline>();
            Application["Reservations"] = Models.Reservation.LoadReservations() ?? new List<Reservation>();
            Application["Reviews"] = Models.Review.LoadReviews() ?? new List<Review>();
            saveTimer = new Timer(SaveDataPeriodically, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
            flightStatusCheckTimer = new Timer(CheckFlightStatuses, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        protected void Application_End()
        {
        saveTimer.Dispose();
        flightStatusCheckTimer.Dispose();
        }

        private void CheckFlightStatuses(object state)
        {
            lock (timerLock)
            {
                var flights = Application["Flights"] as List<Flight>;
                var reservations = Application["Reservations"] as List<Reservation>;

                if (flights == null)
                    return;

                foreach (var flight in flights)
                {
                    if (flight.DateOfDestination <= DateTime.Now && flight.FlightStatus == FlightStatus.Active)
                    {
                        flight.FlightStatus = FlightStatus.Completed;
                        if (reservations != null)
                        {
                            foreach (var reservation in reservations)
                            {
                                if (reservation.Flight.Id == flight.Id &&
                                    (reservation.ReservationStatus == ReservationStatus.Created ||
                                     reservation.ReservationStatus == ReservationStatus.Approved))
                                {
                                    reservation.ReservationStatus = ReservationStatus.Completed;
                                }
                            }
                        }
                    }
                }
                Application["Reservations"] = reservations;
                SaveListToJson("Flights", flights, "flights.json");
            }
        }


        private void SaveDataPeriodically(object state)
        {
            lock (saveLock)
            {
                try
                {
                    SaveListToJson("Users", Application["Users"] as List<User>, "users.json");
                    SaveListToJson("Flights", Application["Flights"] as List<Flight>, "flights.json");
                    SaveListToJson("Airlines", Application["Airlines"] as List<Airline>, "airlines.json");
                    SaveListToJson("Reservations", Application["Reservations"] as List<Reservation>, "reservations.json");
                    SaveListToJson("Reviews", Application["Reviews"] as List<Review>, "reviews.json");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving data: {ex.Message}");
                }
            }
        }

        private void SaveListToJson<T>(string key, List<T> list, string fileName)
        {
            try
            {
                if (list != null)
                {
                    string json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = new List<JsonConverter> { new StringEnumConverter() }
                    });

                    string filePath = HostingEnvironment.MapPath("~/Database/" + fileName);
                    File.WriteAllText(filePath, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving {key} to JSON file: {ex.Message}");
            }
        }


        //Session for login
        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) =>
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            };
            base.Init();
        }
    }
}
