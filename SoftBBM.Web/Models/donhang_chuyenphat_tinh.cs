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
    
    public partial class donhang_chuyenphat_tinh
    {
        public int id { get; set; }
        public Nullable<int> idtp { get; set; }
        public string tentinh { get; set; }
        public string tentinh_us { get; set; }
        public Nullable<bool> vungsau { get; set; }
        public Nullable<decimal> ship { get; set; }
        public Nullable<int> idvung { get; set; }
        public Nullable<int> phightk { get; set; }
        public Nullable<int> tgcongthem { get; set; }
        public Nullable<int> tgghtk { get; set; }
        public string mavungfuta { get; set; }
        public Nullable<int> tgxekhachfuta { get; set; }
        public Nullable<int> phighnhanh { get; set; }
        public string mavungvnpost { get; set; }
        public Nullable<bool> vungsauvnpost { get; set; }
        public string mavungvnpostnhanh { get; set; }
        public Nullable<int> phivnep { get; set; }
        public Nullable<int> tgvnep { get; set; }
        public Nullable<int> Priority { get; set; }
    
        public virtual donhang_chuyenphat_tp donhang_chuyenphat_tp { get; set; }
    }
}
