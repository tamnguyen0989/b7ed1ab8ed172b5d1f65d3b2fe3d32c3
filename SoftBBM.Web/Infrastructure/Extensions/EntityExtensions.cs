using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftBBM.Web.Enum;

namespace SoftBBM.Web.Infrastructure.Extensions
{
    public static class EntityExtensions
    {
        public static void UpdateSoftSupplier(this SoftSupplier softSupplier, SoftSupplierViewModel softSupplierVm)
        {
            softSupplier.Id = softSupplierVm.Id;
            softSupplier.Name = softSupplierVm.Name;
            softSupplier.Address = softSupplierVm.Address;
            softSupplier.Phone = softSupplierVm.Phone;
            softSupplier.Email = softSupplierVm.Email;
            softSupplier.VatId = softSupplierVm.VatId;
            softSupplier.AccBank = softSupplierVm.AccBank;
            softSupplier.Status = softSupplierVm.Status;
        }
        public static void UpdateSoftBranch(this SoftBranch softBranch, SoftBranchViewModel softBranchVm)
        {
            softBranch.Id = softBranchVm.Id;
            softBranch.Name = softBranchVm.Name;
            softBranch.Code = softBranchVm.Code;
            softBranch.Address = softBranchVm.Address;
            softBranch.Phone = softBranchVm.Phone;
            softBranch.Description = softBranchVm.Description;
            softBranch.Status = softBranchVm.Status;
        }
        public static void UpdateSoftChannel(this SoftChannel softChannel, SoftChannelViewModel softChannelVm)
        {
            softChannel.Id = softChannelVm.Id;
            softChannel.Name = softChannelVm.Name;
            softChannel.Description = softChannelVm.Description;
            softChannel.Status = softChannelVm.Status;
            softChannel.Code = softChannelVm.Code;
            softChannel.Enabled = softChannelVm.Enabled;
        }
        public static void UpdateSoftStock(this SoftBranchProductStock softBranchProductStock, SoftStockViewModel SoftStockVm)
        {
            softBranchProductStock.StockTotal = SoftStockVm.Stock_Total;
            softBranchProductStock.shop_sanpham.PriceBase = SoftStockVm.shop_sanpham.PriceBase;
            softBranchProductStock.shop_sanpham.PriceBaseOld = SoftStockVm.shop_sanpham.PriceBaseOld;
            softBranchProductStock.shop_sanpham.PriceAvg = SoftStockVm.shop_sanpham.PriceAvg;
            softBranchProductStock.shop_sanpham.PriceRef = SoftStockVm.shop_sanpham.PriceRef;
            softBranchProductStock.shop_sanpham.Barcode = SoftStockVm.shop_sanpham.Barcode;
            if (SoftStockVm.shop_sanpham.SoftSupplier != null)
                softBranchProductStock.shop_sanpham.SupplierId = SoftStockVm.shop_sanpham.SoftSupplier.Id;
            if (SoftStockVm.shop_sanpham.shop_sanphamStatus != null)
                softBranchProductStock.shop_sanpham.StatusId = SoftStockVm.shop_sanpham.shop_sanphamStatus.Id;
        }
        public static void UpdateShopSanPham(this shop_sanpham shopSanPham, ShopSanPhamViewModel softStockShopSanPhamVm)
        {
            shopSanPham.SupplierId = softStockShopSanPhamVm.SoftSupplier.Id;
            shopSanPham.StatusId = softStockShopSanPhamVm.shop_sanphamStatus.Id;
        }
        public static void UpdateNewShopSanPham(this shop_sanpham shopSanPham, ShopSanPhamViewModel shopSanPhamVm)
        {
            shopSanPham.masp = shopSanPhamVm.masp.Trim();
            shopSanPham.tensp = shopSanPhamVm.tensp.Trim();
            shopSanPham.FromCreate = 2;
            shopSanPham.PriceAvg = 0;
            shopSanPham.PriceBase = 0;
            shopSanPham.PriceBaseOld = 0;
            shopSanPham.PriceRef = 0;
            shopSanPham.CreatedBy = shopSanPhamVm.userId;
            shopSanPham.CreatedDate = DateTime.Now;
            if (shopSanPhamVm.CategoryId > 0)
                shopSanPham.CategoryId = shopSanPhamVm.CategoryId;
        }
        public static void UpdateSoftChannelProductPrice(this SoftChannelProductPrice softChannelProductPrice, SoftChannelProductPriceViewModel softChannelProductPriceViewModelVm)
        {
            softChannelProductPrice.Price = softChannelProductPriceViewModelVm.Price;
            softChannelProductPrice.PriceDiscount = softChannelProductPriceViewModelVm.PriceDiscount;
            if (softChannelProductPriceViewModelVm.StartDateDiscount != null)
                softChannelProductPrice.StartDateDiscount = ((DateTime)softChannelProductPriceViewModelVm.StartDateDiscount).ToLocalTime();
            else
                softChannelProductPrice.StartDateDiscount = null;
            if (softChannelProductPriceViewModelVm.EndDateDiscount != null)
                softChannelProductPrice.EndDateDiscount = ((DateTime)softChannelProductPriceViewModelVm.EndDateDiscount).ToLocalTime();
            else
                softChannelProductPrice.EndDateDiscount = null;
            softChannelProductPrice.Description = softChannelProductPriceViewModelVm.Description;
        }
        public static void UpdateSoftStockIn(this SoftStockIn softStockIn, SoftStockInViewModel SoftStockInVm)
        {
            softStockIn.BranchId = SoftStockInVm.BranchId;
            softStockIn.CategoryId = SoftStockInVm.CategoryId;
            softStockIn.Description = SoftStockInVm.Description;
            softStockIn.CreatedBy = SoftStockInVm.CreatedBy;
            softStockIn.Total = SoftStockInVm.Total;
            softStockIn.TotalQuantity = SoftStockInVm.TotalQuantity;
            if (SoftStockInVm.CategoryId == "00")
            {
                softStockIn.SupplierId = SoftStockInVm.SupplierId;
                softStockIn.SupplierStatusId = "03";
                softStockIn.ToBranchId = SoftStockInVm.BranchId;
                softStockIn.ToBranchStatusId = "03";
                //softStockIn.PaymentStatusId = 2;

            }
            if (SoftStockInVm.CategoryId == "01")
            {
                softStockIn.SupplierId = SoftStockInVm.SupplierId;
                softStockIn.SupplierStatusId = "03";
                softStockIn.FromBranchId = SoftStockInVm.FromBranchId;
                softStockIn.FromBranchStatusId = "03";
                softStockIn.ToBranchId = SoftStockInVm.ToBranchId;
                softStockIn.ToBranchStatusId = "03";
            }
            if (SoftStockInVm.CategoryId == "03")
            {
                softStockIn.FromBranchId = SoftStockInVm.FromBranchId;
                softStockIn.FromBranchStatusId = "01";
                softStockIn.ToBranchId = SoftStockInVm.ToBranchId;
                softStockIn.ToBranchStatusId = "03";
            }
            if (SoftStockInVm.CategoryId == "02")
            {
                softStockIn.ToBranchId = SoftStockInVm.ToBranchId;
                softStockIn.ToBranchStatusId = "01";

            }
        }
        public static void UpdateSoftStockInDetail(this SoftStockInDetail softStockInDetail, SoftStockInProductViewModel softStockInProductVm, string type = "")
        {
            softStockInDetail.ProductId = softStockInProductVm.id;
            softStockInDetail.Quantity = softStockInProductVm.Quantity;
            softStockInDetail.PriceNew = softStockInProductVm.PriceNew;
            switch (type)
            {
                case "01":
                    softStockInDetail.PriceNew = softStockInProductVm.PriceBase;
                    break;
                case "02":
                    softStockInDetail.PriceRef = softStockInProductVm.PriceRef;
                    break;
            }

        }
        public static void UpdateApplicationRole(this ApplicationRole appRole, ApplicationRoleViewModel appRoleViewModel, string action = "add")
        {
            appRole.Name = appRoleViewModel.Name;
            appRole.Description = appRoleViewModel.Description;
            appRole.CategoryId = appRoleViewModel.CategoryId;
            //appRole.Discriminator = "ApplicationRole";
        }
        public static void UpdateApplicationGroup(this ApplicationGroup applicationGroup, ApplicationGroupViewModel appVm)
        {
            applicationGroup.Name = appVm.Name;
            applicationGroup.Description = appVm.Description;
        }
        public static void UpdateUser(this ApplicationUser appUser, ApplicationUserViewModel appUserViewModel, string action = "add")
        {
            appUser.FullName = appUserViewModel.FullName;
            appUser.BirthDay = appUserViewModel.BirthDay;
            appUser.Email = appUserViewModel.Email;
            appUser.UserName = appUserViewModel.UserName;
            appUser.PhoneNumber = appUserViewModel.PhoneNumber;
            appUser.Password = appUserViewModel.Password.Base64Encode();
            appUser.Status = appUserViewModel.Status;
            //appUser.EmailConfirmed = true;
        }
        public static void Updatedonhang(this donhang donhang, OrderViewModel orderVM)
        {
            donhang.Code = orderVM.Code;
            donhang.BranchId = orderVM.BranchId;
            donhang.ChannelId = orderVM.ChannelId;
            donhang.CreatedBy = orderVM.CreatedBy;
            donhang.CreatedDate = DateTime.Now;
            donhang.diemsp = orderVM.diemsp;
            donhang.ghichu = orderVM.ghichu;
            donhang.makh = orderVM.makh;
            donhang.ngaydat = DateTime.Now;
            donhang.ShipperId = orderVM.ShipperId;
            donhang.tongtien = orderVM.tongtien;
            donhang.datru_diem = orderVM.datru_diem;
            donhang.Status = orderVM.Status;
            donhang.Discount = orderVM.Discount;
            donhang.DiscountMoney = orderVM.DiscountMoney;
            donhang.DiscountPercent = orderVM.DiscountPercent;
            donhang.DiscountCode = orderVM.DiscountCode;
            donhang.ship = orderVM.ship;
            donhang.phithuho = orderVM.phithuho;
            donhang.pttt = orderVM.pttt;
            donhang.ptgh = orderVM.ptgh;
            donhang.idgiogiao = orderVM.idgiogiao;
            donhang.tenptgh = orderVM.tenptgh;
        }
        public static void UpdatedonhangFromShopee(this donhang donhang, int? makh, long? total_amount, string ghichu,string shipperName,int? status,string orderIdShopeeApi)
        {
            donhang.makh = makh;
            donhang.CreatedDate = DateTime.Now;
            donhang.CreatedBy = 0;
            donhang.tongtien = total_amount;
            donhang.ShipperNameShopeeApi = shipperName;
            donhang.ghichu = ghichu;
            donhang.Status = status;
            donhang.IsShopeeApi = true;
            donhang.OrderIdShopeeApi = orderIdShopeeApi;
            donhang.ChannelId = (int) ChannelEnum.SPE;
            donhang.BranchId = (int)BranchEnum.KHO_CHINH;
        }
        public static void UpdatedonhangLackFromShopee(this donhang donhang, int? makh, long? total_amount, string ghichu, string shipperName, int? status, string orderIdShopeeApi, DateTime createdDate, DateTime? updatedDate)
        {
            donhang.makh = makh;
            donhang.CreatedDate = createdDate;
            donhang.CreatedBy = 0;
            donhang.tongtien = total_amount;
            donhang.ShipperNameShopeeApi = shipperName;
            donhang.ghichu = ghichu;
            donhang.Status = status;
            donhang.IsShopeeApi = true;
            donhang.OrderIdShopeeApi = orderIdShopeeApi;
            donhang.ChannelId = (int)ChannelEnum.SPE;
            donhang.BranchId = (int)BranchEnum.KHO_CHINH;
            donhang.UpdatedDate = updatedDate;
        }
        public static void Updatedonhangct(this donhang_ct donhang_ct, OrderDetailViewModel orderDetailVM)
        {
            donhang_ct.IdPro = orderDetailVM.id;
            donhang_ct.Soluong = orderDetailVM.Quantity.Value;
            donhang_ct.Dongia = orderDetailVM.Price;
        }
        public static void UpdatedonhangctFromShopee(this donhang_ct donhang_ct, long soDh,long? idPro,long soLuong,int? donGia, int? donGiaKM,string description)
        {
            donhang_ct.Sodh = soDh;
            donhang_ct.IdPro = idPro;
            donhang_ct.Soluong = soLuong;
            donhang_ct.Dongia = donGia;
            donhang_ct.Dongiakm = donGiaKM;
            donhang_ct.Description = description;
        }
        public static void Updatekhachhang(this khachhang khachhang, khachhangViewModel khachhangVM, string action = "add")
        {
            if (action == "update")
            {
                khachhang.diem = khachhangVM.diem.ToString();
            }
            else
            {
                khachhang.diem = 0.ToString();
                khachhang.email = khachhangVM.dienthoai + "@gmail.com";
                khachhang.tendn = khachhangVM.dienthoai;
                khachhang.matkhau = "YmFieW1hcnQudm4=";
            }
            khachhang.dienthoai = khachhangVM.dienthoai;
            if (khachhangVM.duong == null)
                khachhang.duong = khachhangVM.dienthoai;
            else
                khachhang.duong = khachhangVM.duong;
            if (khachhangVM.hoten == null)
                khachhang.hoten = khachhangVM.dienthoai;
            else
                khachhang.hoten = khachhangVM.hoten;
            khachhang.idtp = khachhangVM.idtp;
            khachhang.idquan = khachhangVM.idquan;
            khachhang.ngaydangky = DateTime.Now;

        }
        public static void UpdateAdjustmentStockAdd(this SoftAdjustmentStock model, SoftAdjustmentStockAddViewModel vM)
        {
            model.BranchId = vM.BranchId;
            model.Description = vM.Description;
            model.CreatedBy = vM.CreatedBy;
            model.CreatedDate = DateTime.Now;
        }
        public static void UpdateAdjustmentStockDetailAdd(this SoftAdjustmentStockDetail model, SoftAdjustmentStockDetailAddViewModel vM)
        {
            model.ProductId = vM.id;
            model.Quantity = vM.Quantity;
        }
        public static void UpdatekhachhangV2(this khachhang khachhang, string full_address, string phone, string name, int? idtp, int? idquan)
        {
            khachhang.duong = full_address;
            khachhang.dienthoai = phone;
            khachhang.hoten = name;
            khachhang.tendn = phone;
            khachhang.matkhau = "YmFieW1hcnQudm4=";
            khachhang.email = phone + "@gmail.com";
            khachhang.idtp = idtp;
            khachhang.idquan = idquan;
            khachhang.CreatedDate = DateTime.Now;
            khachhang.CreatedBy = 0;
            khachhang.diem = "0";
        }
        public static void UpdateShopSanPhamLog(this shop_sanphamLogs shopSanPhamLog,int? productId,string description,double? quantity,int? createdBy,int? branchId,double stockTotal)
        {
            shopSanPhamLog.ProductId = productId;
            shopSanPhamLog.Description = description;
            shopSanPhamLog.Quantity = quantity;
            shopSanPhamLog.CreatedBy = createdBy;
            shopSanPhamLog.CreatedDate = DateTime.Now;
            shopSanPhamLog.BranchId = branchId;
            shopSanPhamLog.StockTotal = stockTotal;
        }
        public static void InitSystemLog(this SystemLog systemLog, int? keyId = null,string value="", string appName = "", string description = "",int? type=null, string typeName = "", string before = "", string after = "")
        {
            systemLog.KeyId = keyId;
            systemLog.Value = value;
            systemLog.AppName = appName;
            systemLog.Description = description;
            systemLog.Type = type;
            systemLog.TypeName = typeName;
            systemLog.Before = before;
            systemLog.After = after;
        }
    }
}