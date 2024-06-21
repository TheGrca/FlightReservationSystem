using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightReservationSystem.Models;

namespace FlightReservationSystem.Controllers
{
    public class FlightsController : ApiController
    {
        private static List<Flight> flights;


        public FlightsController()
        {
            flights = Models.Flight.LoadFlights();
        }

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

        // GET: api/Flights/{id}
        [HttpGet]
        public IHttpActionResult GetFlight(int id)
        {
            var flight = flights.SingleOrDefault(f => f.Id == id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }

        // POST: api/Flights
        [HttpPost]
        public IHttpActionResult PostFlight(Flight flight)
        {
            flight.Id = flights.Count > 0 ? flights.Max(f => f.Id) + 1 : 1;
            flight.FlightStatus = FlightStatus.Active;
            flights.Add(flight);
            return CreatedAtRoute("DefaultApi", new { id = flight.Id }, flight);
        }

        // PUT: api/Flights/{id}
        [HttpPut]
        public IHttpActionResult PutFlight(int id, Flight flight)
        {
            var existingFlight = flights.SingleOrDefault(f => f.Id == id);
            if (existingFlight == null)
            {
                return NotFound();
            }
            if (existingFlight.FlightStatus == FlightStatus.Active || existingFlight.FlightStatus == FlightStatus.Active)
            {
                return BadRequest("Cannot change details of a flight with reservations.");
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Flights/{id}
        [HttpDelete]
        public IHttpActionResult DeleteFlight(int id)
        {
            var flight = flights.SingleOrDefault(f => f.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            flights.Remove(flight);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("api/flights/active")]
        public IHttpActionResult GetActiveFlights()
        {
            var activeFlights = flights.Where(f => f.FlightStatus == FlightStatus.Active).ToList();
            return Ok(activeFlights);
        }

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
                filteredFlights = filteredFlights.Where(f => f.DateOfDeparture.Date == searchModel.DateOfDeparture.Value.Date);
            }
            if (searchModel.DateOfDestination.HasValue)
            {
                filteredFlights = filteredFlights.Where(f => f.DateOfDestination.Date == searchModel.DateOfDestination.Value.Date);
            }

            return Ok(filteredFlights);
        }

        [HttpGet]
        [Route("api/airlines/{id}/flights")]
        public IHttpActionResult GetFlightsByAirline(int id)
        {
            var airlineFlights = flights.Where(f => f.Id == id).ToList();
            return Ok(airlineFlights);
        }
    }
    public class FlightSearchModel
    {
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime? DateOfDeparture { get; set; }
        public DateTime? DateOfDestination { get; set; }
    }
}
