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
    
    public partial class shop_image
    {
        public int id { get; set; }
        public Nullable<int> RefId { get; set; }
        public string url { get; set; }
        public string alt { get; set; }
        public Nullable<bool> hide { get; set; }
        public Nullable<bool> ImgPrimary { get; set; }
    
        public virtual shop_sanpham shop_sanpham { get; set; }
    }
}
