//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftBBM.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ApplicationUserSoftBranch
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string Description { get; set; }
    
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual SoftBranch SoftBranch { get; set; }
    }
}
