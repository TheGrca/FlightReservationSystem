using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightReservationSystem.Models;

namespace FlightReservationSystem.Controllers
{


    public class UsersController : ApiController
    {

        private static List<User> users = new List<User>
    {
        new User
        {
            Username = "user",
            Password = "user",
            Name = "User",
            Lastname = "User",
            Email = "user@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            Gender = Gender.Male,
            TypeOfUser = TypeOfUser.Traveler,
            Reservations = new List<Reservation>()
        }
    };

        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return users;
        }

        [HttpGet]
        public IHttpActionResult GetUser(string username)
        {
            var user = users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public IHttpActionResult PostUser(User user)
        {
            if (users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }
            users.Add(user);
            return CreatedAtRoute("DefaultApi", new { id = user.Username }, user);
        }

        // PUT: api/Users/{username}
        [HttpPut]
        public IHttpActionResult PutUser(string username, User user)
        {
            var existingUser = users.SingleOrDefault(u => u.Username == username);
            if (existingUser == null)
            {
                return NotFound();
            }
            existingUser.Password = user.Password;
            existingUser.Name = user.Name;
            existingUser.Lastname = user.Lastname;
            existingUser.Email = user.Email;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Gender = user.Gender;
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Users/{username}
        [HttpDelete]
        public IHttpActionResult DeleteUser(string username)
        {
            var user = users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }
            users.Remove(user);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
