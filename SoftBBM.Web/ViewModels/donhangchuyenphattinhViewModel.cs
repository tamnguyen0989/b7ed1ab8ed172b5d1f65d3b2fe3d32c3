using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class donhangchuyenphattinhViewModel
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
        public Nullable<int> priority { get; set; }
    }
}