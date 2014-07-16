using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edart.Common;

namespace adart.repository
{
    public class ListingRepository : IListingRepository
    {
        public Listing GetListing(int listingId)
        {
            var listing = new Listing();
            listing.Id = listingId;
            listing.Title = "My new listing";
            return listing;
        }

        public IEnumerable<Listing> GetListings()
        {
            throw new NotImplementedException();
        }

        public Listing AddListing(Listing listing)
        {
            throw new NotImplementedException();
        }

        public bool UpdateListing(Listing listing)
        {
            throw new NotImplementedException();
        }

        public bool DeleteListing(int listingId)
        {
            throw new NotImplementedException();
        }
    }
}
