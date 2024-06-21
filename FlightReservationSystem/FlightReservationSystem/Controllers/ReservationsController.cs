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
        private static List<Reservation> reservations;
        public ReservationsController()
        {
            reservations = Reservation.LoadReservations();
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
