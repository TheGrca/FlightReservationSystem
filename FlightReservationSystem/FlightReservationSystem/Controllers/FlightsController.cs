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
        private static List<Flight> flights = new List<Flight>();


        // GET: api/Flights
        [HttpGet]
        public IEnumerable<Flight> GetFlights()
        {
            return flights;
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
    }
}
