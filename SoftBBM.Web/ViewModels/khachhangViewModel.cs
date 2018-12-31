using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class khachhangViewModel
    {
        public int MaKH { get; set; }
        public Nullable<int> idtp { get; set; }
        public Nullable<int> idquan { get; set; }
        public string hoten { get; set; }
        public string duong { get; set; }
        public string dienthoai { get; set; }
        public string email { get; set; }
        public string tendn { get; set; }
        //public string matkhau { get; set; }
        public int diem { get; set; }
        public bool konhanmail { get; set; }
        public Nullable<System.DateTime> ngaydangky { get; set; }
        public int userId { get; set; }
        public string cityName { get; set; }
        public string districtName { get; set; }

        public donhangchuyenphattpViewModel City { get; set; }
        public donhangchuyenphattinhViewModel District { get; set; }
        //public string City { get; set; }
        //public string District { get; set; }
    }
    public class CustomerFilterViewModel
    {
        //public List<donhangStatusViewModel> selectedOrderStatusFilters { get; set; }
        //public List<ApplicationUserViewModel> selectedSellerFilters { get; set; }
        //public List<ApplicationUserViewModel> selectedShipperFilters { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string filter { get; set; }
        //public string sortBy { get; set; }
        //public DateTime startDateFilter { get; set; }
        //public DateTime endDateFilter { get; set; }
    }
}