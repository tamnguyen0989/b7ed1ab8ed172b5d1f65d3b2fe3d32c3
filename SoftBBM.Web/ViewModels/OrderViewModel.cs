using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class OrderViewModel
    {
        public long id { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<int> makh { get; set; }
        public Nullable<int> diemsp { get; set; }
        public Nullable<int> vanglai { get; set; }
        public string ghichu { get; set; }
        public string noidung { get; set; }
        public Nullable<int> ship { get; set; }
        public Nullable<long> tongtien { get; set; }
        public Nullable<int> idgiogiao { get; set; }
        public Nullable<System.DateTime> ngaydat { get; set; }
        public Nullable<int> pttt { get; set; }
        public Nullable<int> ptgh { get; set; }
        public Nullable<int> typeconfim { get; set; }
        public Nullable<bool> dagiao { get; set; }
        public Nullable<System.DateTime> ngaygiao { get; set; }
        public Nullable<int> datru_diem { get; set; }
        public Nullable<bool> dahuy { get; set; }
        public Nullable<System.DateTime> ngayhuy { get; set; }
        public string thongtinxedo { get; set; }
        public string tinhtrang { get; set; }
        public string donhangtu { get; set; }
        public Nullable<int> typeconfimcall { get; set; }
        public string tenptgh { get; set; }
        public Nullable<int> phithuho { get; set; }
        public Nullable<bool> dain { get; set; }
        public Nullable<int> ChannelId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> ShipperId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string Code { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> DiscountMoney { get; set; }
        public Nullable<int> DiscountPercent { get; set; }
        public string DiscountCode { get; set; }

        public IEnumerable<OrderDetailViewModel> OrderDetails { get; set; }
        public khachhangViewModel Customer { get; set; }
    }
    public class OrderViewModelAddInput
    {
        public long id { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<int> makh { get; set; }
        public Nullable<int> diemsp { get; set; }
        public Nullable<int> vanglai { get; set; }
        public string ghichu { get; set; }
        public string noidung { get; set; }
        public Nullable<double> ship { get; set; }
        public Nullable<long> tongtien { get; set; }
        public Nullable<int> idgiogiao { get; set; }
        public Nullable<System.DateTime> ngaydat { get; set; }
        public Nullable<int> pttt { get; set; }
        public Nullable<int> ptgh { get; set; }
        public Nullable<int> typeconfim { get; set; }
        public Nullable<bool> dagiao { get; set; }
        public Nullable<System.DateTime> ngaygiao { get; set; }
        public Nullable<int> datru_diem { get; set; }
        public Nullable<bool> dahuy { get; set; }
        public Nullable<System.DateTime> ngayhuy { get; set; }
        public string thongtinxedo { get; set; }
        public string tinhtrang { get; set; }
        public string donhangtu { get; set; }
        public Nullable<int> typeconfimcall { get; set; }
        public string tenptgh { get; set; }
        public Nullable<int> phithuho { get; set; }
        public Nullable<bool> dain { get; set; }
        public Nullable<int> ChannelId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> ShipperId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string Code { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> DiscountMoney { get; set; }
        public Nullable<int> DiscountPercent { get; set; }
        public string DiscountCode { get; set; }

        public IEnumerable<OrderDetailViewModel> OrderDetails { get; set; }
        public khachhangViewModel Customer { get; set; }
    }
    public class OrderFilterViewModel
    {
        public List<donhangStatusViewModel> selectedOrderStatusFilters { get; set; }
        public List<ApplicationUserViewModel> selectedSellerFilters { get; set; }
        public List<ApplicationUserViewModel> selectedShipperFilters { get; set; }
        public List<EcommerceShipperViewModel> selectedEcommerceShipperFilters { get; set; }
        public List<donhangStatusViewModel> selectedPaymentFilters { get; set; }
        public int branchId { get; set; }
        public int? channelId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string filter { get; set; }
        public string sortBy { get; set; }
        public DateTime startDateFilter { get; set; }
        public DateTime endDateFilter { get; set; }
    }
    public class StatusOrdersViewModel
    {
        public IEnumerable<donhangListViewModel> Orders { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class StatusOrdersViewModelV2
    {
        public List<string> Orders { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class ShipperOrdersViewModel
    {
        public IEnumerable<donhangListViewModel> Orders { get; set; }
        public int UserId { get; set; }
        public int ShipperId { get; set; }
    }
    public class OrdersToPrintInputViewModel
    {
        public List<int> orderIds { get; set; }
        public string username { get; set; }
    }
    public class UpdateCompletedTikiOrderInputVM
    {
        public List<string> orderIds { get; set; }
        public int UserId { get; set; }
    }
    public class EcommerceShipperViewModel
    {
        public string Name { get; set; }
    }
}