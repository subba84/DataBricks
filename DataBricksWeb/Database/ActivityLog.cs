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
    
    public partial class ActivityLog
    {
        public int Id { get; set; }
        public string ActivityDetails { get; set; }
        public Nullable<int> UserId { get; set; }
        public string FullName { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string RoleName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ActivityCategory { get; set; }
        public string CompanyName { get; set; }
    }
}