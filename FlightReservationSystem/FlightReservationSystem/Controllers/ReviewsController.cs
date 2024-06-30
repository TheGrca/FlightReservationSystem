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
    public class ReviewsController : ApiController
    {
        private List<Review> reviews
        {
            get
            {
                return System.Web.HttpContext.Current.Application["Reviews"] as List<Review>;
            }
            set
            {
                System.Web.HttpContext.Current.Application["Reviews"] = value;
            }
        }

        [HttpGet]
        [Route("api/reviews")]
        public IHttpActionResult GetReviews()
        {
            if (reviews == null || !reviews.Any())
            {
                return NotFound();
            }

            return Ok(reviews);
        }

        [HttpGet]
        [Route("api/reviews/{id}")]
        public IHttpActionResult GetReview(int id)
        {
            var review = reviews.SingleOrDefault(r => r.Id == id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpPut]
        [Route("api/reviews/update/{id}")]
        public IHttpActionResult UpdateReview(int id, Review updatedReview)
        {
            var review = reviews.SingleOrDefault(r => r.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            review.ReviewStatus = updatedReview.ReviewStatus;

            return Ok(review);
        }

        [HttpGet]
        [Route("api/reviews/airline/{airlineId}")]
        public IHttpActionResult GetApprovedReviewsForAirline(int airlineId)
        {
            var approvedReviews = reviews
                .Where(r => r.Airline.Id == airlineId && r.ReviewStatus == ReviewStatus.Approved)
                .ToList();

            if (!approvedReviews.Any())
            {
                return NotFound();
            }

            return Ok(approvedReviews);
        }

        [HttpPost]
        [Route("api/reviews/add")]
        public IHttpActionResult AddReview([FromBody] Review review)
        {
            if (reviews == null)
            {
                reviews = new List<Review>();
            }

            int newId = reviews.Count == 0 ? 0 : reviews.Max(r => r.Id) + 1;
            review.Id = newId;
            reviews.Add(review);
            
            var airlines = System.Web.HttpContext.Current.Application["Airlines"] as List<Airline>;
            var airline = airlines.SingleOrDefault(a => a.Name == review.Airline.Name);
            if (airline != null)
            {
                if (airline.Reviews.Count() == 0)
                {
                    airline.Reviews = new List<Review>();
                }
                airline.Reviews.Add(review);
            }
            System.Web.HttpContext.Current.Application["Airlines"] = airlines;
            
            return Ok();
        }


    }
}
