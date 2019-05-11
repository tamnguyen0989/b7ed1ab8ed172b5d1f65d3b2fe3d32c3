using AutoMapper;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Mapping
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<SoftSupplier, SoftSupplierViewModel>()
                .ForMember(d => d.Vat, map => map.MapFrom(s => s.SoftSupplierVatStatu.Name));
            Mapper.CreateMap<SoftBranch, SoftBranchViewModel>();
            Mapper.CreateMap<SoftChannel, SoftChannelViewModel>();
            Mapper.CreateMap<SoftBranchProductStock, SoftStockViewModel>()
                .ForMember(d => d.Vat, map => map.MapFrom(s => s.shop_sanpham.SoftSupplier.SoftSupplierVatStatu.Name));
            Mapper.CreateMap<shop_sanpham, ShopSanPhamViewModel>();
            Mapper.CreateMap<shop_image, ShopImageViewModel>()
                .ForMember(d => d.url, map => map.MapFrom(s => "https://babymart.vn/Images/hinhdulieu/thumbnail/" + s.url));
            Mapper.CreateMap<shop_sanphamStatus, ShopSanPhamStatusViewModel>();
            Mapper.CreateMap<SoftChannelProductPrice, SoftChannelProductPriceViewModel>();
            Mapper.CreateMap<shop_sanpham, SoftStockInProductViewModel>()
                .ForMember(d => d.PriceNew, map => map.MapFrom(s => s.PriceBase));
            Mapper.CreateMap<shop_sanpham, SoftOrderDetailViewModel>();
            Mapper.CreateMap<SoftStockIn, SoftStockInViewModel>()
                .ForMember(d => d.SupplierName, map => map.MapFrom(s => s.SoftSupplier.Name))
                .ForMember(d => d.Category, map => map.MapFrom(s => s.SoftStockInCategory.Name))
                .ForMember(d => d.FromBranchStatus, map => map.MapFrom(s => s.SoftStockInStatu1.Name))
                .ForMember(d => d.FromBranchStatusCss, map => map.MapFrom(s => s.SoftStockInStatu1.CssClass))
                .ForMember(d => d.ToBranchStatus, map => map.MapFrom(s => s.SoftStockInStatu2.Name))
                .ForMember(d => d.ToBranchStatusCss, map => map.MapFrom(s => s.SoftStockInStatu2.CssClass))
                .ForMember(d => d.SupplierStatus, map => map.MapFrom(s => s.SoftStockInStatu3.Name))
                .ForMember(d => d.SupplierStatusCss, map => map.MapFrom(s => s.SoftStockInStatu3.CssClass))
                .ForMember(d => d.CreatedByName, map => map.MapFrom(s => s.ApplicationUser.FullName + " (" + s.ApplicationUser.UserName + ")"))
                .ForMember(d => d.FromBranch, map => map.MapFrom(s => s.SoftBranch1.Name))
                .ForMember(d => d.ToBranch, map => map.MapFrom(s => s.SoftBranch2.Name));

            Mapper.CreateMap<SoftStockInStatu, SoftStockInStatusViewModel>();
            Mapper.CreateMap<SoftStockInDetail, SoftStockInDetailViewModel>();
            Mapper.CreateMap<SoftStockInDetail, SoftStockInProductViewModel>()
                .ForMember(d => d.SupplierName, map => map.MapFrom(s => s.shop_sanpham.SoftSupplier.Name))
                .ForMember(d => d.id, map => map.MapFrom(s => s.ProductId))
                .ForMember(d => d.masp, map => map.MapFrom(s => s.shop_sanpham.masp))
                .ForMember(d => d.tensp, map => map.MapFrom(s => s.shop_sanpham.tensp))
                .ForMember(d => d.PriceBase, map => map.MapFrom(s => s.PriceNew))
                .ForMember(d => d.shop_image, map => map.MapFrom(s => s.shop_sanpham.shop_image));
            Mapper.CreateMap<ApplicationRole, ApplicationRoleViewModel>();
            Mapper.CreateMap<ApplicationGroup, ApplicationGroupViewModel>();
            Mapper.CreateMap<ApplicationUser, ApplicationUserViewModel>();
            Mapper.CreateMap<donhang_chuyenphat_tp, donhangchuyenphattpViewModel>();
            Mapper.CreateMap<donhang_chuyenphat_tinh, donhangchuyenphattinhViewModel>();
            Mapper.CreateMap<donhang, donhangListViewModel>()
                .ForMember(d => d.Channel, map => map.MapFrom(s => s.SoftChannel.Name))
                .ForMember(d => d.CustomerName, map => map.MapFrom(s => s.khachhang.hoten))
                .ForMember(d => d.ShipperName, map => map.MapFrom(s => s.ApplicationUser.FullName))
                .ForMember(d => d.ShipperUserName, map => map.MapFrom(s => s.ApplicationUser.UserName))
                .ForMember(d => d.ShipperId, map => map.MapFrom(s => s.ApplicationUser.Id))
                .ForMember(d => d.CreatedName, map => map.MapFrom(s => s.ApplicationUser1.FullName))
                .ForMember(d => d.CreatedUserName, map => map.MapFrom(s => s.ApplicationUser1.UserName))
                .ForMember(d => d.StatusId, map => map.MapFrom(s => s.donhangStatu.Id))
                .ForMember(d => d.StatusCss, map => map.MapFrom(s => s.donhangStatu.CssClass))
                .ForMember(d => d.StatusName, map => map.MapFrom(s => s.donhangStatu.Name));

            Mapper.CreateMap<donhangStatu, donhangStatusViewModel>();
            Mapper.CreateMap<donhang, donhangDetailViewModel>();
            Mapper.CreateMap<khachhang, khachhangViewModel>();
            Mapper.CreateMap<donhang_ct, donhangctViewModel>()
                .ForMember(d => d.masp, map => map.MapFrom(s => s.shop_bienthe.shop_sanpham.masp));
            Mapper.CreateMap<shop_bienthe, shopbientheViewModel>()
                .ForMember(d => d.masp, map => map.MapFrom(s => s.shop_sanpham.masp))
                .ForMember(d => d.tensp, map => map.MapFrom(s => s.shop_sanpham.tensp));
            Mapper.CreateMap<SoftBranchProductStock, SoftStockTotalAllViewModel>()
                .ForMember(d => d.Name, map => map.MapFrom(s => s.SoftBranch.Name));
            Mapper.CreateMap<shop_sanpham, ShopSanPhamSearchBookViewModel>()
                .ForMember(d => d.PriceNew, map => map.ResolveUsing<PriceResolver>().FromMember(x => x.PriceBase))
                .ForMember(d => d.PriceBase, map => map.ResolveUsing<PriceResolver>().FromMember(x => x.PriceBase))
                .ForMember(d => d.PriceRef, map => map.ResolveUsing<PriceResolver>().FromMember(x => x.PriceRef))
                .ForMember(d => d.Image, map => map.MapFrom(s => "https://babymart.vn/Images/hinhdulieu/thumbnail/" + s.shop_image.FirstOrDefault().url))
                .ForMember(d => d.SupplierName, map => map.MapFrom(s => s.SoftSupplier.Name));
            Mapper.CreateMap<shop_sanpham, ShopSanPhamSoldByDateViewModel>()
                .ForMember(d => d.PriceNew, map => map.ResolveUsing<PriceResolver>().FromMember(x => x.PriceBase))
                .ForMember(d => d.PriceBase, map => map.ResolveUsing<PriceResolver>().FromMember(x => x.PriceBase))
                .ForMember(d => d.PriceRef, map => map.ResolveUsing<PriceResolver>().FromMember(x => x.PriceRef))
                .ForMember(d => d.Image, map => map.MapFrom(s => "https://babymart.vn/Images/hinhdulieu/thumbnail/" + s.shop_image.FirstOrDefault().url))
                .ForMember(d => d.SupplierName, map => map.MapFrom(s => s.SoftSupplier.Name));
            Mapper.CreateMap<SoftStockIn, SoftStockInSearchViewModel>()
                .ForMember(d => d.SoftStockInPaymentMethod, map => map.MapFrom(s => s.SoftStockInPaymentMethod))
                .ForMember(d => d.CreatedByName, map => map.MapFrom(s => s.ApplicationUser.UserName))
                .ForMember(d => d.Category, map => map.MapFrom(s => s.SoftStockInCategory.Name))
                .ForMember(d => d.StatusName, map => map.MapFrom(s => s.SoftStockInStatu.Name))
                .ForMember(d => d.StatusCss, map => map.MapFrom(s => s.SoftStockInStatu.CssClass))
                .ForMember(d => d.FromBranch, map => map.MapFrom(s => s.SoftBranch1.Name))
                .ForMember(d => d.ToBranch, map => map.MapFrom(s => s.SoftBranch2.Name))
                .ForMember(d => d.FromBranchStatus, map => map.MapFrom(s => s.SoftStockInStatu1.Name))
                .ForMember(d => d.FromBranchStatusCss, map => map.MapFrom(s => s.SoftStockInStatu1.CssClass))
                .ForMember(d => d.ToBranchStatus, map => map.MapFrom(s => s.SoftStockInStatu2.Name))
                .ForMember(d => d.ToBranchStatusCss, map => map.MapFrom(s => s.SoftStockInStatu2.CssClass))
                .ForMember(d => d.SupplierStatus, map => map.MapFrom(s => s.SoftStockInStatu3.Name))
                .ForMember(d => d.SupplierStatusCss, map => map.MapFrom(s => s.SoftStockInStatu3.CssClass))
                .ForMember(d => d.PaymentStatus, map => map.MapFrom(s => s.SoftStockInPaymentStatus.Name))
                .ForMember(d => d.SupplierName, map => map.MapFrom(s => s.SoftSupplier.Name));
            Mapper.CreateMap<SoftBranchProductStock, SoftBranchProductStockViewModel>();
            Mapper.CreateMap<SoftAdjustmentStock, SoftAdjustmentStockViewModel>();
            Mapper.CreateMap<SoftAdjustmentStockDetail, SoftAdjustmentStockDetailViewModel>();
            Mapper.CreateMap<SoftNotification, SoftNotificationViewModel>();
            Mapper.CreateMap<SoftChannelProductPrice, SoftChannelProductPriceSearchViewModel>();
            Mapper.CreateMap<ApplicationRoleCategory, ApplicationRoleCategoryViewModel>();
            Mapper.CreateMap<donhang_chuyenphat_vung, donhangchuyenphatvungViewModel>();
            Mapper.CreateMap<donhang_chuyenphat_danhsachdiachifuta, donhangchuyenphatdiachifutaViewModel>();
            Mapper.CreateMap<SoftBranchProductStock, SoftStockViewModelExcel>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.shop_sanpham.id))
                .ForMember(d => d.Name, map => map.MapFrom(s => s.shop_sanpham.tensp))
                .ForMember(d => d.Supplier, map => map.MapFrom(s => s.shop_sanpham.SoftSupplier.Name))
                .ForMember(d => d.Stock, map => map.MapFrom(s => s.StockTotal))
                .ForMember(d => d.PriceOld, map => map.MapFrom(s => s.shop_sanpham.PriceBaseOld))
                .ForMember(d => d.PriceNew, map => map.MapFrom(s => s.shop_sanpham.PriceBase))
                .ForMember(d => d.Url, map => map.ResolveUsing<UrlResolver>().FromMember(x => x.shop_sanpham.spurl))
                .ForMember(d => d.Code, map => map.MapFrom(s => s.shop_sanpham.masp));
            Mapper.CreateMap<SoftStockViewModelExcel, SoftStockViewModelExcelNoId>();
            Mapper.CreateMap<SoftBranch, SoftBranchImportSampleViewModel>();
            Mapper.CreateMap<SoftSupplierVatStatu, SoftSupplierVatStatuViewModel>();
            Mapper.CreateMap<shop_sanphamCategories, ShopSanPhamCategoryViewModel>();
            Mapper.CreateMap<SoftStockInCategory, SoftStockInCategoryViewModel>();
            Mapper.CreateMap<shop_sanpham, ShopSanPhamSearchBookStampViewModel>();
            Mapper.CreateMap<SoftStockInDetail, BookExcelViewModel>()
                .ForMember(d => d.MaSanPham, map => map.MapFrom(s => s.shop_sanpham.masp))
                .ForMember(d => d.SoLuong, map => map.MapFrom(s => s.Quantity))
                .ForMember(d => d.TenSanPham, map => map.MapFrom(s => s.shop_sanpham.tensp));
            Mapper.CreateMap<shop_sanphamLogs, ShopSanPhamLogViewModel>();
            Mapper.CreateMap<SoftReturnSupplier, SoftReturnSupplierViewModel>();
            Mapper.CreateMap<SoftReturnSupplierDetail, SoftReturnSupplierDetailViewModel>()
                .ForMember(d => d.ProductName, map => map.MapFrom(s => s.shop_sanpham.tensp));
            Mapper.CreateMap<SoftBranch, BranchProductStockOutputViewModel>();
            Mapper.CreateMap<shop_sanpham, DetailProductOutputViewModel>()
                .ForMember(d => d.productCode, map => map.MapFrom(s => s.masp))
                .ForMember(d => d.name, map => map.MapFrom(s => s.tensp))
                .ForMember(d => d.productCategory, map => map.MapFrom(s => s.shop_sanphamCategories))
                .ForMember(d => d.supplier, map => map.MapFrom(s => s.SoftSupplier))
                .ForMember(d => d.productStatus, map => map.MapFrom(s => s.shop_sanphamStatus));
            Mapper.CreateMap<SoftStockInPaymentType, SoftStockInPaymentTypeViewModel>();
            Mapper.CreateMap<SoftStockInPaymentStatus, SoftStockInPaymentStatusViewModel>();
            Mapper.CreateMap<SoftStockInPaymentMethod, SoftStockInPaymentMethodViewModel>();
            Mapper.CreateMap<ExportPriceParamsDetail, ExportPriceViewModel>();
            Mapper.CreateMap<ExportPriceViewModel, ExportPriceNoIdViewModel>();
            Mapper.CreateMap<SoftChannelProductPrice, PriceChannelViewModel>()
                .ForMember(d => d.ChannelId, map => map.MapFrom(s => s.ChannelId))
                .ForMember(d => d.Code, map => map.MapFrom(s => s.SoftChannel.Code))
                .ForMember(d => d.Name, map => map.MapFrom(s => s.SoftChannel.Name));
            Mapper.CreateMap<SoftBranchProductStock, ShopSanPhamSearchBookFilterStockViewModel>()
                .ForMember(d => d.id, map => map.MapFrom(s => s.shop_sanpham.id))
                .ForMember(d => d.masp, map => map.MapFrom(s => s.shop_sanpham.masp))
                .ForMember(d => d.Image, map => map.MapFrom(s => "https://babymart.vn/Images/hinhdulieu/thumbnail/" + s.shop_sanpham.shop_image.FirstOrDefault().url))
                .ForMember(d => d.tensp, map => map.MapFrom(s => s.shop_sanpham.tensp));
            Mapper.CreateMap<SoftBranchProductStock, ExportPriceViewModel>()
                .ForMember(d => d.id, map => map.MapFrom(s => s.shop_sanpham.id))
                .ForMember(d => d.Code, map => map.MapFrom(s => s.shop_sanpham.masp))
                .ForMember(d => d.PriceAvg, map => map.MapFrom(s => s.shop_sanpham.PriceAvg))
                .ForMember(d => d.PriceBase, map => map.MapFrom(s => s.shop_sanpham.PriceBase))
                .ForMember(d => d.PriceBaseOld, map => map.MapFrom(s => s.shop_sanpham.PriceBaseOld))
                .ForMember(d => d.PriceWholesale, map => map.MapFrom(s => s.shop_sanpham.PriceWholesale))
                .ForMember(d => d.Name, map => map.MapFrom(s => s.shop_sanpham.tensp));
            Mapper.CreateMap<donhang, donhangExcel>()
                .ForMember(d => d.StatusName, map => map.MapFrom(s => s.donhangStatu.Name));
            Mapper.CreateMap<donhangExcel, donhangExcelNoId>();
        }

        public class PriceResolver : ValueResolver<int?, int>
        {
            protected override int ResolveCore(int? source)
            {
                if (source != null)
                    return source.Value;
                else
                    return 0;
            }
        }

        public class UrlResolver : ValueResolver<string, string>
        {
            protected override string ResolveCore(string source)
            {
                if (!string.IsNullOrEmpty(source))
                    return "https://babymart.vn/tin-tuc/" + source + ".html";
                else
                    return null;
            }
        }
    }
}