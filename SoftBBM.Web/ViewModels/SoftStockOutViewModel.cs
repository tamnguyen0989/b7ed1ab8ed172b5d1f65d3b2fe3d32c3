using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftStockOutViewModel
    {
    }


    public class InputShopSanPhamSoldByDate
    {
        public int branchId { get; set; }
        public DateTime startDateFilter { get; set; }
        public DateTime endDateFilter { get; set; }
    }
}