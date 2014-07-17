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
        private EdartEntities model = new EdartEntities();
        public Listing GetListing(string listingId)
        {
            var domainModel = model.Offers.Where(m => m.ID == listingId).ToList();
            if (domainModel.Count>0)
            {
                var domainModelItem = domainModel.First();
                var listingModel = new Listing();
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
            var domainModel = new Offer();
            try
            {
                
                MapListing(MappingType.ListingToOffer, listing, domainModel);
                domainModel.ID = Guid.NewGuid().ToString();
                model.Offers.Add(domainModel);
                model.SaveChanges();
                listing.Id = domainModel.ID;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            return listing;
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
        private User GetOrCreateUser(string userEmail,string userName,string location,string photoUrl)
        {
            var userObject = model.Users.Where(m => m.Email == userEmail).ToList();
            if (userObject.Count>0)
                return userObject.First();


            var userObjectToAdd = new User();
            userObjectToAdd.ID = Guid.NewGuid().ToString();
            userObjectToAdd.Location = location;
            userObjectToAdd.Name = userName;
            userObjectToAdd.PhotoUrl = photoUrl;
            userObjectToAdd.Email = userEmail;
            return CreateUser(userObjectToAdd);
        }
        private User CreateUser(User userObject)
        {
            
            try
            {
                model.Users.Add(userObject);
                model.SaveChanges();
                
            }
            catch (Exception ex)
            {
                

                throw ex;
            }
            return userObject;
        }

        private Category CreateCategory(string description)
        {
            var categoryObject = new Category();
            categoryObject.Name = description;
            categoryObject.ID = Guid.NewGuid().ToString();
            model.Categories.Add(categoryObject);
            model.SaveChanges();
            return categoryObject;
        }

        private Category GetOrCreateCategory(string description)
        {
            var categoryObject = model.Categories.Where(m => m.Name == description).ToList();
            if (categoryObject.Count > 0)
                return categoryObject.First();

            return CreateCategory(description);
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
                    var userObject = GetOrCreateUser(viewModel.UserEmail,viewModel.UserName,viewModel.Location,"");
                        domainModel.UserID = userObject.ID;
                   
                    domainModel.Status = Convert.ToInt16(viewModel.Status);
                    var categoryObject = GetOrCreateCategory(viewModel.CategoryName);
                    if (categoryObject != null)
                    {
                        domainModel.CategoryID = categoryObject.ID;
                    }
                    domainModel.CreatedDateTime = viewModel.DateCreated;
                    domainModel.ModifiedDate = viewModel.DateModified;
                    domainModel.Photo = viewModel.Photo;
                    domainModel.Description = viewModel.Description;
                }
            }
            else
            {
                viewModel.Id = domainModel.ID;
                viewModel.Title = domainModel.Title;
                viewModel.Status = (ListingStatus)domainModel.Status;
                viewModel.Type = ListingType.Offered;
                viewModel.DateModified = domainModel.ModifiedDate;
                viewModel.DateCreated = domainModel.CreatedDateTime;
                viewModel.Description = domainModel.Description;
                viewModel.UserEmail = domainModel.User.Email;
                viewModel.UserName = domainModel.User.Name;
                viewModel.Location = domainModel.User.Location;
                viewModel.Photo = domainModel.Photo;
            }
        }
        #endregion
    }
}
