using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightReservationSystem.Models;

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
        [Route("api/reservations/user/{username}")]
        public IHttpActionResult GetUserReservations(string username)
        {
            try
            {
                var userReservations = reservations.Where(r => r.User == username).ToList();
                return Ok(userReservations);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
