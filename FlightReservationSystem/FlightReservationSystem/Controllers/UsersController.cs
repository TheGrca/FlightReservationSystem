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
        private List<User> users
        {
            get
            {
                return System.Web.HttpContext.Current.Application["Users"] as List<User>;
            }
            set
            {
                System.Web.HttpContext.Current.Application["Users"] = value;
            }
        }

        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return users;
        }

        [HttpGet]
        [Route("api/users/{username}")]
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
            foreach(User u in users)
            {
                Debug.WriteLine(u.ToString());
            }
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
        [Route("api/users/register")]
        public IHttpActionResult Register(User user)
        {
            if (users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }

            if (!DateTime.TryParse(user.DateOfBirth.ToString(), out DateTime parsedDate))
            {
                return BadRequest("Invalid date format.");
            }
            user.DateOfBirth = parsedDate;
            user.TypeOfUser = TypeOfUser.Traveler;
            user.Reservations = new List<Reservation>();

            users.Add(user);
            // Uncomment and implement SaveUsers method to persist data
            // SaveUsers();
            return Ok(user);
        }

        [HttpPost]
        [Route("api/users/updateProfile")]
        public IHttpActionResult UpdateProfile(User updatedUser)
        {
            try
            {
                var existingUser = users.FirstOrDefault(u => u.Username == updatedUser.Username);
                if (existingUser == null)
                {
                    return NotFound(); // Handle case where user is not found
                }

                // Update user details (except username which is unique and should not change)
                existingUser.Password = updatedUser.Password;
                existingUser.Name = updatedUser.Name;
                existingUser.Lastname = updatedUser.Lastname;
                existingUser.Email = updatedUser.Email;
                existingUser.DateOfBirth = updatedUser.DateOfBirth;
                existingUser.Gender = updatedUser.Gender;
                updatedUser.TypeOfUser = existingUser.TypeOfUser;

                // Save updated users list back to JSON file
                Models.User.SaveUsersToJson(users);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // Handle any exceptions
            }
        }


    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
