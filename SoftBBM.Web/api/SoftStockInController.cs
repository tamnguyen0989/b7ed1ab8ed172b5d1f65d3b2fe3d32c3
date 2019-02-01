using AutoMapper;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using SoftBBM.Web.Common;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Infrastructure.Core;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

//filter = StringHelper.ToUnsignString(filter.Trim().ToLower());

//search input unsign string with unicode string
//var list = _softStockInRepository.GetMulti(c => c.BranchId == branchId && c.CategoryId == categoryId).ToList();
//stockIns = list.FindAll(delegate (SoftStockIn math)
//{

//    if (((StringHelper.ToUnsignString(math.SoftSupplier.Name.ToLower())).Contains(filter)) || math.Id.ToString() == filter)
//        return true;
//    else
//        return false;

//});
//totalStockIns = stockIns.Count;
//stockIns = stockIns.OrderByDescending(c => c.Id)
//    .Skip(currentPage * currentPageSize)
//    .Take(currentPageSize)
//    .ToList();

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/stockin")]
    public class SoftStockInController : ApiController
    {
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftStockRepository _softStockRepository;
        ISoftStockInRepository _softStockInRepository;
        ISoftStockInDetailRepository _softStockInDetailRepository;
        ISoftStockInStatusRepository _softStockInStatusRepository;
        ISoftNotificationRepository _softNotificationRepository;
        ISoftBranchRepository _softBranchRepository;
        IdonhangRepository _donhangRepository;
        IshopbientheRepository _shopbientheRepository;
        ISoftSupplierRepository _softSupplierRepository;
        ISoftStockInCategoryRepository _softStockInCategoryRepository;
        IApplicationUserRepository _applicationUserRepository;
        IShopSanPhamLogRepository _shopSanPhamLogRepository;
        IdonhangctRepository _donhangctRepository;
        ISoftStockInPaymentStatusRepository _softStockInPaymentStatusRepository;
        ISoftStockInPaymentMethodRepository _softStockInPaymentMethodRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        IUnitOfWork _unitOfWork;

        public SoftStockInController(ISoftStockInRepository softStockInRepository, ISoftStockInDetailRepository softStockInDetailRepository, IUnitOfWork unitOfWork, IShopSanPhamRepository shopSanPhamRepository, ISoftStockRepository softStockRepository, ISoftStockInStatusRepository softStockInStatusRepository, ISoftNotificationRepository softNotificationRepository, ISoftBranchRepository softBranchRepository, IdonhangRepository donhangRepository, IshopbientheRepository shopbientheRepository, ISoftSupplierRepository softSupplierRepository, ISoftStockInCategoryRepository softStockInCategoryRepository, IApplicationUserRepository applicationUserRepository, IShopSanPhamLogRepository shopSanPhamLogRepository, IdonhangctRepository donhangctRepository, ISoftStockInPaymentStatusRepository softStockInPaymentStatusRepository, ISoftStockInPaymentMethodRepository softStockInPaymentMethodRepository, ISoftChannelProductPriceRepository softChannelProductPriceRepository)
        {
            _softStockInRepository = softStockInRepository;
            _softStockRepository = softStockRepository;
            _softStockInDetailRepository = softStockInDetailRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _softStockInStatusRepository = softStockInStatusRepository;
            _softNotificationRepository = softNotificationRepository;
            _softBranchRepository = softBranchRepository;
            _donhangRepository = donhangRepository;
            _shopbientheRepository = shopbientheRepository;
            _softStockInCategoryRepository = softStockInCategoryRepository;
            _softSupplierRepository = softSupplierRepository;
            _applicationUserRepository = applicationUserRepository;
            _shopSanPhamLogRepository = shopSanPhamLogRepository;
            _donhangctRepository = donhangctRepository;
            _softStockInPaymentStatusRepository = softStockInPaymentStatusRepository;
            _softStockInPaymentMethodRepository = softStockInPaymentMethodRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _unitOfWork = unitOfWork;
        }

        [Route("getsoftstockinproduct")]
        [HttpGet]
        public HttpResponseMessage GetSoftStockInProduct(HttpRequestMessage request, int productId)
        {
            HttpResponseMessage response = null;
            try
            {
                var SoftStockProduct = _shopSanPhamRepository.GetSingleById(productId);
                var SoftStockProductVm = Mapper.Map<shop_sanpham, SoftStockInProductViewModel>(SoftStockProduct);
                response = request.CreateResponse(HttpStatusCode.OK, SoftStockProductVm);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("addstockout")]
        [Authorize(Roles = "StockOutAdd")]
        [HttpPost]
        public HttpResponseMessage AddStockOut(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    double stockTmp = 0;
                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockInVm.FromBranchId && x.ProductId == item.id);
                    if (productInBranch != null)
                        stockTmp = productInBranch.StockTotal.Value;
                    if (stockTmp < item.Quantity)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Tồn tại kho của " + item.masp.Trim() + " không đủ xuất!");
                        return response;
                    }
                    if (item.Quantity == null)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Số lượng mã " + item.masp.Trim() + " lỗi!");
                        return response;
                    }
                }
                SoftStockIn softStockIn = new SoftStockIn();
                softStockIn.UpdateSoftStockIn(softStockInVm);
                softStockIn.FromBranchStatusId = "06";
                softStockIn.CreatedDate = DateTime.Now;
                softStockIn.StockOutDate = DateTime.Now;
                _softStockInRepository.Add(softStockIn);
                _unitOfWork.Commit();
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    softStockInDetail.UpdateSoftStockInDetail(item);
                    softStockInDetail.StockInId = softStockIn.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);
                    //Update Product, Stock

                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockIn.FromBranchId.Value && x.ProductId == item.id);
                    if (productInBranch == null)
                    {
                        var newStockCurrent = _softStockRepository.Init(softStockIn.FromBranchId.Value, item.id);
                        productInBranch = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    productInBranch.StockTotal -= item.Quantity;
                    productInBranch.UpdatedDate = DateTime.Now;
                    _softStockRepository.Update(productInBranch);

                    //Add log
                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.id;
                    productLog.Description = "Xuất kho, mã đơn: " + softStockIn.Id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = softStockInVm.CreatedBy;
                    productLog.CreatedDate = DateTime.Now;
                    productLog.BranchId = softStockInVm.FromBranchId;
                    productLog.StockTotal = productInBranch.StockTotal.Value;
                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(item.id) - item.Quantity.Value;
                    _shopSanPhamLogRepository.Add(productLog);
                }

                var newSoftNotification = new SoftNotification();
                newSoftNotification.StockinId = softStockIn.Id;
                newSoftNotification.Message = "Đơn xuất kho " + softStockIn.Id + " cần được xử lý";
                newSoftNotification.Url = "stockins";
                newSoftNotification.FromBranchId = softStockInVm.FromBranchId;
                newSoftNotification.ToBranchId = softStockInVm.ToBranchId;
                newSoftNotification.IsRead = false;
                newSoftNotification.Status = false;
                newSoftNotification.CreatedDate = DateTime.Now;
                _softNotificationRepository.Add(newSoftNotification);

                _unitOfWork.Commit();

                var nwSoftStockIn = _softStockInRepository.GetSingleById(softStockIn.Id);
                var responseVM = Mapper.Map<SoftStockIn, SoftStockInViewModel>(nwSoftStockIn);
                foreach (var item in responseVM.SoftStockInDetails)
                {
                    var crProduct = _shopSanPhamRepository.GetSingleById(item.id);
                    item.masp = crProduct.masp;
                    item.tensp = crProduct.tensp;
                }
                responseVM.FromBranch = _softBranchRepository.GetSingleById(softStockIn.FromBranchId.Value).Name;
                responseVM.ToBranch = _softBranchRepository.GetSingleById(softStockIn.ToBranchId.Value).Name;
                response = request.CreateResponse(HttpStatusCode.OK, responseVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("addstockin")]
        [Authorize(Roles = "StockInAdd")]
        [HttpPost]
        public HttpResponseMessage AddStockIn(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            foreach (var item in softStockInVm.SoftStockInDetails)
            {
                if (item.Quantity == null)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Số lượng Mã SP " + item.masp.Trim() + " lỗi!");
                    return response;
                }
            }
            try
            {
                bool stockoutAble = false;
                bool updatedPrice = false;
                var fromSuppliers = "";
                SoftStockIn softStockIn = new SoftStockIn();
                softStockIn.UpdateSoftStockIn(softStockInVm);
                softStockIn.ToBranchStatusId = "07";
                softStockIn.CreatedDate = DateTime.Now;
                softStockIn.StockInDate = DateTime.Now;
                softStockIn.PaymentMethodId = softStockInVm.PaymentMethodId;
                softStockIn.PaymentStatusId = softStockInVm.PaymentStatusId;
                softStockIn.CreatedBy = softStockInVm.CreatedBy;
                if (softStockInVm.PaymentStatusId == 1)
                    softStockIn.PaidDate = DateTime.Now;
                _softStockInRepository.Add(softStockIn);
                _unitOfWork.Commit();
                var branch = _softBranchRepository.GetSingleById(softStockIn.BranchId.Value);
                var branches = _softBranchRepository.GetAllIds().ToList();
                branches.Remove(2);
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    softStockInDetail.UpdateSoftStockInDetail(item, "02");
                    softStockInDetail.StockInId = softStockIn.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);

                    var ctProduct = _shopSanPhamRepository.GetSingleById(item.id);
                    softStockInDetail.UpdateSoftStockInDetail(item);
                    if (ctProduct.SupplierId.HasValue && !fromSuppliers.Contains(ctProduct.SoftSupplier.Name))
                    {
                        fromSuppliers += ctProduct.SoftSupplier.Name + "|";
                    }

                    //Update Product, Stock

                    var shopSanPham = _shopSanPhamRepository.GetSingleById(item.id);
                    var stockTotalAll = _softStockRepository.GetStockTotalAll(item.id);
                    if (shopSanPham != null)
                    {
                        shopSanPham.PriceBaseOld = shopSanPham.PriceBase;
                        shopSanPham.PriceBase = item.PriceNew;
                        shopSanPham.PriceRef = item.PriceRef;
                        shopSanPham.UpdatedDate = DateTime.Now;
                        shopSanPham.UpdatedBy = softStockInVm.CreatedBy;
                        var checkZero = ((int)stockTotalAll + item.Quantity);
                        if (checkZero == 0)
                            shopSanPham.PriceAvg = shopSanPham.PriceBase;
                        else
                            shopSanPham.PriceAvg = (shopSanPham.PriceAvg * (int)stockTotalAll + item.PriceNew * item.Quantity) / ((int)stockTotalAll + item.Quantity);
                        var priceOnl = 0;
                        var priceOnlModel = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == item.id && x.ChannelId == 2);
                        if (priceOnlModel != null)
                            priceOnl = priceOnlModel.Price.Value;

                        shopSanPham.PriceWholesale = UtilExtensions.GetPriceWholesaleByPriceAvgOnl(shopSanPham.PriceAvg, priceOnl);
                        //set UpdateBy
                        _shopSanPhamRepository.Update(shopSanPham);
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Sản phẩm này không tồn tại!");
                        return response;
                    }

                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockIn.ToBranchId.Value && x.ProductId == item.id);
                    if (productInBranch == null)
                    {
                        var newStock = UtilExtensions.InitStock(softStockIn.ToBranchId.Value, item.id);
                        _softStockRepository.Add(newStock);
                        _unitOfWork.Commit();
                        productInBranch = newStock;
                    }
                    productInBranch.StockTotal += item.Quantity;
                    productInBranch.UpdatedDate = DateTime.Now;
                    _softStockRepository.Update(productInBranch);


                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.id;
                    productLog.Description = "Nhập kho, mã đơn: " + softStockIn.Id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = softStockInVm.CreatedBy;
                    productLog.CreatedDate = DateTime.Now;
                    productLog.BranchId = softStockInVm.ToBranchId;
                    productLog.StockTotal = productInBranch.StockTotal.Value;
                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(item.id) + item.Quantity.Value;
                    _shopSanPhamLogRepository.Add(productLog);

                    foreach (var itemBranch in branches)
                    {
                        double stockTmp = 0;
                        var stockPrivate = _softStockRepository.GetSingleByCondition(x => x.BranchId == itemBranch && x.ProductId == item.id);
                        if (stockPrivate != null)
                            stockTmp = stockPrivate.StockTotal.Value;
                        if (stockTmp == 0)
                        {
                            stockoutAble = true;
                            break;
                        }
                    }
                }
                _unitOfWork.Commit();
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    var product = _shopSanPhamRepository.GetSingleById(item.id);
                    if (product.PriceBase != product.PriceBaseOld)
                    {
                        updatedPrice = true;
                        break;
                    }

                }
                var result = new SoftStockInAddReturnViewModel();
                if (stockoutAble == true && softStockInVm.BranchId == 2)
                    result.stockoutAble = true;
                if (updatedPrice == true)
                    result.updatedPrice = true;
                if (!string.IsNullOrEmpty(fromSuppliers))
                {
                    var crSoftStockIn = _softStockInRepository.GetSingleById(softStockIn.Id);
                    crSoftStockIn.FromSuppliers = fromSuppliers;
                    _softStockInRepository.Update(crSoftStockIn);
                    _unitOfWork.Commit();
                }
                response = request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("addexist")]
        [Authorize(Roles = "StockInAdd")]
        [HttpGet]
        public HttpResponseMessage AddExist(HttpRequestMessage request, int stockinId, int userId)
        {
            bool stockoutAble = false;
            bool updatedPrice = false;
            var branches = _softBranchRepository.GetAllIds().ToList();
            branches.Remove(2);
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);

                if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02" || (softStockIn.CategoryId == "00" && softStockIn.SupplierStatusId == "02"))
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                if (softStockIn.CategoryId == "00")
                    softStockIn.SupplierStatusId = "08";
                softStockIn.ToBranchStatusId = "07";
                softStockIn.StockInDate = DateTime.Now;
                _softStockInRepository.Update(softStockIn);
                _unitOfWork.Commit();
                foreach (var item in softStockIn.SoftStockInDetails)
                {
                    //Update Product, Stock
                    if (softStockIn.CategoryId == "00" || softStockIn.CategoryId == "02")
                    {
                        var shopSanPham = _shopSanPhamRepository.GetSingleById(item.ProductId);
                        var stockTotalAll = _softStockRepository.GetStockTotalAll(item.ProductId);
                        if (shopSanPham != null)
                        {
                            shopSanPham.PriceBaseOld = shopSanPham.PriceBase;
                            shopSanPham.PriceBase = item.PriceNew;
                            if (softStockIn.CategoryId == "02")
                                shopSanPham.PriceRef = item.PriceRef;
                            shopSanPham.UpdatedDate = DateTime.Now;
                            shopSanPham.UpdatedBy = userId;
                            var checkZero = ((int)stockTotalAll + item.Quantity);
                            if (checkZero == 0)
                                shopSanPham.PriceAvg = shopSanPham.PriceBase;
                            else
                                shopSanPham.PriceAvg = (shopSanPham.PriceAvg * (int)stockTotalAll + item.PriceNew * item.Quantity) / ((int)stockTotalAll + item.Quantity);
                            var priceOnl = 0;
                            var priceOnlModel = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == item.ProductId && x.ChannelId == 2);
                            if (priceOnlModel != null)
                                priceOnl = priceOnlModel.Price.Value;
                            shopSanPham.PriceWholesale = UtilExtensions.GetPriceWholesaleByPriceAvgOnl(shopSanPham.PriceAvg, priceOnl);
                            _shopSanPhamRepository.Update(shopSanPham);
                        }
                        else
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Sản phẩm này không tồn tại!");
                            return response;
                        }
                    }

                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockIn.ToBranchId && x.ProductId == item.ProductId);
                    if (productInBranch == null)
                    {
                        var newStock = UtilExtensions.InitStock(softStockIn.ToBranchId.Value, item.ProductId);
                        _softStockRepository.Add(newStock);
                        _unitOfWork.Commit();
                        productInBranch = newStock;
                    }
                    productInBranch.StockTotal += item.Quantity;
                    productInBranch.UpdatedDate = DateTime.Now;
                    productInBranch.UpdatedBy = userId;
                    _softStockRepository.Update(productInBranch);

                    //Add Log
                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.ProductId;
                    //var branch = _softBranchRepository.GetSingleById(softStockIn.BranchId.Value);
                    //switch (softStockIn.CategoryId)
                    //{
                    //    case "00":
                    //        var supplierLog = _softSupplierRepository.GetSingleById(softStockIn.SupplierId.Value);
                    //        productLog.Description = "Nhập kho " + branch.Name + ", từ đơn đặt hàng NCC " + supplierLog.Name + ", mã đơn: " + softStockIn.Id;
                    //        break;
                    //    case "01":
                    //        var branchBookLog = _softBranchRepository.GetSingleById(softStockIn.FromBranchId.Value);
                    //        productLog.Description = "Nhập kho " + branch.Name + ", từ đơn đặt hàng kho " + branchBookLog.Name + ", mã đơn: " + softStockIn.Id;
                    //        break;
                    //    case "02":
                    //        productLog.Description = "Nhập kho " + branch.Name + ", từ đơn nhập kho mới, mã đơn: " + softStockIn.Id;
                    //        break;
                    //    case "03":
                    //        var branchStockoutLog = _softBranchRepository.GetSingleById(softStockIn.FromBranchId.Value);
                    //        var branchStockinLog03 = _softBranchRepository.GetSingleById(softStockIn.ToBranchId.Value);
                    //        productLog.Description = "Nhập kho " + branchStockinLog03.Name + ", từ đơn xuất kho " + branchStockoutLog.Name + ", mã đơn: " + softStockIn.Id;
                    //        break;
                    //}
                    productLog.Description = "Nhập kho, mã đơn: " + softStockIn.Id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = userId;
                    productLog.CreatedDate = DateTime.Now;
                    productLog.BranchId = softStockIn.ToBranchId;
                    productLog.StockTotal = productInBranch.StockTotal.Value;
                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(item.ProductId) + item.Quantity.Value;
                    _shopSanPhamLogRepository.Add(productLog);

                    foreach (var itemBranch in branches)
                    {
                        double stockTmp = 0;
                        var stockPrivate = _softStockRepository.GetSingleByCondition(x => x.BranchId == itemBranch && x.ProductId == item.ProductId);
                        if (stockPrivate != null)
                            stockTmp = stockPrivate.StockTotal.Value;
                        if (stockTmp == 0)
                        {
                            stockoutAble = true;
                            break;
                        }
                    }
                }
                _unitOfWork.Commit();
                foreach (var item in softStockIn.SoftStockInDetails)
                {
                    var product = _shopSanPhamRepository.GetSingleById(item.ProductId);
                    if (product.PriceBase != product.PriceBaseOld)
                    {
                        updatedPrice = true;
                        break;
                    }

                }
                var result = new SoftStockInAddReturnViewModel();
                if (stockoutAble == true && softStockIn.BranchId == 2)
                    result.stockoutAble = true;
                if (updatedPrice == true)
                    result.updatedPrice = true;
                var detailVm = Mapper.Map<IEnumerable<SoftStockInDetail>, IEnumerable<SoftStockInProductViewModel>>(softStockIn.SoftStockInDetails);
                result.SoftStockInDetails = detailVm.ToList();
                response = request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("outexist")]
        [Authorize(Roles = "StockOutAdd")]
        [HttpGet]
        public HttpResponseMessage OutExist(HttpRequestMessage request, int stockinId, int userId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);

                if (softStockIn.FromBranchStatusId == "06" || softStockIn.FromBranchStatusId == "02" || (softStockIn.CategoryId == "01" && softStockIn.SupplierStatusId == "02"))
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }

                foreach (var item in softStockIn.SoftStockInDetails)
                {
                    double stockTmp = 0;
                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockIn.FromBranchId.Value && x.ProductId == item.ProductId);
                    if (productInBranch != null)
                    {
                        stockTmp = productInBranch.StockTotal.Value;
                    }
                    if (stockTmp < item.Quantity)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Tồn tại kho của " + item.shop_sanpham.masp.Trim() + " không đủ xuất!");
                        return response;
                    }
                }
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                softStockIn.SupplierStatusId = "08";
                softStockIn.FromBranchStatusId = "06";
                softStockIn.StockOutDate = DateTime.Now;
                _softStockInRepository.Update(softStockIn);
                //_unitOfWork.Commit();

                foreach (var item in softStockIn.SoftStockInDetails)
                {


                    //Update Product, Stock
                    //var shopSanPham = _shopSanPhamRepository.GetSingleById(item.ProductId);

                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockIn.FromBranchId.Value && x.ProductId == item.ProductId);
                    if (productInBranch == null)
                    {
                        var newStockCurrent = _softStockRepository.Init(softStockIn.FromBranchId.Value, item.ProductId);
                        productInBranch = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    productInBranch.StockTotal -= item.Quantity;
                    productInBranch.UpdatedDate = DateTime.Now;
                    _softStockRepository.Update(productInBranch);

                    //Add Log
                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.ProductId;
                    //switch (softStockIn.CategoryId)
                    //{
                    //    case "01":
                    //        var branch01 = _softBranchRepository.GetSingleById(softStockIn.FromBranchId.Value);
                    //        var branch01Book = _softBranchRepository.GetSingleById(softStockIn.ToBranchId.Value);
                    //        productLog.Description = "Xuất kho " + branch01.Name + ", từ đơn đặt hàng kho " + branch01Book.Name + ", mã đơn: " + softStockIn.Id;
                    //        break;
                    //    case "03":
                    //        var branch03 = _softBranchRepository.GetSingleById(softStockIn.BranchId.Value);
                    //        var branch03Stockin = _softBranchRepository.GetSingleById(softStockIn.ToBranchId.Value);
                    //        productLog.Description = "Xuất kho " + branch03.Name + ", từ đơn xuất kho mới đến kho " + branch03Stockin.Name + ", mã đơn: " + softStockIn.Id;
                    //        break;
                    //}
                    productLog.Description = "Xuất kho, mã đơn: " + softStockIn.Id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = userId;
                    productLog.CreatedDate = DateTime.Now;
                    productLog.BranchId = softStockIn.FromBranchId;
                    productLog.StockTotal = productInBranch.StockTotal.Value;
                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(item.ProductId) - item.Quantity.Value;
                    _shopSanPhamLogRepository.Add(productLog);
                }

                var newSoftNotification = new SoftNotification();
                newSoftNotification.StockinId = softStockIn.Id;
                newSoftNotification.Message = "Đơn xuất kho " + softStockIn.Id + " cần được xử lý";
                newSoftNotification.Url = "stockins";
                newSoftNotification.FromBranchId = softStockIn.FromBranchId;
                newSoftNotification.ToBranchId = softStockIn.ToBranchId;
                newSoftNotification.IsRead = false;
                newSoftNotification.Status = false;
                newSoftNotification.CreatedDate = DateTime.Now;
                _softNotificationRepository.Add(newSoftNotification);

                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, softStockIn.ToBranchId);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("out")]
        [HttpPost]
        public HttpResponseMessage Out(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var itemCheck in softStockInVm.SoftStockInDetails)
                {
                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockInVm.BranchId && x.ProductId == itemCheck.id);
                    if (productInBranch != null)
                    {
                        if (productInBranch.StockTotal < itemCheck.Quantity)
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Tồn tại kho không đủ xuất!");
                            return response;
                        }
                    }
                    else
                    {
                        var newstock = new SoftBranchProductStock();
                        newstock.ProductId = itemCheck.id;
                        newstock.BranchId = softStockInVm.BranchId;
                        newstock.StockTotal = 0;
                        newstock.CreatedDate = DateTime.Now;
                        _softStockRepository.Add(newstock);
                        _unitOfWork.Commit();
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Tồn tại kho không đủ xuất!");
                        return response;
                    }
                }

                SoftStockIn softStockIn = new SoftStockIn();
                softStockIn.UpdateSoftStockIn(softStockInVm);
                softStockIn.CreatedDate = DateTime.Now;
                _softStockInRepository.Add(softStockIn);
                _unitOfWork.Commit();
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    softStockInDetail.UpdateSoftStockInDetail(item);
                    softStockInDetail.StockInId = softStockIn.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);

                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockInVm.BranchId && x.ProductId == item.id);
                    if (productInBranch == null)
                    {
                        var newStockCurrent = _softStockRepository.Init(softStockInVm.BranchId.Value, item.id);
                        productInBranch = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    productInBranch.StockTotal -= item.Quantity;
                    _softStockRepository.Update(productInBranch);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("update")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                //SoftStockIn softStockIn = new SoftStockIn();
                //softStockIn.UpdateSoftStockIn(softStockInVm);
                //softStockIn.CreatedDate = DateTime.Now;
                //_softStockInRepository.Add(softStockIn);

                var oldSoftStockIn = _softStockInRepository.GetSingleById(softStockInVm.Id);
                oldSoftStockIn.SupplierId = softStockInVm.SupplierId;
                oldSoftStockIn.Total = softStockInVm.Total;
                oldSoftStockIn.UpdatedDate = DateTime.Now;
                oldSoftStockIn.StatusId = softStockInVm.StatusId;
                _softStockInRepository.Update(oldSoftStockIn);
                //update detail 
                _softStockInDetailRepository.DeleteMulti(x => x.StockInId == softStockInVm.Id);
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    softStockInDetail.UpdateSoftStockInDetail(item);
                    softStockInDetail.StockInId = softStockInVm.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);
                    //Update Product, Stock
                    var shopSanPham = _shopSanPhamRepository.GetSingleById(item.id);
                    if (shopSanPham != null)
                    {
                        shopSanPham.PriceBaseOld = shopSanPham.PriceBase;
                        shopSanPham.PriceBase = item.PriceNew;
                        shopSanPham.UpdatedDate = DateTime.Now;
                        //set UpdateBy
                        _shopSanPhamRepository.Update(shopSanPham);
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Sản phẩm này không tồn tại!");
                        return response;
                    }
                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == softStockInVm.BranchId && x.ProductId == item.id);
                    if (productInBranch == null)
                    {
                        var newStockCurrent = _softStockRepository.Init(softStockInVm.BranchId.Value, item.id);
                        productInBranch = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    productInBranch.StockTotal += item.Quantity;
                    productInBranch.UpdatedDate = DateTime.Now;
                    //set UpdateBy
                    _softStockRepository.Update(productInBranch);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("updatebook")]
        [HttpPost]
        public HttpResponseMessage UpdateBook(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                double stockCurrent = 0;
                var oldSoftStockIn = _softStockInRepository.GetSingleById(softStockInVm.Id);

                if (oldSoftStockIn.CategoryId == "00" && (oldSoftStockIn.SupplierStatusId == "08" || oldSoftStockIn.SupplierStatusId == "02"))
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }

                if (oldSoftStockIn.CategoryId == "01" && (oldSoftStockIn.SupplierStatusId == "08" || oldSoftStockIn.SupplierStatusId == "02"))
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }

                if (oldSoftStockIn.CategoryId == "02" && (oldSoftStockIn.ToBranchStatusId == "07" || oldSoftStockIn.ToBranchStatusId == "02"))
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }

                if (oldSoftStockIn.CategoryId == "03" && (oldSoftStockIn.FromBranchStatusId == "06" || oldSoftStockIn.FromBranchStatusId == "02"))
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }

                oldSoftStockIn.UpdatedDate = DateTime.Now;
                oldSoftStockIn.UpdatedBy = softStockInVm.UpdatedBy;
                oldSoftStockIn.Description = softStockInVm.Description;
                oldSoftStockIn.Total = softStockInVm.Total;
                oldSoftStockIn.TotalQuantity = softStockInVm.TotalQuantity;
                if (oldSoftStockIn.CategoryId == "01")
                {
                    foreach (var item in softStockInVm.SoftStockInDetails)
                    {
                        stockCurrent = _softStockRepository.GetStockTotal(item.id, oldSoftStockIn.FromBranchId.Value);
                        if (item.Quantity > stockCurrent)
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Tồn tại kho đặt của " + item.masp.Trim() + " không đủ xuất!");
                            return response;
                        }
                    }

                    oldSoftStockIn.FromBranchId = softStockInVm.FromBranchId;
                    oldSoftStockIn.SupplierStatusId = "05";
                    var newSoftNotification = new SoftNotification();
                    newSoftNotification.StockinId = oldSoftStockIn.Id;
                    newSoftNotification.Message = "Đơn ĐH kho " + oldSoftStockIn.Id + " đã được cập nhật";
                    newSoftNotification.Url = "stockouts";
                    newSoftNotification.FromBranchId = oldSoftStockIn.ToBranchId;
                    newSoftNotification.ToBranchId = oldSoftStockIn.FromBranchId;
                    newSoftNotification.IsRead = false;
                    newSoftNotification.Status = false;
                    newSoftNotification.CreatedDate = DateTime.Now;
                    _softNotificationRepository.Add(newSoftNotification);


                }
                if (oldSoftStockIn.CategoryId == "03")
                {
                    oldSoftStockIn.ToBranchId = softStockInVm.ToBranchId;
                }
                _softStockInRepository.Update(oldSoftStockIn);
                //update detail 
                _softStockInDetailRepository.DeleteMulti(x => x.StockInId == softStockInVm.Id);
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    if (oldSoftStockIn.CategoryId == "01")
                        softStockInDetail.UpdateSoftStockInDetail(item, "01");
                    else
                        softStockInDetail.UpdateSoftStockInDetail(item, "02");
                    softStockInDetail.StockInId = softStockInVm.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("save")]
        [HttpPost]
        public HttpResponseMessage Save(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    if (item.Quantity == null)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Số lượng mã " + item.masp.Trim() + " lỗi!");
                        return response;
                    }
                }

                double stockCurrent = 0;
                var fromSuppliers = "";
                SoftStockIn softStockIn = new SoftStockIn();
                softStockIn.UpdateSoftStockIn(softStockInVm);
                softStockIn.CreatedDate = DateTime.Now;
                if (softStockInVm.CategoryId == "00" && softStockInVm.SupplierId == 0)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Sản phẩm chưa cập nhật nhà cung cấp!");
                    return response;
                }
                _softStockInRepository.Add(softStockIn);
                _unitOfWork.Commit();
                if (softStockInVm.CategoryId == "01")
                {
                    var newSoftNotification = new SoftNotification();
                    newSoftNotification.StockinId = softStockIn.Id;
                    newSoftNotification.Message = "Đơn ĐH kho " + softStockIn.Id + " cần được xử lý";
                    newSoftNotification.Url = "stockouts";
                    newSoftNotification.FromBranchId = softStockInVm.ToBranchId;
                    newSoftNotification.ToBranchId = softStockInVm.FromBranchId;
                    newSoftNotification.IsRead = false;
                    newSoftNotification.Status = false;
                    newSoftNotification.CreatedDate = DateTime.Now;
                    _softNotificationRepository.Add(newSoftNotification);
                }
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    var ctProduct = _shopSanPhamRepository.GetSingleById(item.id);
                    if (softStockInVm.CategoryId == "02")
                        softStockInDetail.UpdateSoftStockInDetail(item, "02");
                    else if (softStockInVm.CategoryId == "01")
                        softStockInDetail.UpdateSoftStockInDetail(item, "01");
                    else
                        softStockInDetail.UpdateSoftStockInDetail(item);
                    if (softStockInVm.CategoryId == "00" && ctProduct.SupplierId.HasValue && !fromSuppliers.Contains(ctProduct.SoftSupplier.Name))
                    {
                        fromSuppliers += ctProduct.SoftSupplier.Name + "|";
                    }
                    softStockInDetail.StockInId = softStockIn.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);
                }
                _unitOfWork.Commit();
                if (!string.IsNullOrEmpty(fromSuppliers))
                {
                    var crSoftStockIn = _softStockInRepository.GetSingleById(softStockIn.Id);
                    crSoftStockIn.FromSuppliers = fromSuppliers;
                    _softStockInRepository.Update(crSoftStockIn);
                    _unitOfWork.Commit();
                }

                response = request.CreateResponse(HttpStatusCode.OK, softStockIn.Id);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("savestockout")]
        [HttpPost]
        public HttpResponseMessage SaveStockOut(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            if (softStockInVm == null)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, "Danh sách xuất kho lỗi");
                return response;
            }
            foreach (var item in softStockInVm.SoftStockInDetails)
            {
                if (item.Quantity == null)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Số lượng mã " + item.masp.Trim() + " lỗi!");
                    return response;
                }
            }
            try
            {
                double stockCurrent = 0;
                SoftStockIn softStockIn = new SoftStockIn();

                softStockIn.BranchId = softStockInVm.BranchId;
                softStockIn.CategoryId = softStockInVm.CategoryId;
                softStockIn.Description = softStockInVm.Description;
                softStockIn.CreatedBy = softStockInVm.CreatedBy;
                softStockIn.Total = softStockInVm.Total;
                softStockIn.TotalQuantity = softStockInVm.TotalQuantity;
                softStockIn.FromBranchId = softStockInVm.FromBranchId;
                softStockIn.FromBranchStatusId = "01";
                softStockIn.ToBranchId = softStockInVm.ToBranchId;
                softStockIn.ToBranchStatusId = "03";
                softStockIn.CreatedDate = DateTime.Now;

                //if (softStockInVm.CategoryId == "03")
                //{
                //    foreach (var item in softStockInVm.SoftStockInDetails)
                //    {
                //        stockCurrent = _softStockRepository.GetStockTotal(item.id, softStockInVm.BranchId.Value);
                //        if (item.Quantity > stockCurrent)
                //        {
                //            response = request.CreateResponse(HttpStatusCode.BadRequest, "Tồn tại kho của " + item.masp.Trim() + " không đủ xuất!");
                //            return response;
                //        }
                //    }
                //}

                _softStockInRepository.Add(softStockIn);
                _unitOfWork.Commit();
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    softStockInDetail.ProductId = item.id;
                    softStockInDetail.Quantity = item.Quantity;
                    softStockInDetail.PriceNew = item.PriceNew;
                    softStockInDetail.StockInId = softStockIn.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, softStockIn.Id);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("delete")]
        [HttpDelete]
        public HttpResponseMessage Delete(HttpRequestMessage request, int stockInId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockInId);
                softStockIn.StatusId = "02";
                softStockIn.UpdatedDate = DateTime.Today;
                _softStockInRepository.Update(softStockIn);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        //[Route("detail")]
        //[HttpGet]
        //public HttpResponseMessage Detail(HttpRequestMessage request, int stockinId)
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        var softStockInDetails = _softStockInDetailRepository.GetMulti(x => x.StockInId == stockinId);
        //        var responsedata = Mapper.Map<IEnumerable<SoftStockInDetail>, IEnumerable<SoftStockInDetailViewModel>>(softStockInDetails);
        //        response = request.CreateResponse(HttpStatusCode.OK, responsedata);
        //        return response;
        //    }

        //    catch (Exception ex)
        //    {
        //        response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //        return response;
        //    }
        //}

        [Route("detail")]
        [HttpGet]
        public HttpResponseMessage Detail(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detailtoprint")]
        [HttpGet]
        public HttpResponseMessage DetailToPrint(HttpRequestMessage request, int stockinId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detailbook")]
        [Authorize(Roles = "BookDetail")]
        [HttpGet]
        public HttpResponseMessage DetailBook(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detailstockout")]
        [Authorize(Roles = "StockOutDetail")]
        [HttpGet]
        public HttpResponseMessage DetailStockOut(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detailstockouttoprint")]
        [HttpGet]
        public HttpResponseMessage DetailStockOutToPrint(HttpRequestMessage request, int stockoutId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockoutId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detailstockin")]
        [Authorize(Roles = "StockInDetail")]
        [HttpGet]
        public HttpResponseMessage DetailStockIn(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detailbranchbook")]
        [Authorize(Roles = "BranchBookDetail")]
        [HttpGet]
        public HttpResponseMessage DetailBranchBook(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detaileditbranchbook")]
        [Authorize(Roles = "BranchBookEdit")]
        [HttpGet]
        public HttpResponseMessage DetailEditBranchBook(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detaileditbook")]
        [Authorize(Roles = "BookEdit")]
        [HttpGet]
        public HttpResponseMessage DetailEditBook(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detaileditstockin")]
        [Authorize(Roles = "StockInEdit")]
        [HttpGet]
        public HttpResponseMessage DetailEditStockin(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detaileditstockout")]
        [Authorize(Roles = "StockOutEdit")]
        [HttpGet]
        public HttpResponseMessage DetailEditStockOut(HttpRequestMessage request, int stockinId, int branchId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                foreach (var item in responsedata.SoftStockInDetails)
                {
                    var shopsanpham = _shopSanPhamRepository.GetSingleById(item.id);
                    item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
                    item.PriceAvg = shopsanpham.PriceAvg == null ? 0 : shopsanpham.PriceAvg.Value;
                }
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("getbyid/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(id);
                var responsedata = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("edit")]
        [HttpPost]
        public HttpResponseMessage Edit(HttpRequestMessage request, SoftStockInViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var oldSoftStockIn = _softStockInRepository.GetSingleById(softStockInVm.Id);
                oldSoftStockIn.SupplierId = softStockInVm.SupplierId;
                oldSoftStockIn.Total = softStockInVm.Total;
                oldSoftStockIn.UpdatedDate = DateTime.Now;
                oldSoftStockIn.StatusId = softStockInVm.StatusId;
                _softStockInRepository.Update(oldSoftStockIn);
                //update detail 
                _softStockInDetailRepository.DeleteMulti(x => x.StockInId == softStockInVm.Id);
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    SoftStockInDetail softStockInDetail = new SoftStockInDetail();
                    softStockInDetail.UpdateSoftStockInDetail(item);
                    softStockInDetail.StockInId = softStockInVm.Id;
                    _softStockInDetailRepository.Add(softStockInDetail);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        //[HttpGet]
        //[Route("search")]
        //public HttpResponseMessage Search(HttpRequestMessage request, int branchId, int? page, int? pageSize, string categoryId = null, string filter = null)
        //{
        //    int currentPage = page.Value;
        //    int currentPageSize = pageSize.Value;
        //    HttpResponseMessage response = null;
        //    List<SoftStockIn> stockIns = null;
        //    int totalStockIns = new int();

        //    if (!string.IsNullOrEmpty(filter))
        //    {
        //        filter = filter.Trim().ToLower();

        //        stockIns = _softStockInRepository.GetMulti(c => c.CategoryId == categoryId && c.BranchId == branchId && (c.SoftSupplier.Name.ToLower().Contains(filter) || c.Id.ToString() == filter)).OrderByDescending(c => c.Id)
        //        .Skip(currentPage * currentPageSize)
        //        .Take(currentPageSize)
        //        .ToList(); ;

        //        totalStockIns = _softStockInRepository.GetMulti(c => c.CategoryId == categoryId && c.BranchId == branchId && (c.SoftSupplier.Name.ToLower().Contains(filter) || c.Id.ToString() == filter)).Count();
        //    }
        //    else
        //    {
        //        stockIns = _softStockInRepository.GetMulti(c => c.BranchId == branchId && c.CategoryId == categoryId).OrderByDescending(c => c.Id)
        //        .Skip(currentPage * currentPageSize)
        //        .Take(currentPageSize)
        //        .ToList();

        //        totalStockIns = _softStockInRepository.GetMulti(c => c.BranchId == branchId && c.CategoryId == categoryId).Count();
        //    }

        //    IEnumerable<SoftStockInViewModel> stockInsVM = Mapper.Map<IEnumerable<SoftStockIn>, IEnumerable<SoftStockInViewModel>>(stockIns);

        //    PaginationSet<SoftStockInViewModel> pagedSet = new PaginationSet<SoftStockInViewModel>()
        //    {
        //        Page = currentPage,
        //        TotalCount = totalStockIns,
        //        TotalPages = (int)Math.Ceiling((decimal)totalStockIns / currentPageSize),
        //        Items = stockInsVM
        //    };

        //    response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
        //    return response;
        //}

        [HttpPost]
        [Route("search")]
        public HttpResponseMessage Search(HttpRequestMessage request, BookFilterViewModel bookFilterVM)
        {
            int currentPage = bookFilterVM.page.Value;
            int currentPageSize = bookFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftStockIn> stockIns = null;
            int totalStockIns = 0;
            long totalMoney = 0;

            stockIns = _softStockInRepository.GetAllPaging(currentPage, currentPageSize, out totalStockIns, out totalMoney, bookFilterVM).ToList();

            IEnumerable<SoftStockInSearchViewModel> stockInsVM = Mapper.Map<IEnumerable<SoftStockIn>, IEnumerable<SoftStockInSearchViewModel>>(stockIns);

            foreach (var item in stockInsVM)
            {
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
            }

            PaginationSet<SoftStockInSearchViewModel> pagedSet = new PaginationSet<SoftStockInSearchViewModel>()
            {
                Page = currentPage,
                TotalCount = totalStockIns,
                TotalPages = (int)Math.Ceiling((decimal)totalStockIns / currentPageSize),
                Items = stockInsVM,
                TotalMoney = totalMoney
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [HttpPost]
        [Authorize(Roles = "BranchBookList")]
        [Route("searchbranchbook")]
        public HttpResponseMessage SearchBranchBook(HttpRequestMessage request, BookFilterViewModel bookFilterVM)
        {
            int currentPage = bookFilterVM.page.Value;
            int currentPageSize = bookFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftStockIn> stockIns = null;
            int totalStockIns = 0;
            long totalMoney = 0;

            stockIns = _softStockInRepository.GetAllPaging(currentPage, currentPageSize, out totalStockIns, out totalMoney, bookFilterVM).ToList();

            IEnumerable<SoftStockInSearchViewModel> stockInsVM = Mapper.Map<IEnumerable<SoftStockIn>, IEnumerable<SoftStockInSearchViewModel>>(stockIns);

            foreach (var item in stockInsVM)
            {
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
            }

            PaginationSet<SoftStockInSearchViewModel> pagedSet = new PaginationSet<SoftStockInSearchViewModel>()
            {
                Page = currentPage,
                TotalCount = totalStockIns,
                TotalPages = (int)Math.Ceiling((decimal)totalStockIns / currentPageSize),
                Items = stockInsVM,
                TotalMoney = totalMoney
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [HttpPost]
        [Authorize(Roles = "BookList")]
        [Route("searchbook")]
        public HttpResponseMessage SearchBook(HttpRequestMessage request, BookFilterViewModel bookFilterVM)
        {
            int currentPage = bookFilterVM.page.Value;
            int currentPageSize = bookFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftStockIn> stockIns = null;
            int totalStockIns = 0;
            long totalMoney = 0;

            stockIns = _softStockInRepository.GetAllPaging(currentPage, currentPageSize, out totalStockIns, out totalMoney, bookFilterVM).ToList();

            IEnumerable<SoftStockInSearchViewModel> stockInsVM = Mapper.Map<IEnumerable<SoftStockIn>, IEnumerable<SoftStockInSearchViewModel>>(stockIns);

            foreach (var item in stockInsVM)
            {
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
                if (!string.IsNullOrEmpty(item.FromSuppliers))
                {
                    item.FromSuppliers = item.FromSuppliers.Substring(0, item.FromSuppliers.Length - 1).Replace("|", " , ");
                }
            }

            PaginationSet<SoftStockInSearchViewModel> pagedSet = new PaginationSet<SoftStockInSearchViewModel>()
            {
                Page = currentPage,
                TotalCount = totalStockIns,
                TotalPages = (int)Math.Ceiling((decimal)totalStockIns / currentPageSize),
                Items = stockInsVM,
                TotalMoney = totalMoney
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [HttpPost]
        [Authorize(Roles = "StockInList")]
        [Route("searchstockin")]
        public HttpResponseMessage SearchStockIn(HttpRequestMessage request, BookFilterViewModel bookFilterVM)
        {
            int currentPage = bookFilterVM.page.Value;
            int currentPageSize = bookFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftStockIn> stockIns = null;
            int totalStockIns = 0;
            long totalMoney = 0;

            stockIns = _softStockInRepository.GetAllPaging(currentPage, currentPageSize, out totalStockIns, out totalMoney, bookFilterVM).ToList();

            IEnumerable<SoftStockInSearchViewModel> stockInsVM = Mapper.Map<IEnumerable<SoftStockIn>, IEnumerable<SoftStockInSearchViewModel>>(stockIns);

            foreach (var item in stockInsVM)
            {
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
                if (item.StockInDate != null)
                    item.StockInDateConvert = UtilExtensions.ConvertDate(item.StockInDate.Value);
                if (item.PaidDate != null)
                    item.PaidDateConvert = UtilExtensions.ConvertDate(item.PaidDate.Value);
                if (!string.IsNullOrEmpty(item.FromSuppliers))
                {
                    item.FromSuppliers = item.FromSuppliers.Substring(0, item.FromSuppliers.Length - 1).Replace("|", " , ");
                }
            }

            PaginationSet<SoftStockInSearchViewModel> pagedSet = new PaginationSet<SoftStockInSearchViewModel>()
            {
                Page = currentPage,
                TotalCount = totalStockIns,
                TotalPages = (int)Math.Ceiling((decimal)totalStockIns / currentPageSize),
                Items = stockInsVM,
                TotalMoney = totalMoney
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [HttpPost]
        [Authorize(Roles = "StockOutList")]
        [Route("searchstockout")]
        public HttpResponseMessage SearchStockOut(HttpRequestMessage request, BookFilterViewModel bookFilterVM)
        {
            int currentPage = bookFilterVM.page.Value;
            int currentPageSize = bookFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftStockIn> stockIns = null;
            int totalStockIns = 0;
            long totalMoney = 0;

            stockIns = _softStockInRepository.GetAllPaging(currentPage, currentPageSize, out totalStockIns, out totalMoney, bookFilterVM).ToList();

            IEnumerable<SoftStockInSearchViewModel> stockInsVM = Mapper.Map<IEnumerable<SoftStockIn>, IEnumerable<SoftStockInSearchViewModel>>(stockIns);

            foreach (var item in stockInsVM)
            {
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
                if (item.StockOutDate != null)
                    item.StockOutDateConvert = UtilExtensions.ConvertDate(item.StockOutDate.Value);
            }

            PaginationSet<SoftStockInSearchViewModel> pagedSet = new PaginationSet<SoftStockInSearchViewModel>()
            {
                Page = currentPage,
                TotalCount = totalStockIns,
                TotalPages = (int)Math.Ceiling((decimal)totalStockIns / currentPageSize),
                Items = stockInsVM,
                TotalMoney = totalMoney
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("getallbookstatus")]
        [HttpGet]
        public HttpResponseMessage GetAllBookStatus(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockInStatus = _softStockInStatusRepository.GetAll().OrderBy(x => x.Id);
                var softStockInStatusVm = Mapper.Map<IEnumerable<SoftStockInStatu>, IEnumerable<SoftStockInStatusViewModel>>(softStockInStatus);
                response = request.CreateResponse(HttpStatusCode.OK, softStockInStatusVm);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("updatecancel")]
        [HttpGet]
        public HttpResponseMessage UpdateCancel(HttpRequestMessage request, int stockinId, int userId, string type)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                switch (type)
                {
                    case "fromsupplier":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        break;
                    case "supplierstockin":
                        softStockIn.SupplierStatusId = "08";
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "branch":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        var newSoftNotification = new SoftNotification();
                        newSoftNotification.StockinId = softStockIn.Id;
                        newSoftNotification.Message = "Đơn ĐH kho nội bộ " + softStockIn.Id + " đã huỷ";
                        newSoftNotification.Url = "#";
                        newSoftNotification.FromBranchId = softStockIn.BranchId;
                        newSoftNotification.ToBranchId = softStockIn.FromBranchId;
                        newSoftNotification.IsRead = false;
                        newSoftNotification.Status = false;
                        newSoftNotification.CreatedDate = DateTime.Now;
                        _softNotificationRepository.Add(newSoftNotification);
                        break;
                    case "stockout":
                        if (softStockIn.FromBranchStatusId == "07" || softStockIn.FromBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.FromBranchStatusId = "02";
                        break;
                    case "stockin":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "stockinbook":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02" || softStockIn.SupplierStatusId == "02" || softStockIn.SupplierStatusId == "08")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        softStockIn.SupplierStatusId = "08";
                        break;
                }
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                _softStockInRepository.Update(softStockIn);
                _unitOfWork.Commit();

                var softStockInVM = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);

                response = request.CreateResponse(HttpStatusCode.OK, softStockInVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("updatecancelbook")]
        [Authorize(Roles = "BookDelete")]
        [HttpGet]
        public HttpResponseMessage UpdateCancelBook(HttpRequestMessage request, int stockinId, int userId, string type)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                switch (type)
                {
                    case "fromsupplier":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        break;
                    case "supplierstockin":
                        softStockIn.SupplierStatusId = "08";
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "branch":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        var newSoftNotification = new SoftNotification();
                        newSoftNotification.StockinId = softStockIn.Id;
                        newSoftNotification.Message = "Đơn ĐH kho nội bộ " + softStockIn.Id + " đã huỷ";
                        newSoftNotification.Url = "#";
                        newSoftNotification.FromBranchId = softStockIn.BranchId;
                        newSoftNotification.ToBranchId = softStockIn.FromBranchId;
                        newSoftNotification.IsRead = false;
                        newSoftNotification.Status = false;
                        newSoftNotification.CreatedDate = DateTime.Now;
                        _softNotificationRepository.Add(newSoftNotification);
                        break;
                    case "stockout":
                        if (softStockIn.FromBranchStatusId == "07" || softStockIn.FromBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.FromBranchStatusId = "02";
                        break;
                    case "stockin":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "stockinbook":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02" || softStockIn.SupplierStatusId == "02" || softStockIn.SupplierStatusId == "08")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        softStockIn.SupplierStatusId = "08";
                        break;
                }
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                _softStockInRepository.Update(softStockIn);
                _unitOfWork.Commit();

                var softStockInVM = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);

                response = request.CreateResponse(HttpStatusCode.OK, softStockInVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("updatecancelstockin")]
        [Authorize(Roles = "StockInDelete")]
        [HttpGet]
        public HttpResponseMessage UpdateCancelStockIn(HttpRequestMessage request, int stockinId, int userId, string type)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                switch (type)
                {
                    case "fromsupplier":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        break;
                    case "supplierstockin":
                        softStockIn.SupplierStatusId = "08";
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "branch":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        var newSoftNotification = new SoftNotification();
                        newSoftNotification.StockinId = softStockIn.Id;
                        newSoftNotification.Message = "Đơn ĐH kho nội bộ " + softStockIn.Id + " đã huỷ";
                        newSoftNotification.Url = "#";
                        newSoftNotification.FromBranchId = softStockIn.BranchId;
                        newSoftNotification.ToBranchId = softStockIn.FromBranchId;
                        newSoftNotification.IsRead = false;
                        newSoftNotification.Status = false;
                        newSoftNotification.CreatedDate = DateTime.Now;
                        _softNotificationRepository.Add(newSoftNotification);
                        break;
                    case "stockout":
                        if (softStockIn.FromBranchStatusId == "07" || softStockIn.FromBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.FromBranchStatusId = "02";
                        break;
                    case "stockin":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "stockinbook":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02" || softStockIn.SupplierStatusId == "02" || softStockIn.SupplierStatusId == "08")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        softStockIn.SupplierStatusId = "08";
                        break;
                }
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                _softStockInRepository.Update(softStockIn);
                _unitOfWork.Commit();

                var softStockInVM = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);

                response = request.CreateResponse(HttpStatusCode.OK, softStockInVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("updatecancelstockout")]
        [Authorize(Roles = "StockOutDelete")]
        [HttpGet]
        public HttpResponseMessage UpdateCancelStockOut(HttpRequestMessage request, int stockinId, int userId, string type)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                switch (type)
                {
                    case "fromsupplier":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        break;
                    case "supplierstockin":
                        softStockIn.SupplierStatusId = "08";
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "branch":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        var newSoftNotification = new SoftNotification();
                        newSoftNotification.StockinId = softStockIn.Id;
                        newSoftNotification.Message = "Đơn ĐH kho nội bộ " + softStockIn.Id + " đã huỷ";
                        newSoftNotification.Url = "#";
                        newSoftNotification.FromBranchId = softStockIn.BranchId;
                        newSoftNotification.ToBranchId = softStockIn.FromBranchId;
                        newSoftNotification.IsRead = false;
                        newSoftNotification.Status = false;
                        newSoftNotification.CreatedDate = DateTime.Now;
                        _softNotificationRepository.Add(newSoftNotification);
                        break;
                    case "stockout":
                        if (softStockIn.FromBranchStatusId == "07" || softStockIn.FromBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.FromBranchStatusId = "02";
                        break;
                    case "stockin":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "stockinbook":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02" || softStockIn.SupplierStatusId == "02" || softStockIn.SupplierStatusId == "08")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        softStockIn.SupplierStatusId = "08";
                        break;
                }
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                _softStockInRepository.Update(softStockIn);
                _unitOfWork.Commit();

                var softStockInVM = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);

                response = request.CreateResponse(HttpStatusCode.OK, softStockInVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("updatecancelbranchbook")]
        [Authorize(Roles = "BranchBookDelete")]
        [HttpGet]
        public HttpResponseMessage UpdateCancelBranchBook(HttpRequestMessage request, int stockinId, int userId, string type)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                switch (type)
                {
                    case "fromsupplier":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        break;
                    case "supplierstockin":
                        softStockIn.SupplierStatusId = "08";
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "branch":
                        if (softStockIn.SupplierStatusId == "08" || softStockIn.SupplierStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.SupplierStatusId = "02";
                        var newSoftNotification = new SoftNotification();
                        newSoftNotification.StockinId = softStockIn.Id;
                        newSoftNotification.Message = "Đơn ĐH kho nội bộ " + softStockIn.Id + " đã huỷ";
                        newSoftNotification.Url = "#";
                        newSoftNotification.FromBranchId = softStockIn.BranchId;
                        newSoftNotification.ToBranchId = softStockIn.FromBranchId;
                        newSoftNotification.IsRead = false;
                        newSoftNotification.Status = false;
                        newSoftNotification.CreatedDate = DateTime.Now;
                        _softNotificationRepository.Add(newSoftNotification);
                        break;
                    case "stockout":
                        if (softStockIn.FromBranchStatusId == "07" || softStockIn.FromBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.FromBranchStatusId = "02";
                        break;
                    case "stockin":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        break;
                    case "stockinbook":
                        if (softStockIn.ToBranchStatusId == "07" || softStockIn.ToBranchStatusId == "02" || softStockIn.SupplierStatusId == "02" || softStockIn.SupplierStatusId == "08")
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                            return response;
                        }
                        softStockIn.ToBranchStatusId = "02";
                        softStockIn.SupplierStatusId = "08";
                        break;
                }
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                _softStockInRepository.Update(softStockIn);
                _unitOfWork.Commit();

                var softStockInVM = Mapper.Map<SoftStockIn, SoftStockInViewModel>(softStockIn);

                response = request.CreateResponse(HttpStatusCode.OK, softStockInVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("updateoutstock")]
        [Authorize(Roles = "StockOutEmpty")]
        [HttpGet]
        public HttpResponseMessage UpdateOutStock(HttpRequestMessage request, int stockinId, int userId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockIn = _softStockInRepository.GetSingleById(stockinId);
                softStockIn.SupplierStatusId = "04";
                softStockIn.UpdatedDate = DateTime.Now;
                softStockIn.UpdatedBy = userId;
                _softStockInRepository.Update(softStockIn);

                var newSoftNotification = new SoftNotification();
                newSoftNotification.StockinId = softStockIn.Id;
                newSoftNotification.Message = "Đơn ĐH kho nội bộ " + softStockIn.Id + " có tồn không đủ xuất";
                newSoftNotification.Url = "branch_books";
                newSoftNotification.FromBranchId = softStockIn.FromBranchId;
                newSoftNotification.ToBranchId = softStockIn.ToBranchId;
                newSoftNotification.IsRead = false;
                newSoftNotification.Status = false;
                newSoftNotification.CreatedDate = DateTime.Now;
                _softNotificationRepository.Add(newSoftNotification);

                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, softStockIn.ToBranchId);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("loadsoldproductsbydate")]
        [HttpPost]
        public HttpResponseMessage LoadSoldProductsByDate(HttpRequestMessage request, InputShopSanPhamSoldByDate InputVM)
        {
            HttpResponseMessage response = null;
            try
            {
                List<ShopSanPhamSoldByDateViewModel> shopsanphams = new List<ShopSanPhamSoldByDateViewModel>();
                var startDate = InputVM.startDateFilter.ToLocalTime();
                var endDate = InputVM.endDateFilter.ToLocalTime();
                //var startDateConvert = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 1);
                //var endDateConvert = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 1);
                var startDateConvert = UtilExtensions.ConvertStartDate(startDate);
                var endDateConvert = UtilExtensions.ConvertStartDate(endDate);
                var soldOrders = _donhangRepository.GetMulti(x => x.BranchId == InputVM.branchId && x.CreatedDate >= startDateConvert && x.CreatedDate <= endDateConvert).ToList();
                var endDateSold = DateTime.Now;
                var startDateSold = endDate.AddDays(-30);
                startDateSold = UtilExtensions.ConvertStartDate(startDateSold);
                endDateSold = UtilExtensions.ConvertEndDate(endDateSold);
                foreach (var item in soldOrders)
                {
                    foreach (var jtem in item.donhang_ct)
                    {
                        var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == jtem.IdPro);

                        var sp = _shopSanPhamRepository.GetSingleById((int)bienthe.idsp);
                        var newItem = Mapper.Map<shop_sanpham, ShopSanPhamSoldByDateViewModel>(sp);
                        newItem.Quantity = jtem.Soluong;
                        newItem.StockTotal = _softStockRepository.GetStockTotal(newItem.id, 2);
                        newItem.AvgSoldQuantity = 0;
                        if (bienthe != null)
                        {
                            var quantityTmp = _donhangctRepository.GetMulti(x => x.donhang.ngaydat >= startDateSold && x.donhang.ngaydat <= endDateSold).Where(y => y.IdPro == bienthe.id);
                            if (quantityTmp != null)
                                newItem.AvgSoldQuantity = quantityTmp.Sum(x => x.Soluong);
                        }
                        if (shopsanphams.Count == 0)
                            shopsanphams.Add(newItem);
                        else
                        {
                            var exist = false;
                            foreach (var ztem in shopsanphams)
                            {
                                if (newItem.id == ztem.id)
                                {
                                    ztem.Quantity += jtem.Soluong;
                                    exist = true;
                                }
                            }
                            if (exist == false)
                                shopsanphams.Add(newItem);
                        }
                    }
                }

                response = request.CreateResponse(HttpStatusCode.OK, shopsanphams);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("getallcategories")]
        [HttpGet]
        public HttpResponseMessage GetAllCategories(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockInCategory = _softStockInCategoryRepository.GetAll().OrderBy(x => x.Name);
                var softStockInCategoryVm = Mapper.Map<IEnumerable<SoftStockInCategory>, IEnumerable<SoftStockInCategoryViewModel>>(softStockInCategory);
                response = request.CreateResponse(HttpStatusCode.OK, softStockInCategoryVm);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("detailstockintoaddstamp")]
        //[Authorize(Roles = "StockInDetail")]
        [HttpGet]
        public HttpResponseMessage DetailStockInToAddStamp(HttpRequestMessage request, int stockinId)
        {
            HttpResponseMessage response = null;
            List<ShopSanPhamSearchBookStampViewModel> shopSanPhamSearchBookStamps = new List<ShopSanPhamSearchBookStampViewModel>();
            try
            {
                var softStockInDetails = _softStockInDetailRepository.GetMulti(x => x.StockInId == stockinId);
                if (softStockInDetails.Count() > 0)
                {
                    foreach (var item in softStockInDetails.ToList())
                    {
                        var product = _shopSanPhamRepository.GetSingleById(item.ProductId);
                        var productVM = Mapper.Map<shop_sanpham, ShopSanPhamSearchBookStampViewModel>(product);
                        productVM.Quantity = item.Quantity.Value;
                        shopSanPhamSearchBookStamps.Add(productVM);
                    }
                    response = request.CreateResponse(HttpStatusCode.OK, shopSanPhamSearchBookStamps);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Đơn nhập kho không có sản phẩm nào!");
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("getbookdetailtoprint")]
        [HttpGet]
        public HttpResponseMessage GetBookDetailToPrint(HttpRequestMessage request, int stockinId)
        {
            HttpResponseMessage response = null;
            try
            {
                BookPrintViewModel bookPrintVm = new BookPrintViewModel();
                var stockin = _softStockInRepository.GetSingleById(stockinId);
                var supplier = _softSupplierRepository.GetSingleById(stockin.SupplierId.Value);
                var supplierVM = Mapper.Map<SoftSupplier, SoftSupplierViewModel>(supplier);
                var bookDetails = _softStockInDetailRepository.GetMulti(x => x.StockInId == stockinId);
                var bookDetailsVM = Mapper.Map<IEnumerable<SoftStockInDetail>, IEnumerable<SoftStockInDetailViewModel>>(bookDetails);
                bookPrintVm.BookDetails = bookDetailsVM;
                bookPrintVm.Supplier = supplierVM;
                response = request.CreateResponse(HttpStatusCode.OK, bookPrintVm);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("exportexcel")]
        [HttpGet]
        public HttpResponseMessage ExportExcel(HttpRequestMessage request, int stockinId)
        {
            HttpResponseMessage response = null;
            try
            {
                response = request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(GetExcelSheet(stockinId));
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Book.xlsx";
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        public byte[] GetExcelSheet(int stockinId)
        {
            BookPrintViewModel bookPrintVm = new BookPrintViewModel();
            var stockin = _softStockInRepository.GetSingleById(stockinId);
            var user = _applicationUserRepository.GetSingleById(stockin.CreatedBy.Value);
            var supplier = _softSupplierRepository.GetSingleById(stockin.SupplierId.Value);
            var bookDetails = _softStockInDetailRepository.GetMulti(x => x.StockInId == stockinId);
            var bookDetailsVM = Mapper.Map<IEnumerable<SoftStockInDetail>, IEnumerable<BookExcelViewModel>>(bookDetails);

            using (var package = new ExcelPackage())
            {
                // Tạo author cho file Excel
                package.Workbook.Properties.Author = "SoftBBM";
                // Tạo title cho file Excel
                package.Workbook.Properties.Title = "Export Book";
                // thêm tí comments vào làm màu 
                package.Workbook.Properties.Comments = "This is my generated Comments";
                // Add Sheet vào file Excel
                package.Workbook.Worksheets.Add("Books");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells["B1"].Value = "Công ty TNHH MTV Thương mại & Dịch vụ Babymart";
                worksheet.Cells["B2"].Value = "3/19 Phan Văn Sửu – P. 13 – Q. Tân Bình";
                worksheet.Cells["B3"].Value = "ĐT: 028) 7309.3479 – Email: www.babymart.vn@gmail.com";
                worksheet.Cells[1, 2, 4, 2].Style.Font.Italic = true;
                worksheet.Cells["C6"].Value = "ĐƠN ĐẶT HÀNG";
                worksheet.Cells["C6"].Style.Font.Bold = true;
                worksheet.Cells["C6"].Style.Font.Size = 20;
                worksheet.Cells["B8"].Value = "Gửi đến NCC: " + supplier.Name;
                worksheet.Cells["B9"].Value = "Địa chỉ: " + supplier.Address;
                worksheet.Cells["B10"].Value = "SĐT: " + supplier.Phone;
                worksheet.Cells["B11"].Value = "Email: " + supplier.Email;
                worksheet.Cells["B12"].Value = "Ngày tạo đơn: " + stockin.CreatedDate.Value.ToString("dd/MM/yyyy");
                worksheet.Cells["B13"].Value = "Người tạo đơn: " + user.UserName;
                worksheet.Cells["B15"].LoadFromCollection(bookDetailsVM, true, TableStyles.Dark9);
                worksheet.Cells["B15"].Value = "Mã sản phẩm";
                worksheet.Cells["C15"].Value = "Tên sản phẩm";
                worksheet.Cells["D15"].Value = "Số lượng";
                worksheet.Cells[15, 2, 15 + bookDetailsVM.Count(), 4].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        [Route("thenout")]
        [HttpPost]
        public HttpResponseMessage ThenOut(HttpRequestMessage request, SoftStockInThenOutViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var branches = _softBranchRepository.GetAll();
                var branchesVm = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<SoftBranchViewModel>>(branches);
                ThenOutViewModel thenOutVm;
                List<ThenOutViewModel> result = new List<ThenOutViewModel>();
                List<int> branchAdded = new List<int>();
                var endDate = DateTime.Now;
                var startDate = endDate.AddDays(-30);
                startDate = UtilExtensions.ConvertStartDate(startDate);
                endDate = UtilExtensions.ConvertEndDate(endDate);
                foreach (var itemProduct in softStockInVm.SoftStockInDetails)
                {
                    foreach (var itemBranch in branchesVm)
                    {
                        double stockTmp = 0;
                        var stock = _softStockRepository.GetSingleByCondition(x => x.BranchId == itemBranch.Id && x.ProductId == itemProduct.id);
                        if (stock != null)
                            stockTmp = stock.StockTotal.Value;
                        if (stockTmp == 0)
                        {
                            thenOutVm = new ThenOutViewModel();
                            thenOutVm.Products = new List<ShopSanPhamSoldByDateViewModel>();
                            var product = _shopSanPhamRepository.GetSingleById(itemProduct.id);
                            var productVm = Mapper.Map<shop_sanpham, ShopSanPhamSoldByDateViewModel>(product);
                            productVm.StockTotal = _softStockRepository.GetStockTotal(productVm.id, 2);
                            productVm.Quantity = 1;
                            productVm.AvgSoldQuantity = 0;
                            var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.idsp == itemProduct.id);
                            if (bienthe != null)
                            {
                                var quantityTmp = _donhangctRepository.GetMulti(x => x.donhang.ngaydat >= startDate && x.donhang.ngaydat <= endDate).Where(y => y.IdPro == bienthe.id);
                                if (quantityTmp != null)
                                    productVm.AvgSoldQuantity = quantityTmp.Sum(y => y.Soluong);
                            }
                            var index = branchAdded.IndexOf(itemBranch.Id);
                            if (index > -1)
                                result[index].Products.Add(productVm);
                            else
                            {
                                thenOutVm.ToBranch = itemBranch;
                                thenOutVm.Products.Add(productVm);
                                result.Add(thenOutVm);
                                branchAdded.Add(itemBranch.Id);
                            }
                        }
                    }
                }
                response = request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("getupdatedproducts")]
        [HttpPost]
        public HttpResponseMessage GetUpdatedProducts(HttpRequestMessage request, SoftStockInThenOutViewModel softStockInVm)
        {
            HttpResponseMessage response = null;
            try
            {
                List<ShopSanPhamViewModel> updatedProducts = new List<ShopSanPhamViewModel>();
                foreach (var item in softStockInVm.SoftStockInDetails)
                {
                    var product = _shopSanPhamRepository.GetSingleById(item.id);
                    if (product.PriceBase != product.PriceBaseOld)
                    {
                        var productVm = Mapper.Map<shop_sanpham, ShopSanPhamViewModel>(product);
                        updatedProducts.Add(productVm);
                    }
                }
                response = request.CreateResponse(HttpStatusCode.OK, updatedProducts);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        //[Route("getallpaymenttype")]
        //[HttpGet]
        //public HttpResponseMessage GetAllPaymentType(HttpRequestMessage request)
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        var paymentTypes = _softStockInPaymentTypeRepository.GetAll();
        //        var paymentTypesVm = Mapper.Map<IEnumerable<SoftStockInPaymentType>, IEnumerable<SoftStockInPaymentTypeViewModel>>(paymentTypes);
        //        response = request.CreateResponse(HttpStatusCode.OK, paymentTypesVm);
        //    }
        //    catch (Exception ex)
        //    {
        //        response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //    return response;
        //}

        [Route("getallpaymentstatus")]
        [HttpGet]
        public HttpResponseMessage GetAllPaymentType(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var paymentTypes = _softStockInPaymentStatusRepository.GetAll();
                var paymentTypesVm = Mapper.Map<IEnumerable<SoftStockInPaymentStatus>, IEnumerable<SoftStockInPaymentStatusViewModel>>(paymentTypes);
                response = request.CreateResponse(HttpStatusCode.OK, paymentTypesVm);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("getallpaymentmethod")]
        [HttpGet]
        public HttpResponseMessage GetAllPaymentMethod(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var paymentMethods = _softStockInPaymentMethodRepository.GetAll();
                var paymentMethodsVm = Mapper.Map<IEnumerable<SoftStockInPaymentMethod>, IEnumerable<SoftStockInPaymentMethodViewModel>>(paymentMethods);
                response = request.CreateResponse(HttpStatusCode.OK, paymentMethodsVm);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("updatepayment")]
        [Authorize(Roles = "StockInEdit")]
        [HttpPost]
        public HttpResponseMessage UpdatePayment(HttpRequestMessage request, UpdatePaymentInputViewModel updatePaymentInputVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var oldStockin = _softStockInRepository.GetSingleById(updatePaymentInputVm.stockinId);
                oldStockin.PaymentMethodId = updatePaymentInputVm.paymentMethodId;
                oldStockin.PaymentStatusId = updatePaymentInputVm.paymentStatusId;
                oldStockin.UpdatedDate = DateTime.Now;
                oldStockin.UpdatedBy = updatePaymentInputVm.updatedBy;
                if (updatePaymentInputVm.paymentStatusId == 1)
                    oldStockin.PaidDate = DateTime.Now;
                _softStockInRepository.Update(oldStockin);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("updatepaymentpopover")]
        [Authorize(Roles = "StockInEdit")]
        [HttpGet]
        public HttpResponseMessage UpdatePaymentPopover(HttpRequestMessage request, int stockinId, int paymentMethodId, int updateBy)
        {
            HttpResponseMessage response = null;
            try
            {
                var oldStockin = _softStockInRepository.GetSingleById(stockinId);
                oldStockin.PaymentMethodId = paymentMethodId;
                oldStockin.PaymentStatusId = 1;
                oldStockin.UpdatedDate = DateTime.Now;
                oldStockin.UpdatedBy = updateBy;
                oldStockin.PaidDate = DateTime.Now;
                _softStockInRepository.Update(oldStockin);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }
    }
}