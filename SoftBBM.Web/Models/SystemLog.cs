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
    
    public partial class SystemLog
    {
        public int Id { get; set; }
        public Nullable<int> KeyId { get; set; }
        public string Value { get; set; }
        public string AppName { get; set; }
        public string Description { get; set; }
        public Nullable<int> Type { get; set; }
        public string TypeName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }
}