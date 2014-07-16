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
        EdartEntities model = new EdartEntities();
        public Listing GetListing(int listingId)
        {
            var domainModel = model.Offers.Where(m => m.ID == listingId);
            if (domainModel != null)
            {
                var domainModelItem = domainModel.First();
                Listing listingModel = new Listing();
                MapListing(MappingType.OfferToListing, listingModel, domainModelItem);
                return listingModel;
            }
            return null;
        }
        
        public IEnumerable<Listing> GetListings()
        {
            var domainModel = model.Offers;
            IList<Listing> listings = new List<Listing>();
            foreach (Offer domainModelItem in domainModel)
            {
                var viewModel = new Listing();
                MapListing(MappingType.OfferToListing, viewModel, domainModelItem);
                listings.Add(viewModel);
            }
            return listings;
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

        #region Private
        private enum MappingType
        {
            ListingToOffer = 0,
            OfferToListing = 2
        }
        private User GetUser(string userEmail)
        {
            var userObject = model.Users.Where(m => m.Email == userEmail);
            if (userObject != null)
                return userObject.First();

            return null;
        }
        private void MapListing(MappingType typeOfMapping, Listing viewModel, Offer domainModel)
        {
            if (typeOfMapping == MappingType.ListingToOffer)
            {
                if (viewModel != null)
                {
                    //domainModel. viewModel.DateCreated;
                    domainModel.ID = viewModel.Id;
                    domainModel.Title = viewModel.Title;
                    var userObject = GetUser(viewModel.UserName);
                    if (userObject != null)
                    {
                        domainModel.UserID = userObject.ID;
                    }
                }
            }
            else
            {
                viewModel.Id = domainModel.ID;
                viewModel.Title = domainModel.Title;
            }
        }
        #endregion
    }
}
