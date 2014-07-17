//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace adart.repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class Offer
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<decimal> StartPrice { get; set; }
        public Nullable<decimal> ReservePrice { get; set; }
        public Nullable<decimal> EndDate { get; set; }
        public short Status { get; set; }
        public string UserID { get; set; }
        public string CategoryID { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
    }
}
