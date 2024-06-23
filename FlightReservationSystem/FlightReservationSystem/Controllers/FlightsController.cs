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

        [HttpPost]
        [Route("api/flights")]
        public IHttpActionResult AddFlight([FromBody] Flight newFlight)
        {
            if(newFlight == null)
            {
                return BadRequest();
            }
            // Find the highest current ID in flights list
            int maxId = flights.Max(f => f.Id);

            // Increment the ID for the new flight
            newFlight.Id = maxId + 1;

            // Set other properties for the new flight
            newFlight.FlightStatus = FlightStatus.Active;
            newFlight.OccupiedSeats = 0;

            // Add the new flight to the flights list
            flights.Add(newFlight);

            // Return HTTP 201 Created status with the newly added flight
            return Created($"api/flights/{newFlight.Id}", newFlight);
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

        [HttpDelete]
        [Route("api/flights/{id}")]
        public IHttpActionResult DeleteFlight(int id, [FromBody] List<Reservation> reservations)
        {
            var flightToDelete = flights.FirstOrDefault(f => f.Id == id);
            if (flightToDelete == null)
            {
                return NotFound();
            }

            // Check if there are reservations with status 'Created' or 'Approved' for this flight
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
            return Ok();
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

        [HttpPost]
        [Route("api/flights/updateSeats")]
        public IHttpActionResult UpdateSeats(UpdateSeatsRequest request)
        {
            // Assuming 'flights' is your data context or repository
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

            // Save the updated flight details
            //SaveFlight(flight); // Implement SaveFlight to persist changes

            return Ok(new { message = "Seats updated successfully" });
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
