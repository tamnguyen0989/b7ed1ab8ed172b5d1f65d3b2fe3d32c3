using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ShopSanPhamViewModel
    {
        public int id { get; set; }
        public string masp { get; set; }
        public string tensp { get; set; }
        public Nullable<int> PriceBase { get; set; }
        public Nullable<int> PriceBaseOld { get; set; }
        public Nullable<int> PriceAvg { get; set; }
        public Nullable<int> PriceRef { get; set; }
        public Nullable<int> PriceWholesale { get; set; }
        public int userId { get; set; }
        public string Barcode { get; set; }
        public int? CategoryId { get; set; }

        public ShopSanPhamStatusViewModel shop_sanphamStatus { get; set; }
        public SoftSupplierViewModel SoftSupplier { get; set; }
        public IEnumerable<ShopImageViewModel> shop_image { get; set; }
        public IEnumerable<SoftBranchProductStockViewModel> SoftBranchProductStocks { get; set; }
    }
    public class ShopSanPhamStatusViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CssClass { get; set; }
    }
    public class ShopSanPhamSearchBookViewModel
    {
        public int id { get; set; }
        public string masp { get; set; }
        public string Image { get; set; }
        public string tensp { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> PriceBase { get; set; }
        public Nullable<int> PriceNew { get; set; }
        public Nullable<int> PriceRef { get; set; }
        public Nullable<int> PriceAvg { get; set; }
        public double StockTotal { get; set; }
        public double StockTotalAll { get; set; }
        public Nullable<int> PriceChannel { get; set; }
        public Nullable<double> AvgSoldQuantity { get; set; }
        public Nullable<int> PriceWholesale { get; set; }

        public IEnumerable<SoftBranchProductStockViewModel> SoftBranchProductStocks { get; set; }
        public IEnumerable<SoftChannelProductPriceSearchViewModel> SoftChannelProductPrices { get; set; }
    }
    public class ShopSanPhamSearchBookFilterStockViewModel
    {
        public int id { get; set; }
        public string masp { get; set; }
        public string tensp { get; set; }
        public string Image { get; set; }
        public double StockTotal { get; set; }
        public double StockTotalAll { get; set; }
    }
    public class ShopSanPhamSoldByDateViewModel
    {
        public int id { get; set; }
        public string masp { get; set; }
        public string Image { get; set; }
        public string tensp { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> PriceBase { get; set; }
        public Nullable<int> PriceNew { get; set; }
        public Nullable<int> PriceRef { get; set; }
        public Nullable<int> PriceAvg { get; set; }
        public double StockTotal { get; set; }
        public double StockTotalAll { get; set; }
        public Nullable<int> PriceChannel { get; set; }
        public double Quantity { get; set; }
        public Nullable<double> AvgSoldQuantity { get; set; }

        public IEnumerable<SoftBranchProductStockViewModel> SoftBranchProductStocks { get; set; }
        public IEnumerable<SoftChannelProductPriceSearchViewModel> SoftChannelProductPrices { get; set; }
    }
    public class ShopSanPhamFilterBookViewModel
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int branchId { get; set; }
        public string stringSearch { get; set; }
        public IEnumerable<ShopSanPhamFilterBookDetailViewModel> FilterBookDetail;
    }
    public class ShopSanPhamFilterBookDetailViewModel
    {
        public int key { get; set; }
        public int value { get; set; }
        public string name { get; set; }
        public string aliasName { get; set; }
    }
    public class SoftBranchProductStockViewModel
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> StockTotal { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
    }
    public class ShopSanPhamSearchBookStampViewModel
    {
        public int id { get; set; }
        public string masp { get; set; }
        public string Image { get; set; }
        public string tensp { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> PriceBase { get; set; }
        public Nullable<int> PriceNew { get; set; }
        public Nullable<int> PriceRef { get; set; }
        public Nullable<int> PriceAvg { get; set; }
        public double StockTotal { get; set; }
        public double StockTotalAll { get; set; }
        public Nullable<int> PriceChannel { get; set; }
        public int Quantity { get; set; }

        public IEnumerable<SoftBranchProductStockViewModel> SoftBranchProductStocks { get; set; }
        public IEnumerable<SoftChannelProductPriceSearchViewModel> SoftChannelProductPrices { get; set; }
    }
    public class ShopSanPhamInformation
    {
        public double? kg { get; set; }
        public double? chieudai { get; set; }
        public double? chieurong { get; set; }
        public double? chieucao { get; set; }
        public string masp { get; set; }
        public string tensp { get; set; }
    }

}