using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftOrderViewModel
    {
    }

    public class SoftOrderDetailViewModel
    {
        public int id { get; set; }
        public string masp { get; set; }
        public string tensp { get; set; }
        public string image { get; set; }
        public bool NotDiscountMember { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> PriceBeforeDiscount { get; set; }
        public Nullable<double> Quantity { get; set; }
        public Nullable<double> chieudai { get; set; }
        public Nullable<double> chieurong { get; set; }
        public Nullable<double> chieucao { get; set; }
        public Nullable<double> kg { get; set; }
        public Nullable<bool> freeship { get; set; }
        public Nullable<int> PriceWholesale { get; set; }
        public Nullable<int> CategoryId { get; set; }
    }
}