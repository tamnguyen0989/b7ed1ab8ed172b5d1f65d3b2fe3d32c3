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
        public bool? hide { get; set; }

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
    public class ShopSanPhamInputAddVM
    {
        public ShopSanPhamCategoryViewModel selectedProductCategory { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public SoftSupplierViewModel selectedSupplier { get; set; }
        public ShopSanPhamStatusViewModel selectedProductStatus { get; set; }
        public string barCode { get; set; }
        public int priceRef { get; set; }
        public int priceBase { get; set; }
        //public int priceCHA { get; set; }
        //public int priceONL { get; set; }
        public int userId { get; set; }
        public List<SoftChannelProductPriceViewModel> SoftChannelProductPrices { get; set; }
    }
    public class ShopSanPhamMaxCodeProductVM
    {
        public string masp { get; set; }
        public int len { get; set; }
    }
    public class ShopSanPhamNoRef
    {
        public int id { get; set; }
        public Nullable<int> StoreId { get; set; }
        public string masp { get; set; }
        public Nullable<int> maloai { get; set; }
        public Nullable<int> mahieu { get; set; }
        public Nullable<int> plantype { get; set; }
        public string tensp { get; set; }
        public string tensp_us { get; set; }
        public Nullable<int> typeshowhome { get; set; }
        public string thongtin { get; set; }
        public string thongtin_us { get; set; }
        public string spurl { get; set; }
        public string spurl_us { get; set; }
        public string sptitle { get; set; }
        public string sptitle_us { get; set; }
        public string spdescription { get; set; }
        public string spdescription_us { get; set; }
        public string spkeywords_us { get; set; }
        public string spkeywords { get; set; }
        public Nullable<bool> hide { get; set; }
        public string chitiet { get; set; }
        public string chitiet_us { get; set; }
        public Nullable<int> douutien { get; set; }
        public Nullable<bool> ischeckout { get; set; }
        public Nullable<bool> ischecksaleoff { get; set; }
        public Nullable<bool> ischeckgift { get; set; }
        public string gift { get; set; }
        public Nullable<int> countsale { get; set; }
        public Nullable<System.DateTime> timeend { get; set; }
        public Nullable<double> kg { get; set; }
        public Nullable<double> chieudai { get; set; }
        public Nullable<double> chieurong { get; set; }
        public Nullable<double> chieucao { get; set; }
        public Nullable<double> chieudaisd { get; set; }
        public Nullable<double> chieurongsd { get; set; }
        public Nullable<double> chieucaosd { get; set; }
        public Nullable<bool> showkg { get; set; }
        public Nullable<bool> showcm { get; set; }
        public Nullable<bool> showhome { get; set; }
        public Nullable<bool> showbanner { get; set; }
        public Nullable<bool> showsptangkemvaomuckm { get; set; }
        public string Barcode { get; set; }
        public Nullable<bool> StopSale { get; set; }
        public Nullable<int> PriceBase { get; set; }
        public Nullable<int> PriceBaseOld { get; set; }
        public Nullable<int> PriceAvg { get; set; }
        public Nullable<int> PriceWholesale { get; set; }
        public Nullable<int> PriceRef { get; set; }
        public Nullable<bool> freeship { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string StatusId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> NotDiscountMember { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> FromCreate { get; set; }
        public Nullable<int> ShopeeId { get; set; }
        public Nullable<int> CodeSuffix { get; set; }
    }

}