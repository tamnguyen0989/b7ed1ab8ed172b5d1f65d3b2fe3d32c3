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
    
    public partial class donhang_chuyenphat_danhsachdiachifuta
    {
        public int id { get; set; }
        public int idtp { get; set; }
        public string tenchinhanh { get; set; }
        public string diachi { get; set; }
    
        public virtual donhang_chuyenphat_tp donhang_chuyenphat_tp { get; set; }
    }
}
