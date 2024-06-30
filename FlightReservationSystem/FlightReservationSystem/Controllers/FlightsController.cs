using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using FlightReservationSystem.Models;

namespace FlightReservationSystem.Controllers
{
    public class FlightsController : ApiController
    {
        private List<Flight> flights
        {
            get
            {
                return System.Web.HttpContext.Current.Application["Flights"] as List<Flight>;
            }
            set
            {
                System.Web.HttpContext.Current.Application["Flights"] = value;
            }
        }

        //Api for getting all the flights
        // GET: api/Flights
        [HttpGet]
        [Route("api/flights")]
        public IHttpActionResult GetFlights()
        {
            if (flights == null || !flights.Any())
            {
                return NotFound();
            }

            return Ok(flights);
        }

        //Api for getting one flight by its id
        // GET: api/Flights/{id}
        [HttpGet]
        [Route("api/flights/{id}")]
        public IHttpActionResult GetFlight(int id)
        {
            var flight = flights.SingleOrDefault(f => f.Id == id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }

        //Api for adding a flight
        [HttpPost]
        [Route("api/flights")]
        public IHttpActionResult AddFlight([FromBody] Flight newFlight)
        {
            var airlines = System.Web.HttpContext.Current.Application["Airlines"] as List<Airline>;
            if (newFlight == null || airlines == null)
            {
                return BadRequest();
            }

            int maxId = flights.Max(f => f.Id);

            newFlight.Id = maxId + 1;
            newFlight.FlightStatus = FlightStatus.Active;
            newFlight.OccupiedSeats = 0;
            flights.Add(newFlight);

            var airline = airlines.FirstOrDefault(a => a.Name == newFlight.Airline);
            if (airline != null)
            {
                if (airline.Flights == null)
                {
                    airline.Flights = new List<int>();
                }
                airline.Flights.Add(newFlight.Id);
            }
            return Created($"api/flights/{newFlight.Id}", newFlight);
        }

        //Api for updating a flight
        // PUT: api/flights/{id}
        [HttpPut]
        [Route("api/flights/{id}")]
        public IHttpActionResult PutFlight(int id, Flight updatedFlight)
        {
            var existingFlight = flights.SingleOrDefault(f => f.Id == id);
            if (existingFlight == null)
            {
                return NotFound();
            }

            existingFlight.Airline = updatedFlight.Airline;
            existingFlight.Departure = updatedFlight.Departure;
            existingFlight.Destination = updatedFlight.Destination;
            existingFlight.DateOfDeparture = updatedFlight.DateOfDeparture;
            existingFlight.DateOfDestination = updatedFlight.DateOfDestination;
            existingFlight.Price = updatedFlight.Price;
            existingFlight.FlightStatus = updatedFlight.FlightStatus;

            return StatusCode(HttpStatusCode.NoContent);
        }


        //Api for fetching the airlines from a flights
        [HttpPost]
        [Route("api/flights/airlines")]
        public IHttpActionResult GetAirlinesFromFlights([FromBody] List<Flight> flights)
        {
            try
            {
                var airlineNames = flights.Select(f => f.Airline).Distinct().ToList();
                return Ok(airlineNames);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        //Api for deleting a flight

        [HttpDelete]
        [Route("api/flights/{id}")]
        public IHttpActionResult DeleteFlight(int id)
        {
            var flightToDelete = flights.FirstOrDefault(f => f.Id == id);
            if (flightToDelete == null)
            {
                return NotFound();
            }

            var reservations = System.Web.HttpContext.Current.Application["Reservations"] as List<Reservation>;
            bool canDelete = true;
            foreach (var reservation in reservations)
            {
                if (reservation.Flight.Id == id &&
                    (reservation.ReservationStatus == ReservationStatus.Created ||
                     reservation.ReservationStatus == ReservationStatus.Approved))
                {
                    canDelete = false;
                    break;
                }
            }

            if (!canDelete)
            {
                return BadRequest("Cannot delete flight with existing 'Created' or 'Approved' reservations.");
            }

            flights.Remove(flightToDelete);
            System.Web.HttpContext.Current.Application["Reservations"] = reservations;
            return Ok();
        }

        //Api for returning active flights

        [HttpGet]
        [Route("api/flights/active")]
        public IHttpActionResult GetActiveFlights()
        {
            var activeFlights = flights.Where(f => f.FlightStatus == FlightStatus.Active).ToList();
            return Ok(activeFlights);
        }

        //Api for filtering flights

        [HttpPost]
        [Route("api/flights/search")]
        public IHttpActionResult SearchFlights([FromBody] FlightSearchModel searchModel)
        {
            var filteredFlights = flights.Where(f => f.FlightStatus == FlightStatus.Active);

            if (!string.IsNullOrEmpty(searchModel.Departure))
            {
                filteredFlights = filteredFlights.Where(f => f.Departure.ToLower().Contains(searchModel.Departure.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchModel.Destination))
            {
                filteredFlights = filteredFlights.Where(f => f.Destination.ToLower().Contains(searchModel.Destination.ToLower()));
            }
            if (searchModel.DateOfDeparture.HasValue)
            {
                filteredFlights = filteredFlights.Where(f => f.DateOfDeparture.Date >= searchModel.DateOfDeparture.Value.Date);
            }
            if (searchModel.DateOfDestination.HasValue)
            {
                filteredFlights = filteredFlights.Where(f => f.DateOfDestination.Date <= searchModel.DateOfDestination.Value.Date);
            }

            return Ok(filteredFlights);
        }

        //Api for fetching flight by an airline

        [HttpGet]
        [Route("api/airlines/{id}/flights")]
        public IHttpActionResult GetFlightsByAirline(int id)
        {
            var airlineFlights = flights.Where(f => f.Id == id).ToList();
            return Ok(airlineFlights);
        }

        //Api for updating seats when a ticket is bought

        [HttpPost]
        [Route("api/flights/updateSeats")]
        public IHttpActionResult UpdateSeats(UpdateSeatsRequest request)
        {
            var flight = flights.SingleOrDefault(f => f.Id == request.FlightId);
            if (flight == null)
            {
                return NotFound();
            }

            if (flight.AvailableSeats < request.Tickets)
            {
                return BadRequest("Not enough available seats.");
            }

            flight.AvailableSeats -= request.Tickets;
            flight.OccupiedSeats += request.Tickets;

            return Ok(new { message = "Seats updated successfully" });
        }


        //Api for updating seats when a reservation gets canceled
        [HttpPost]
        [Route("api/flights/updateCanceledSeats")]
        public IHttpActionResult UpdateCanceledSeats(UpdateSeatsRequest request)
        {
            var flight = flights.SingleOrDefault(f => f.Id == request.FlightId);
            if (flight == null)
            {
                return NotFound();
            }

            flight.AvailableSeats += request.Tickets;
            flight.OccupiedSeats -= request.Tickets;

            return Ok(new { message = "Seats updated successfully" });
        }

        //Api for returning flights that have active reservations

        [HttpGet]
        [Route("api/flights/{id}/activeReservations")]
        public IHttpActionResult HasActiveReservations(int id)
        {
            var reservations = System.Web.HttpContext.Current.Application["Reservations"] as List<Reservation>;
            bool hasActiveReservations;
            if (reservations != null)
            {
                hasActiveReservations = reservations.Any(reservation =>
                    reservation.Flight.Id == id &&
                    (reservation.ReservationStatus == ReservationStatus.Created || reservation.ReservationStatus == ReservationStatus.Approved));
            }
            else
            {
                hasActiveReservations = false;
            }
            System.Web.HttpContext.Current.Application["Reservations"] = reservations;
            return Ok(hasActiveReservations);
        }

        //Api for returning multiple flights by their ID's
        [HttpPost]
        [Route("api/flights/byIds")]
        public IHttpActionResult GetFlightsByIds([FromBody] List<int> flightIds)
        {
            if (flightIds != null && flightIds.Any())
            {
                try
                {
                    var filteredFlights = flights.Where(f => flightIds.Contains(f.Id)).ToList();
                    return Ok(filteredFlights);
                }
                catch (Exception)
                {
                    return InternalServerError(); 
                }
            }
            return NotFound(); 
        }

    }
    public class FlightSearchModel
    {
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime? DateOfDeparture { get; set; }
        public DateTime? DateOfDestination { get; set; }
    }
    public class UpdateSeatsRequest
    {
        public int FlightId { get; set; }
        public int Tickets { get; set; }
    }

}
