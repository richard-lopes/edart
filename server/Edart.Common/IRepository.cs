using System;
using System.Collections.Generic;

namespace Edart.Common
{
    public interface IListingRepository
    {
        Listing GetListing(string listingId);
        IEnumerable<Listing> GetListings();
        Listing AddListing(Listing listing);
        Boolean UpdateListing(Listing listing);
        Boolean DeleteListing(int listingId);
    }
}
