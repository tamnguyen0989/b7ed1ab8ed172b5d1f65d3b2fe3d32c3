using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftStockInViewModel
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<long> Total { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public string StatusId { get; set; }
        public string CategoryId { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<int> FromBranchId { get; set; }
        public Nullable<int> ToBranchId { get; set; }
        public string FromBranch { get; set; }
        public string ToBranch { get; set; }
        public string FromBranchStatusId { get; set; }
        public string ToBranchStatusId { get; set; }
        public string FromBranchStatus { get; set; }
        public string FromBranchStatusCss { get; set; }
        public string ToBranchStatus { get; set; }
        public string ToBranchStatusCss { get; set; }
        public string SupplierStatusId { get; set; }
        public string SupplierStatus { get; set; }
        public string SupplierStatusCss { get; set; }
        public string CreatedByName { get; set; }
        public string UpdateddByName { get; set; }
        public string SupplierName { get; set; }
        public string Category { get; set; }
        public DateTime StockinDate { get; set; }
        public DateTime StockoutDate { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? PaymentStatusId { get; set; }

        public IEnumerable<SoftStockInProductViewModel> SoftStockInDetails { get; set; }
        public virtual SoftStockInStatusViewModel SoftStockInStatu { get; set; }
        public virtual SoftSupplierViewModel SoftSupplier { get; set; }
    }
    public class SoftStockInThenOutViewModel
    {
        public IEnumerable<SoftStockInProductViewModel> SoftStockInDetails { get; set; }
    }
    public class SoftStockInProductViewModel
    {
        public int id { get; set; }
        public string masp { get; set; }
        public string tensp { get; set; }
        public Nullable<int> PriceBase { get; set; }
        public Nullable<int> PriceNew { get; set; }
        public Nullable<int> PriceBaseOld { get; set; }
        public Nullable<int> PriceAvg { get; set; }
        public Nullable<int> PriceRef { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string SupplierName { get; set; }
        public double StockTotal { get; set; }

        public IEnumerable<ShopImageViewModel> shop_image { get; set; }
        public ShopSanPhamViewModel shop_sanpham { get; set; }
    }
    public class SoftStockInStatusViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CssClass { get; set; }
    }
    public class SoftStockInDetailViewModel
    {
        public int StockInId { get; set; }
        public int ProductId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> PriceNew { get; set; }

        public SoftStockInViewModel SoftStockIn { get; set; }
        public ShopSanPhamViewModel shop_sanpham { get; set; }
    }
    public class BookFilterViewModel
    {
        public List<SoftStockInStatusViewModel> selectedBookStatusFilters { get; set; }
        public List<SoftSupplierViewModel> selectedSupplierFilters { get; set; }
        public List<SoftBranchViewModel> selectedBranchFilters { get; set; }
        public List<SoftStockInCategoryViewModel> selectedStockinCategoryFilters { get; set; }
        public List<SoftStockInPaymentStatusViewModel> selectedPaymentStatusFilters { get; set; }

        public int branchId { get; set; }
        public string categoryId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string filter { get; set; }
        public string sortBy { get; set; }
        public DateTime startDateFilter { get; set; }
        public DateTime endDateFilter { get; set; }
        public DateTime startStockinDateFilter { get; set; }
        public DateTime endStockinDateFilter { get; set; }
        public DateTime startStockoutDateFilter { get; set; }
        public DateTime endStockoutDateFilter { get; set; }
        public Nullable<int> fromBranchId { get; set; }
        public Nullable<int> toBranchId { get; set; }
        public int? isSupplierStockOut { get; set; }
        public int? isListStockout { get; set; }
        public int? isListStockin { get; set; }
        public int? isListBook { get; set; }
        public int? isListBranchBook { get; set; }
    }
    public class SoftStockInSearchViewModel
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<long> Total { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public string StatusId { get; set; }
        public string CategoryId { get; set; }
        public string SupplierName { get; set; }
        public string StatusName { get; set; }
        public string StatusCss { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<int> FromBranchId { get; set; }
        public Nullable<int> ToBranchId { get; set; }
        public string FromBranch { get; set; }
        public string ToBranch { get; set; }
        public string FromBranchStatusId { get; set; }
        public string ToBranchStatusId { get; set; }
        public string FromBranchStatus { get; set; }
        public string FromBranchStatusCss { get; set; }
        public string ToBranchStatus { get; set; }
        public string ToBranchStatusCss { get; set; }
        public string SupplierStatusId { get; set; }
        public string SupplierStatus { get; set; }
        public string SupplierStatusCss { get; set; }
        public string Category { get; set; }
        public string CreatedDateConvert { get; set; }
        public Nullable<System.DateTime> StockInDate { get; set; }
        public Nullable<System.DateTime> StockOutDate { get; set; }
        public string StockInDateConvert { get; set; }
        public string StockOutDateConvert { get; set; }
        public string PaymentStatus { get; set;}
        public int? PaymentStatusId { get; set; }
        public string FromSuppliers { get; set; }


        public IEnumerable<SoftStockInProductViewModel> SoftStockInDetails { get; set; }
        public SoftStockInPaymentMethodViewModel SoftStockInPaymentMethod { get; set; }

    }
    public class SoftStockInCategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class BookPrintViewModel
    {
        public IEnumerable<SoftStockInDetailViewModel> BookDetails { get; set; }
        public SoftSupplierViewModel Supplier { get; set; }
    }
    public class BookExcelViewModel
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
    }
    public class ThenOutViewModel
    {
        public SoftBranchViewModel ToBranch { get; set; }
        public List<ShopSanPhamSoldByDateViewModel> Products { get; set; }
    }
    public class SoftStockInAddReturnViewModel
    {
        public bool stockoutAble { get; set; }
        public bool updatedPrice { get; set; }
        public List<SoftStockInProductViewModel> SoftStockInDetails { get; set; }
    }
    public class UpdatePaymentInputViewModel
    {
        public int stockinId { get; set; }
        public int paymentStatusId { get; set; }
        public int? paymentMethodId { get; set; }       
        public int? updatedBy { get; set; }
    }
}