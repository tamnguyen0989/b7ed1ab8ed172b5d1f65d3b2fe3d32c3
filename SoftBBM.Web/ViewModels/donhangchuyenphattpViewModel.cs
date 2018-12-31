using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class donhangchuyenphattpViewModel
    {
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

        public IEnumerable<donhangchuyenphattinhViewModel> donhang_chuyenphat_tinh { get; set; }
}
}