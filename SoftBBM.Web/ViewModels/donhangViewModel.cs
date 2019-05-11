using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class donhangListViewModel
    {
        public long id { get; set; }
        public Nullable<int> makh { get; set; }
        public string ghichu { get; set; }
        public Nullable<long> tongtien { get; set; }
        public Nullable<System.DateTime> ngaydat { get; set; }
        public Nullable<int> ChannelId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string StatusName { get; set; }
        public string CustomerName { get; set; }
        public string ShipperName { get; set; }
        public string StatusCss { get; set; }
        public string CreatedName { get; set; }
        public string CreatedUserName { get; set; }
        public string ShipperUserName { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string CreatedDateConvert { get; set; }
        public string Channel { get; set; }
        public int StatusId { get; set; }
        public string Code { get; set; }
        public int ShipperId { get; set; }
        public string StatusPrint { get; set; }
        public Nullable<int> idgiogiao { get; set; }
        public Nullable<int> BranchId { get; set; }
    }
    public class donhangDetailViewModel
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
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<int> ShipperId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string Code { get; set; }
        public Nullable<int> Status { get; set; }
        public string tentp { get; set; }
        public string tenquan { get; set; }
        public string tenpttt { get; set; }
        public string giogiao { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public int Discount { get; set; }
        public Nullable<int> DiscountMoney { get; set; }
        public Nullable<int> DiscountPercent { get; set; }
        public string DiscountCode { get; set; }
        public string StatusPrint { get; set; }
        public string chitietgiogiao { get; set; }

        public SoftChannelViewModel SoftChannel { get; set; }
        public donhangStatusViewModel donhangStatu { get; set; }
        public khachhangViewModel khachhang { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
        public SoftBranchViewModel SoftBranch { get; set; }
        public IEnumerable<donhangctViewModel> donhang_ct { get; set; }
        public ApplicationUserViewModel ApplicationUser1 { get; set; }
    }
    public class donhangctViewModel
    {
        public long Id { get; set; }
        public long Sodh { get; set; }
        public long IdPro { get; set; }
        public double Soluong { get; set; }
        public Nullable<int> Dongia { get; set; }
        public Nullable<int> Dongiakm { get; set; }
        public string masp;

        public shopbientheViewModel shop_bienthe { get; set; }
    }
    public class shopbientheViewModel
    {
        public long id { get; set; }
        public Nullable<int> idsp { get; set; }
        public string title { get; set; }
        public string title_us { get; set; }
        public Nullable<int> gia { get; set; }
        public Nullable<int> giasosanh { get; set; }
        public bool isdelete { get; set; }
        public string tensp { get; set; }
        public string masp { get; set; }
    }
    public class donhangUpdateViewModel
    {
        public long id { get; set; }
        public Nullable<int> makh { get; set; }
        public string ghichu { get; set; }
        public int ShipperId { get; set; }
        public int UserId { get; set; }
    }
    public class donhangAfterEditViewModel
    {
        public SoftChannelViewModel channel { get; set; }
        public khachhangViewModel customer { get; set; }
        public string historyOrder { get; set; }
        public int? shipperId { get; set; }
        public IEnumerable<SoftOrderDetailViewModel> orderDetails { get; set; }
    }
    public class donhangExcel
    {
        public long id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateConvert { get; set; }
        public string chitiet { get; set; }
        public Nullable<long> tongtien { get; set; }
        public string ghichu { get; set; }
        public string StatusName { get; set; }       
    }
    public class donhangExcelNoId
    {
        public string CreatedDateConvert { get; set; }
        public string chitiet { get; set; }
        public Nullable<long> tongtien { get; set; }
        public string ghichu { get; set; }
        public string StatusName { get; set; }
    }
}