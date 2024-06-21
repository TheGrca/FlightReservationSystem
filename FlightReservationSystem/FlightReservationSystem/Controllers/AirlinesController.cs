using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightReservationSystem.Models;

namespace FlightReservationSystem.Controllers
{
    public class AirlinesController : ApiController
    {
        private static List<Airline> airlines;
        public AirlinesController()
        {
            airlines = Airline.LoadAirlines();
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
    }
}
