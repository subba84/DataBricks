//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataBricksWeb.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lead
    {
        public int Id { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDate { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductName { get; set; }
        public string LeadDescription { get; set; }
        public Nullable<int> TotalModuleAmount { get; set; }
        public Nullable<int> PartnerId { get; set; }
        public string PartnerName { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ChangedBy { get; set; }
        public Nullable<System.DateTime> ChangedOn { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public Nullable<bool> IsConvertedtoPoc { get; set; }
    }
}
