using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
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
        //[HttpPost]
        //public HttpResponseMessage Test1([FromBody] Test test1, string token)
        //{
          //  return null;
        //}

        [HttpPost]
        public HttpResponseMessage CreateListing([FromBody] Listing listing1,string token)
        {
            if (IsTokenValid(token) && listing1 != null)
            {
                listing1.DateCreated = DateTime.Now;
                listing1.DateModified = DateTime.Now;
                listing1.Status = ListingStatus.Active;
                listing1.UserEmail = token;
                listing1 = _repository.AddListing(listing1);

                var response = Request.CreateResponse(HttpStatusCode.Created, listing1);
                var link = Url.Link("DefaultApi", new {id = listing1.Id});
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
