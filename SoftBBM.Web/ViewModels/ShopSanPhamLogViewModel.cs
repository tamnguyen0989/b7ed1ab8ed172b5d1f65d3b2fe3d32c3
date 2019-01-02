using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ShopSanPhamLogViewModel
    {
        public long Id { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> Quantity { get; set; }
        public int StockTotal { get; set; }
        public int StockTotalAll { get; set; }
        public string CreatedDateConvert { get; set; }

        public ShopSanPhamViewModel shop_sanpham { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
    }

    public class ProductLogFilterViewModel
    {
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string filter { get; set; }
        //public string sortBy { get; set; }
        public DateTime startDateFilter { get; set; }
        public DateTime endDateFilter { get; set; }
        public DateTime startDateDeleteFilter { get; set; }
        public DateTime endDateDeleteFilter { get; set; }
        public int branchId { get; set; }
    }
}