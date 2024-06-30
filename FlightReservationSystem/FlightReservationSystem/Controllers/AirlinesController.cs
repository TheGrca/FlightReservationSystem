using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightReservationSystem.Models;

namespace FlightReservationSystem.Controllers
{
    public class AirlinesController : ApiController
    {
        private List<Airline> airlines
        {
            get
            {
                return System.Web.HttpContext.Current.Application["Airlines"] as List<Airline>;
            }
            set
            {
                System.Web.HttpContext.Current.Application["Airlines"] = value;
            }
        }


        //Api for getting all airlines
        [HttpGet]
        [Route("api/airlines")]
        public IHttpActionResult GetAirlines()
        {
            return Ok(airlines);
        }

        //Api for getting one airline by its id
        [HttpGet]
        [Route("api/airlines/{id}")]
        public IHttpActionResult GetAirline(int id)
        {
            var airline = airlines.FirstOrDefault(a => a.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            return Ok(airline);
        }

        //Api for getting airline ID by its name
        [HttpGet]
        [Route("api/airlines/idByName/{name}")]
        public IHttpActionResult GetAirlineIdByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Airline name cannot be null or empty.");
                }

                var airline = airlines.SingleOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (airline == null)
                {
                    return NotFound();
                }
                return Ok(airline.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in GetAirlineIdByName: {ex.Message}");
                return InternalServerError(ex);
            }
        }
        //Api for adding airline

        [HttpPost]
        [Route("api/airlines")]
        public IHttpActionResult AddAirline([FromBody] Airline airline)
        {
            if (airline == null)
            {
                return BadRequest("Airline data is null.");
            }

            int maxId = airlines.Max(a => a.Id);
            airline.Id = maxId + 1;

            airlines.Add(airline);

            return Created($"api/airlines/{airline.Id}", airline);
        }

        //Api for updating an airline

        [HttpPut]
        [Route("api/airlines/{id}")]
        public IHttpActionResult UpdateAirline(int id, [FromBody] Airline updatedAirline)
        {
            if (updatedAirline == null)
            {
                return BadRequest("Airline data is null.");
            }

            var existingAirline = airlines.FirstOrDefault(a => a.Id == id);
            if (existingAirline == null)
            {
                return NotFound();
            }

            existingAirline.Name = updatedAirline.Name;
            existingAirline.Address = updatedAirline.Address;
            existingAirline.Contact = updatedAirline.Contact;

            return Ok(existingAirline);
        }

        //Api for deleting an airline

        [HttpDelete]
        [Route("api/airlines/{id}")]
        public IHttpActionResult DeleteAirline(int id)
        {
            var flights = System.Web.HttpContext.Current.Application["Flights"] as List<Flight>;
            var airline = airlines.FirstOrDefault(a => a.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            var activeFlights = airline.Flights
                .Select(flightId => flights.FirstOrDefault(f => f.Id == flightId))
                .Where(f => f != null && f.FlightStatus == FlightStatus.Active)
                .ToList();

            if (activeFlights.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, "Cannot delete the airline because it has active flights.");
            }

            airlines.Remove(airline);
            return Ok();
        }


        //Api for getting an airline by its name
        [HttpGet]
        [Route("api/airlines/getairlinebyname/{name}")]
        public IHttpActionResult GetAirlineByName(string name)
        {
            var airline = airlines.SingleOrDefault(a => a.Name == name);
            Debug.Write(airline.Name);
            if (airline == null)
            {
                return NotFound();
            }
            return Ok(airline);
        }

    }
}
    

