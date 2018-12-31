using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SoftBBM.Web.ViewModels
{

    public class ShopImageViewModel
    {
        public int id { get; set; }
        public Nullable<int> RefId { get; set; }
        public string url { get; set; }
        public string alt { get; set; }
        public Nullable<bool> hide { get; set; }
        public Nullable<bool> ImgPrimary { get; set; }

        //public virtual ShopSanPhamViewModel shop_sanpham { get; set; }
    }
}