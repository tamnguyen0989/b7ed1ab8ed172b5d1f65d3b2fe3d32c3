using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftStockViewModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ProductId { get; set; }
        public double Stock_Total { get; set; }
        public double Stock_Total_All { get; set; }
        public int PriceShop { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Vat { get; set; }
        public int? PriceChannel { get; set; }
        public int? PriceDiscount { get; set; }
        public DateTime? StartDateDiscount { get; set; }
        public DateTime? EndDateDiscount { get; set; }

        public ShopSanPhamViewModel shop_sanpham { get; set; }
        public SoftBranchViewModel SoftBranch { get; set; }
    }
    public class SoftStockTotalAllViewModel
    {
        public string Name { get; set; }
        public double Stock_Total { get; set; }
    }
    public class SoftAdjustmentStockViewModel
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }

        public ApplicationUserViewModel ApplicationUser { get; set; }
        public ApplicationUserViewModel ApplicationUser1 { get; set; }
        public IEnumerable<SoftAdjustmentStockDetailViewModel> SoftAdjustmentStockDetails { get; set; }
        public SoftBranchViewModel SoftBranch { get; set; }
    }
    public class SoftAdjustmentStockDetailViewModel
    {
        public int AdjustmentId { get; set; }
        public int ProductId { get; set; }
        public Nullable<int> Quantity { get; set; }

        public ShopSanPhamViewModel shop_sanpham { get; set; }
    }
    public class SoftAdjustmentStockAddViewModel
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public IEnumerable<SoftAdjustmentStockDetailAddViewModel> SoftAdjustmentStockDetails { get; set; }

    }
    public class SoftAdjustmentStockDetailAddViewModel
    {
        public int id { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
    public class AdjustmentStockFilterViewModel
    {
        public int branchId { get; set; }
        public string categoryId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string filter { get; set; }
        public string sortBy { get; set; }
        public DateTime startDateFilter { get; set; }
        public DateTime endDateFilter { get; set; }
    }
    public class SoftStockViewModelExcel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Supplier { get; set; }
        public double Stock { get; set; }
        public double StockAll { get; set; }
        public int PriceOld { get; set; }
        public int PriceNew { get; set; }
        public string Url { get; set; }
        public int? ONL { get; set; }
        public int? ONLkm { get; set; }
        public string ONLstart { get; set; }
        public string ONLend { get; set; }
        public int? CHA { get; set; }
        public int? CHAkm { get; set; }
        public string CHAstart { get; set; }
        public string CHAend { get; set; }
        public int? LZD { get; set; }
        public int? LZDkm { get; set; }
        public string LZDstart { get; set; }
        public string LZDend { get; set; }
        public int? SPE { get; set; }
        public int? SPEkm { get; set; }
        public string SPEstart { get; set; }
        public string SPEend { get; set; }

    }
    public class SoftStockViewModelExcelNoId
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Supplier { get; set; }
        public double Stock { get; set; }
        public double StockAll { get; set; }
        public int PriceOld { get; set; }
        public int PriceNew { get; set; }
        public string Url { get; set; }
        public int? ONL { get; set; }
        public int? ONLkm { get; set; }
        public string ONLstart { get; set; }
        public string ONLend { get; set; }
        public int? CHA { get; set; }
        public int? CHAkm { get; set; }
        public string CHAstart { get; set; }
        public string CHAend { get; set; }
        public int? LZD { get; set; }
        public int? LZDkm { get; set; }
        public string LZDstart { get; set; }
        public string LZDend { get; set; }
        public int? SPE { get; set; }
        public int? SPEkm { get; set; }
        public string SPEstart { get; set; }
        public string SPEend { get; set; }

    }
    public class SoftStockViewModelExcelSampleImport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public int PriceOld { get; set; }
        public int PriceNew { get; set; }
        //public string Url { get; set; }
        public int? ONL { get; set; }
        public int? ONLkm { get; set; }
        public string ONLstart { get; set; }
        public string ONLend { get; set; }
        public int? CHA { get; set; }
        public int? CHAkm { get; set; }
        public string CHAstart { get; set; }
        public string CHAend { get; set; }
        public int? LZD { get; set; }
        public int? LZDkm { get; set; }
        public string LZDstart { get; set; }
        public string LZDend { get; set; }
        public int? SPE { get; set; }
        public int? SPEkm { get; set; }
        public string SPEstart { get; set; }
        public string SPEend { get; set; }

    }
    public class SoftStockSearchFilterViewModel
    {
        public int branchId { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        //public List<ShopSanPhamCategoryViewModel> selectedProductCategoryFilters { get; set; }
        //public List<SoftSupplierViewModel> selectedSupplierFilters { get; set; }
        //public List<ShopSanPhamStatusViewModel> selectedProductStatusFilters { get; set; }
        //public List<SoftSupplierVatStatuViewModel> selectedVatStatusFilters { get; set; }
        //public List<ShopSanPhamHideStatusViewModel> selectedHideStatusFilter { get; set; }
        public List<int> selectedProductCategoryFilters { get; set; }
        public List<int> selectedSupplierFilters { get; set; }
        public List<string> selectedProductStatusFilters { get; set; }
        public List<int> selectedVatStatusFilters { get; set; }
        public List<int> selectedHideStatusFilter { get; set; }

        public Nullable<int> selectedStockFilter { get; set; }
        public Nullable<int> selectedStockFilterValue { get; set; }
        public Nullable<int> selectedStockTotalFilter { get; set; }
        public Nullable<int> selectedStockTotalFilterValue { get; set; }
        public string stringFilter { get; set; }
        public string sortBy { get; set; }
        public int channelId { get; set; }
    }
    public class BranchProductStockOutputViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double StockTotal { get; set; }
    }
    public class ProductChannelPriceInputViewModel
    {
        public int productId { get; set; }
        public int channelId { get; set; }
        public int? priceChannel { get; set; }
        public int? priceDiscount { get; set; }
        public DateTime? startDateDiscount { get; set; }
        public DateTime? endDateDiscount { get; set; }
    }
    public class DetailProductOutputViewModel
    {
        public int id { get; set; }
        public string productCode { get; set; }
        public string barCode { get; set; }
        public string name { get; set; }
        public int priceBase { get; set; }
        public int priceBaseOld { get; set; }
        public int priceAvg { get; set; }
        public int priceRef { get; set; }
        public int? shopeeId { get; set; }
        public ICollection<SoftChannelProductPriceViewModel> SoftChannelProductPrices { get; set; }

        public ShopSanPhamCategoryViewModel productCategory { get; set; }
        public SoftSupplierViewModel supplier { get; set; }
        public ShopSanPhamStatusViewModel productStatus { get; set; }
    }
    public class DetailProductInputViewModel
    {
        public int id { get; set; }
        public string productCode { get; set; }
        public string barCode { get; set; }
        public string name { get; set; }
        public int priceBase { get; set; }
        public int priceBaseOld { get; set; }
        public int priceAvg { get; set; }
        public int priceRef { get; set; }
        public int? categoryId { get; set; }
        public int? supplierId { get; set; }
        public string statusId { get; set; }
        public int userId { get; set; }
        public int? shopeeId { get; set; }

        public List<SoftChannelProductPriceViewModel> SoftChannelProductPrices { get; set; }

    }
}