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
    
    public partial class donhang_chuyenphat_tp
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public donhang_chuyenphat_tp()
        {
            this.donhang_chuyenphat_danhsachdiachifuta = new HashSet<donhang_chuyenphat_danhsachdiachifuta>();
            this.donhang_chuyenphat_tinh = new HashSet<donhang_chuyenphat_tinh>();
            this.khachhang_vanglai = new HashSet<khachhang_vanglai>();
        }
    
        public int id { get; set; }
        public string tentp { get; set; }
        public string tentp_us { get; set; }
        public string mavung { get; set; }
        public string thoigian { get; set; }
        public Nullable<int> idtinhtra { get; set; }
        public string uutien { get; set; }
        public Nullable<int> mavungcpn { get; set; }
        public Nullable<int> thoigiancpn { get; set; }
        public string mavungvnpostnhanh { get; set; }
        public Nullable<int> thoigianvnpostnhanh { get; set; }
        public string mavungvnpostbuukien { get; set; }
        public Nullable<int> thoigianvnpostbuukien { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<donhang_chuyenphat_danhsachdiachifuta> donhang_chuyenphat_danhsachdiachifuta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<donhang_chuyenphat_tinh> donhang_chuyenphat_tinh { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<khachhang_vanglai> khachhang_vanglai { get; set; }
    }
}
