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
        public HttpResponseMessage GetCurrentListings(string token)
        {
            if (IsTokenValid(token))
            {
                var listings = _repository.GetListings().Where(l => l.Status == ListingStatus.Active).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, listings);
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }


        [HttpGet]
        public HttpResponseMessage GetListing(string id,string token)
        {
            if (IsTokenValid(token))
            {
                var listing = _repository.GetListing(id);
                if (listing == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return Request.CreateResponse(HttpStatusCode.OK, listing);
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }


        [HttpPost]
        public HttpResponseMessage CreateListing([FromBody] Listing listing,string token)
        {
            Listing li=new Listing();
            li.Description = "ss";
            li.Title = "fsfss";
            

            if (IsTokenValid("ff") && listing!=null)
            {
                listing.DateCreated = DateTime.Now;
                listing.DateModified = DateTime.Now;
                listing.Status = ListingStatus.Active;
                listing.UserName = token;
                listing = _repository.AddListing(listing);

                var response = Request.CreateResponse(HttpStatusCode.Created, listing);
                var link = Url.Link("DefaultApi", new {id = listing.Id});
                response.Headers.Location = new Uri(link);

                return response;
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }


        [HttpPost]
        public HttpResponseMessage UpdateListing([FromBody]Listing listing, string token)
        {
            if (IsTokenValid(token))
            {
                listing.DateModified = DateTime.Now;
                if (!_repository.UpdateListing(listing))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return Request.CreateResponse(HttpStatusCode.OK, listing);
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }


        [HttpPost]
        public HttpResponseMessage CancelListing(string id, string token)
        {
            if (IsTokenValid(token))
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
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        private bool IsTokenValid(string value)
        {
            if (value != null && value.Trim().Length >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
