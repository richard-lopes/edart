using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edart.Common
{
    public class Listing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ListingType Type {get; set; }
        public ListingStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string UserName { get; set; }
    }


    public enum ListingType
    {
        Offered = 1,
        Wanted = 2
    }

    public enum ListingStatus
    {
        Active = 1,
        Sold = 2,
        Cancelled = 3

    }
    
}
