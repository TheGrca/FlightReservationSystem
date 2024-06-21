using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using FlightReservationSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using static System.Collections.Specialized.BitVector32;

namespace FlightReservationSystem.Controllers
{


    public class UsersController : ApiController
    {
        private static List<User> users;
        public UsersController()
        {
            users = Models.User.LoadUsers();
        }

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

        [HttpPost]
        [Route("api/users/login")]
        public IHttpActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = users.SingleOrDefault(u => u.Username == loginModel.Username && u.Password == loginModel.Password);
            if (user != null)
            {
                System.Web.HttpContext.Current.Session["User"] = user;
                return Ok(new
                {
                    success = true,
                    username = user.Username // return the username
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/users/logout")]
        public IHttpActionResult Logout()
        {
            // Clear session when user logs out
            System.Web.HttpContext.Current.Session["User"] = null;

            return Ok(new { success = true, message = "Logout successful" });
        }

        [HttpPost]
        public IHttpActionResult Register(User user)
        {
            if (users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }

            // Set default values
            user.TypeOfUser = TypeOfUser.Traveler;
            user.Reservations = new List<Reservation>();

            users.Add(user);
            //SaveUsers();

            return Ok(user);
        }


    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
