using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightReservationSystem.Models;
using Newtonsoft.Json.Converters;

namespace FlightReservationSystem.Controllers
{
    public class ReservationsController : ApiController
    {
        private List<Reservation> reservations
        {
            get
            {
                return System.Web.HttpContext.Current.Application["Reservations"] as List<Reservation>;
            }
            set
            {
                System.Web.HttpContext.Current.Application["Reservations"] = value;
            }
        }


        [HttpGet]
        [Route("api/reservations")]
        public IHttpActionResult GetReservations()
        {
            if (reservations == null || !reservations.Any())
            {
                return NotFound();
            }

            return Ok(reservations);
        }

        [HttpGet]
        [Route("api/reservations/{id}")]
        public IHttpActionResult GetReservation(int id)
        {
            var reservation = reservations.Where(r => r.Id == id).FirstOrDefault();
            if(reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }



        [HttpGet]
        [Route("api/reservations/user/{username}")]
        public IHttpActionResult GetUserReservations(string username)
        {
            try
            {
                var userReservations = reservations.Where(r => r.User.Username == username).ToList();
                return Ok(userReservations);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("api/reservations/completed")]
        public IHttpActionResult GetCompletedFlightReservations([FromBody] List<Reservation> userReservations)
        {
            try
            {
                var completedReservations = userReservations.Where(r => r.Flight.FlightStatus == FlightStatus.Completed).ToList();
                return Ok(completedReservations);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("api/reservations")]
        public IHttpActionResult CreateReservation([FromBody] CreateReservationRequest request)
        {
            var users = System.Web.HttpContext.Current.Application["Users"] as List<User>;
            var flights = System.Web.HttpContext.Current.Application["Flights"] as List<Flight>;
            Debug.Write(request.Username + "  " + request.FlightId);

            var user = users?.SingleOrDefault(u => u.Username == request.Username);
            var flight = flights?.SingleOrDefault(f => f.Id == request.FlightId);

            if (user == null || flight == null || request.NumberOfPassengers > flight.AvailableSeats)
            {
                return BadRequest("Invalid request data.");
            }

            int maxId;
            if (reservations.Count == 0)
            {
                maxId = -1;
            }
            else
            {
                maxId = reservations.Max(r => r.Id);
            }
            
            int newId = maxId + 1;

            var reservation = new Reservation
            {
                Id = newId,
                User = user,
                Flight = flight,
                NumberOfPassengers = request.NumberOfPassengers,
                TotalPrice = flight.Price * request.NumberOfPassengers,
                ReservationStatus = ReservationStatus.Created
            };

           reservations.Add(reservation);

            foreach(User u in users)
            {
                if(u.Username == user.Username)
                {
                    u.Reservations.Add(reservation.Id);
                }
            }
           
           System.Web.HttpContext.Current.Application["Reservations"] = reservations;
           System.Web.HttpContext.Current.Application["Users"] = users;
           System.Web.HttpContext.Current.Session["User"] = user;

            return Ok(reservation);
        }


        [HttpPut]
        [Route("api/reservations/{id}")]
        public IHttpActionResult UpdateReservationStatus(int id, [FromBody] UpdateReservationStatusRequest request)
        {
            var flights = System.Web.HttpContext.Current.Application["Flights"] as List<Flight>;
            var reservation = reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.ReservationStatus = request.ReservationStatus;

            if (request.ReservationStatus == ReservationStatus.Canceled)
            {
                var updateSeatsRequest = new UpdateSeatsRequest
                {
                    FlightId = reservation.Flight.Id,
                    Tickets = reservation.NumberOfPassengers
                };

                var flight = flights.SingleOrDefault(f => f.Id == updateSeatsRequest.FlightId);
                if (flight != null)
                {
                    flight.AvailableSeats += updateSeatsRequest.Tickets;
                    flight.OccupiedSeats -= updateSeatsRequest.Tickets;
                }
            }
            System.Web.HttpContext.Current.Application["Flights"] = flights;
            return Ok();
        }

        [HttpPost]
        [Route("api/reservations/userFlights")]
        public IHttpActionResult GetUserFlights([FromBody] List<int> reservationIds)
        {
            if (reservationIds == null || !reservationIds.Any())
            {
                return BadRequest("Invalid reservation IDs.");
            }

            var userReservations = reservations.Where(r => reservationIds.Contains(r.Id)).ToList();
            if (userReservations == null || !userReservations.Any())
            {
                return NotFound();
            }

            return Ok(userReservations);
        }

        [HttpDelete]
        [Route("api/reservations/{id}")]
        public IHttpActionResult CancelReservation(int id)
        {
            var reservation = reservations.SingleOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.ReservationStatus == ReservationStatus.Created || reservation.ReservationStatus == ReservationStatus.Approved)
            {
                var departureTime = reservation.Flight.DateOfDeparture;
                var now = DateTime.Now;
                if ((departureTime - now).TotalHours > 24)
                {
                    reservations.Remove(reservation);
                    // Save changes to reservations
                    return Ok();
                }
            }
            return BadRequest("Reservation cannot be canceled.");
        }

        [HttpPost]
        [Route("api/users/removeReservation")]
        public IHttpActionResult RemoveReservation(RemoveReservationRequest request)
        {
            var users = System.Web.HttpContext.Current.Application["Users"] as List<User>;
            var user = users.SingleOrDefault(u => u.Username == request.Username);
            if (user == null)
            {
                return NotFound();
            }

            user.Reservations.Remove(request.ReservationId);
            System.Web.HttpContext.Current.Application["Users"] = users;
            return Ok();
        }

        public class RemoveReservationRequest
        {
            public string Username { get; set; }
            public int ReservationId { get; set; }
        }
        public class UpdateReservationStatusRequest
        {
            public ReservationStatus ReservationStatus { get; set; }
        }
        public class CreateReservationRequest
        {
            public string Username { get; set; }
            public int FlightId { get; set; }
            public int NumberOfPassengers { get; set; }
        }


    }
}
