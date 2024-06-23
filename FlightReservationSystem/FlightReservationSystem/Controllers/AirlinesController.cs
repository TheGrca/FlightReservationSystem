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


        [HttpGet]
        [Route("api/airlines")]
        public IHttpActionResult GetAirlines()
        {
            return Ok(airlines);
        }

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

        [HttpGet]
        [Route("api/airlines/idByName/{name}")]
        public IHttpActionResult GetAirlineIdByName(string name)
        {
            try
            {
                var airline = airlines.SingleOrDefault(a => a.Name == name);
                if (airline == null)
                {
                    return NotFound();
                }
                return Ok(airline.Id);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/airlines")]
        public IHttpActionResult AddAirline([FromBody] Airline airline)
        {
            if (airline == null)
            {
                return BadRequest("Airline data is null.");
            }

            // Find the maximum Id in the existing airlines
            int maxId = airlines.Max(a => a.Id);

            // Assign the new airline's Id as maxId + 1
            airline.Id = maxId + 1;

            airlines.Add(airline);

            foreach(Airline e in airlines)
            {
                Debug.Write(e.Name);
            }

            return Created($"api/airlines/{airline.Id}", airline);
        }

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

            // Update existing airline details
            existingAirline.Name = updatedAirline.Name;
            existingAirline.Address = updatedAirline.Address;
            existingAirline.Contact = updatedAirline.Contact;
            // Update other fields as needed

            return Ok(existingAirline);
        }

        [HttpDelete]
        [Route("api/airlines/{id}")]
        public IHttpActionResult DeleteAirline(int id)
        {
            var airline = airlines.FirstOrDefault(a => a.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            airlines.Remove(airline);
            return Ok();
        }

    }
}
    

