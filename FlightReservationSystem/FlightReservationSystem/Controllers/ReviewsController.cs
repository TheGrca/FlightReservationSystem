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

    }
}
