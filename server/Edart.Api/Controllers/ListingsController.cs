using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Edart.Common;
using adart.repository;

namespace Edart.Api.Controllers
{
    public class ListingsController : BaseController
    {

        private readonly IListingRepository _repository;

        public ListingsController()
        {
            _repository = new ListingRepository();
        }


        [HttpGet]
        public HttpResponseMessage GetCurrentListings()
        {
            var listings = _repository.GetListings().Where(l => l.Status == ListingStatus.Active).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, listings);
        }


        [HttpGet]
        public HttpResponseMessage GetListing(int id)
        {
            var listing = _repository.GetListing(id);
            if (listing == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, listing);
        }


        [HttpPost]
        public HttpResponseMessage CreateListing([FromBody]Listing listing)
        {
            listing.DateCreated = DateTime.Now;
            listing.DateModified = DateTime.Now;

            listing = _repository.AddListing(listing);

            var response = Request.CreateResponse(HttpStatusCode.Created, listing);
            var link = Url.Link("DefaultApi", new { id = listing.Id });
            response.Headers.Location = new Uri(link);

            return response;
        }


        [HttpPost]
        public HttpResponseMessage UpdateListing([FromBody]Listing listing)
        {
            listing.DateModified = DateTime.Now;
            if (!_repository.UpdateListing(listing))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, listing);
        }


        [HttpPost]
        public HttpResponseMessage CancelListing(int id)
        {
            var listing = _repository.GetListing(id);
            if (listing == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            listing.Status = ListingStatus.Cancelled;

            _repository.UpdateListing(listing);
            
            var response = Request.CreateResponse(HttpStatusCode.OK, listing);
            return response;
        }

    }
}
