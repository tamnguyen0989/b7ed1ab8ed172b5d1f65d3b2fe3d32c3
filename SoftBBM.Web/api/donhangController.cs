﻿using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Infrastructure.Core;
using AutoMapper;
using SoftBBM.Web.Enum;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using OfficeOpenXml.Style;
using System.Drawing;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using SoftBBM.Web.Common;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/order")]
    public class donhangController : ApiController
    {
        IdonhangRepository _donhangRepository;
        IdonhangctRepository _donhangctRepository;
        IkhachhangRepository _khachhangRepository;
        IshopbientheRepository _shopbientheRepository;
        ISoftStockRepository _softStockRepository;
        IdonhangchuyenphattinhRepository _donhangchuyenphattinhRepository;
        IdonhangchuyenphattpRepository _donhangchuyenphattpRepository;
        IdonhangchuyenphatvungRepository _donhangchuyenphatvungRepository;
        IApplicationUserRepository _applicationUserRepository;
        IdonhangchuyenphatdiachifutaRepository _donhangchuyenphatdiachifutaRepository;
        IApplicationUserSoftBranchRepository _applicationUserSoftBranchRepository;
        ISoftChannelRepository _softChannelRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        ISoftBranchRepository _softBranchRepository;
        IShopSanPhamLogRepository _shopSanPhamLogRepository;
        ISoftPointUpdateLogRepository _softPointUpdateLogRepository;
        IShopeeRepository _shopeeRepository;
        ISoftOfflineOrderWindowRepository _softOfflineOrderWindowRepository;
        ISystemLogRepository _systemLogRepository;
        IUnitOfWork _unitOfWork;

        public donhangController(IUnitOfWork unitOfWork,
            IdonhangRepository donhangRepository,
            IdonhangctRepository donhangctRepository,
            IkhachhangRepository khachhangRepository,
            IshopbientheRepository shopbientheRepository,
            ISoftStockRepository softStockRepository,
            IdonhangchuyenphattinhRepository donhangchuyenphattinhRepository,
            IdonhangchuyenphattpRepository donhangchuyenphattpRepository,
            IApplicationUserRepository applicationUserRepository,
            IdonhangchuyenphatvungRepository donhangchuyenphatvungRepository,
            IdonhangchuyenphatdiachifutaRepository donhangchuyenphatdiachifutaRepository,
            IApplicationUserSoftBranchRepository applicationUserSoftBranchRepository,
            ISoftChannelRepository softChannelRepository,
            IShopSanPhamRepository shopSanPhamRepository,
            ISoftChannelProductPriceRepository softChannelProductPriceRepository,
            ISoftBranchRepository softBranchRepository,
            IShopSanPhamLogRepository shopSanPhamLogRepository,
            ISoftPointUpdateLogRepository softPointUpdateLogRepository,
            IShopeeRepository shopeeRepository,
            ISoftOfflineOrderWindowRepository softOfflineOrderWindowRepository,
            ISystemLogRepository systemLogRepository)
        {
            _unitOfWork = unitOfWork;
            _donhangRepository = donhangRepository;
            _donhangctRepository = donhangctRepository;
            _khachhangRepository = khachhangRepository;
            _shopbientheRepository = shopbientheRepository;
            _softStockRepository = softStockRepository;
            _donhangchuyenphattinhRepository = donhangchuyenphattinhRepository;
            _donhangchuyenphattpRepository = donhangchuyenphattpRepository;
            _applicationUserRepository = applicationUserRepository;
            _donhangchuyenphatvungRepository = donhangchuyenphatvungRepository;
            _donhangchuyenphatdiachifutaRepository = donhangchuyenphatdiachifutaRepository;
            _softChannelRepository = softChannelRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _applicationUserSoftBranchRepository = applicationUserSoftBranchRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _softBranchRepository = softBranchRepository;
            _shopSanPhamLogRepository = shopSanPhamLogRepository;
            _softPointUpdateLogRepository = softPointUpdateLogRepository;
            _shopeeRepository = shopeeRepository;
            _softOfflineOrderWindowRepository = softOfflineOrderWindowRepository;
            _systemLogRepository = systemLogRepository;
        }

        [Route("save")]
        [Authorize(Roles = "OrderAdd")]
        [HttpPost]
        public HttpResponseMessage Save(HttpRequestMessage request, OrderViewModel orderVM)
        {
            HttpResponseMessage response = null;
            foreach (var item in orderVM.OrderDetails)
            {
                if (item.Quantity == null)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Số lượng Mã SP " + item.masp.Trim() + " lỗi!");
                    return response;
                }
            }
            try
            {
                bool flagDiscountOrder = false;
                donhang donhang = new donhang();
                donhang.Updatedonhang(orderVM);
                donhang.CreatedDate = DateTime.Now;
                var user = _applicationUserRepository.GetSingleById(orderVM.CreatedBy.Value);
                if (orderVM.ChannelId == 2)
                {
                    var datePrint = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    donhang.StatusPrint += "<li>" + user.UserName + " đã in (" + datePrint + ")</li>";
                }
                if (orderVM.Customer == null) donhang.makh = 0;
                else if (orderVM.Customer.dienthoai != null)
                {
                    var khachhang = _khachhangRepository.GetSingleByCondition(x => x.dienthoai == orderVM.Customer.dienthoai);
                    if (khachhang != null)
                    {
                        donhang.makh = khachhang.MaKH;
                        donhang.PointCusBeforeOrder = int.Parse(khachhang.diem);
                        if (orderVM.datru_diem > 0)
                        {
                            //if (int.Parse(khachhang.diem) < 1000)
                            //{
                            //    return request.CreateResponse(HttpStatusCode.BadRequest, "Khách hàng không đủ 1000 điểm để giảm giá");
                            //}
                            khachhang.diem = (int.Parse(khachhang.diem) + orderVM.diemsp.Value - 1000).ToString();
                        }
                        else
                        {
                            khachhang.diem = (int.Parse(khachhang.diem) + orderVM.diemsp.Value).ToString();
                        }
                        _khachhangRepository.Update(khachhang);
                        _unitOfWork.Commit();
                    }
                    else
                    {
                        khachhang khachhangnew = new khachhang();
                        khachhangnew.Updatekhachhang(orderVM.Customer);
                        khachhangnew.diem = orderVM.diemsp.ToString();
                        var resultkhachhangnew = _khachhangRepository.Add(khachhangnew);
                        _unitOfWork.Commit();
                        donhang.makh = resultkhachhangnew.MaKH;
                    }
                }
                else
                {
                    donhang.makh = 0;
                }
                var result = _donhangRepository.Add(donhang);
                _unitOfWork.Commit();
                foreach (var item in orderVM.OrderDetails)
                {
                    donhang_ct donhang_ct = new donhang_ct();
                    donhang_ct.Updatedonhangct(item);
                    donhang_ct.Sodh = result.id;
                    var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.idsp == item.id);
                    if (bienthe == null)
                    {
                        shop_bienthe newbt = new shop_bienthe();
                        newbt.idsp = item.id;
                        newbt.title = "default";
                        newbt.gia = 100000;
                        newbt.giasosanh = 0;
                        newbt.isdelete = false;
                        bienthe = _shopbientheRepository.Add(newbt);
                        _unitOfWork.Commit();
                    }
                    donhang_ct.IdPro = bienthe.id;
                    var priceAvg = 0;
                    var product = _shopSanPhamRepository.GetSingleById(item.id);
                    if (product != null && product.PriceAvg.HasValue)
                        priceAvg = product.PriceAvg.Value;
                    donhang_ct.PriceAvg = priceAvg;
                    donhang_ct.Dongiakm = item.PriceBeforeDiscount;
                    if (!flagDiscountOrder)
                    {
                        var memberDis = orderVM.datru_diem == null ? 0 : orderVM.datru_diem.Value;
                        var orderDis = orderVM.Discount == null ? 0 : orderVM.Discount.Value;
                        donhang_ct.TotalDiscount = memberDis + orderDis;
                        flagDiscountOrder = true;
                    }
                    else
                        donhang_ct.TotalDiscount = 0;
                    _donhangctRepository.Add(donhang_ct);

                    var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == orderVM.BranchId && x.ProductId == item.id);
                    if (stockCurrent == null)
                    {
                        var newStockCurrent = new SoftBranchProductStock();
                        newStockCurrent.BranchId = orderVM.BranchId;
                        newStockCurrent.ProductId = item.id;
                        newStockCurrent.StockTotal = 0;
                        newStockCurrent.CreatedDate = DateTime.Now;
                        newStockCurrent.CreatedBy = 0;
                        stockCurrent = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    stockCurrent.StockTotal -= item.Quantity;
                    _softStockRepository.Update(stockCurrent);

                    var channelOrder = _softChannelRepository.GetSingleById(orderVM.ChannelId.Value);
                    _unitOfWork.Commit();
                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.id;
                    productLog.Description = "Đơn hàng " + channelOrder.Name + ", mã đơn: " + result.id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = orderVM.CreatedBy;
                    productLog.CreatedDate = DateTime.Now;
                    productLog.BranchId = orderVM.BranchId;
                    productLog.StockTotal = stockCurrent.StockTotal.Value;
                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(item.id);
                    _shopSanPhamLogRepository.Add(productLog);
                }
                var donhangReturn = _donhangRepository.GetSingleById((int)donhang.id);
                var donhangReturnVM = Mapper.Map<donhang, donhangDetailViewModel>(donhangReturn);
                var khachhangPrint = _khachhangRepository.GetSingleById(donhangReturnVM.makh.Value);
                var khachhangPrintVM = Mapper.Map<khachhang, khachhangViewModel>(khachhangPrint);
                if (khachhangPrintVM.idtp.HasValue)
                {
                    var tp = _donhangchuyenphattpRepository.GetSingleById(khachhangPrintVM.idtp.Value);
                    if (tp != null)
                        khachhangPrintVM.cityName = tp.tentp;
                }
                if (khachhangPrintVM.idquan.HasValue)
                {
                    var quan = _donhangchuyenphattinhRepository.GetSingleById(khachhangPrintVM.idquan.Value);
                    if (quan != null)
                        khachhangPrintVM.districtName = quan.tentinh;
                }
                donhangReturnVM.khachhang = khachhangPrintVM;
                if (!string.IsNullOrEmpty(donhangReturnVM.tenptgh))
                {
                    var pattern = "- từ|[0-9]|- trong";
                    var ptghs = Regex.Split(donhangReturnVM.tenptgh, pattern);
                    donhangReturnVM.tenptgh = ptghs[0];
                }
                if (donhang.ChannelId == 2)
                {
                    if (donhangReturnVM.pttt != null)
                        donhangReturnVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangReturnVM.pttt.Value);
                    if (donhangReturnVM.idgiogiao != null)
                        donhangReturnVM.giogiao = UtilExtensions.ConvertDeliveryTime(donhangReturnVM.idgiogiao.Value);
                }
                if (donhang.ChannelId > 2)
                {
                    if (donhangReturnVM.pttt != null)
                        donhangReturnVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangReturnVM.pttt.Value);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, donhangReturnVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Authorize(Roles = "OrderAdd")]
        [Route("add")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, OrderViewModel orderVM)
        {
            HttpResponseMessage response = null;
            if (!(orderVM.pttt > 0))
                orderVM.pttt = 1;
            foreach (var item in orderVM.OrderDetails)
            {
                if (item.Quantity == null)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Số lượng Mã SP " + item.masp.Trim() + " lỗi!");
                    return response;
                }
            }
            try
            {
                bool flagDiscountOrder = false;
                donhang donhang = new donhang();
                donhang.Updatedonhang(orderVM);
                donhang.CreatedDate = DateTime.Now;
                if (orderVM.Customer == null) donhang.makh = 0;
                else if (orderVM.Customer.dienthoai != null && orderVM.Customer.dienthoai != "")
                {
                    var khachhang = _khachhangRepository.GetSingleByCondition(x => x.dienthoai == orderVM.Customer.dienthoai);
                    if (khachhang != null)
                    {
                        donhang.makh = khachhang.MaKH;
                        donhang.PointCusBeforeOrder = int.Parse(khachhang.diem);
                        khachhang.Updatekhachhang(orderVM.Customer, "update");
                        _unitOfWork.Commit();
                    }
                    else
                    {
                        khachhang khachhangnew = new khachhang();
                        khachhangnew.Updatekhachhang(orderVM.Customer);
                        khachhangnew.diem = orderVM.diemsp.ToString();
                        var resultkhachhangnew = _khachhangRepository.Add(khachhangnew);
                        _unitOfWork.Commit();
                        donhang.makh = resultkhachhangnew.MaKH;
                    }
                }
                else
                {
                    donhang.makh = 0;
                }
                var result = _donhangRepository.Add(donhang);
                _unitOfWork.Commit();
                var channelOrder = _softChannelRepository.GetSingleById(orderVM.ChannelId.Value);
                var branchOrder = _softBranchRepository.GetSingleById(orderVM.BranchId.Value);
                foreach (var item in orderVM.OrderDetails)
                {
                    donhang_ct donhang_ct = new donhang_ct();
                    donhang_ct.Updatedonhangct(item);
                    donhang_ct.Sodh = result.id;
                    var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == orderVM.BranchId && x.ProductId == item.id);
                    if (stockCurrent == null)
                    {
                        var newStockCurrent = new SoftBranchProductStock();
                        newStockCurrent.BranchId = orderVM.BranchId;
                        newStockCurrent.ProductId = item.id;
                        newStockCurrent.StockTotal = 0;
                        newStockCurrent.CreatedDate = DateTime.Now;
                        newStockCurrent.CreatedBy = 0;
                        stockCurrent = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    stockCurrent.StockTotal -= item.Quantity;
                    _softStockRepository.Update(stockCurrent);

                    var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.idsp == item.id);
                    if (bienthe == null)
                    {
                        shop_bienthe newbt = new shop_bienthe();
                        newbt.idsp = item.id;
                        newbt.title = "default";
                        newbt.gia = 100000;
                        newbt.giasosanh = 0;
                        newbt.isdelete = false;
                        bienthe = _shopbientheRepository.Add(newbt);
                        _unitOfWork.Commit();
                    }
                    donhang_ct.IdPro = bienthe.id;
                    var priceAvg = 0;
                    var product = _shopSanPhamRepository.GetSingleById(item.id);
                    if (product != null && product.PriceAvg.HasValue)
                        priceAvg = product.PriceAvg.Value;
                    donhang_ct.PriceAvg = priceAvg;
                    donhang_ct.Dongiakm = item.PriceBeforeDiscount;
                    if (!flagDiscountOrder)
                    {
                        var memberDis = orderVM.datru_diem == null ? 0 : orderVM.datru_diem.Value;
                        var orderDis = orderVM.Discount == null ? 0 : orderVM.Discount.Value;
                        donhang_ct.TotalDiscount = memberDis + orderDis;
                        flagDiscountOrder = true;
                    }
                    else
                        donhang_ct.TotalDiscount = 0;
                    _donhangctRepository.Add(donhang_ct);

                    _unitOfWork.Commit();
                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.id;
                    productLog.Description = "Đơn hàng " + channelOrder.Name + ", mã đơn: " + result.id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = orderVM.CreatedBy;
                    productLog.CreatedDate = DateTime.Now;
                    productLog.BranchId = orderVM.BranchId;
                    productLog.StockTotal = stockCurrent.StockTotal.Value;
                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(item.id);
                    _shopSanPhamLogRepository.Add(productLog);
                }
                var donhangReturn = _donhangRepository.GetSingleById((int)donhang.id);
                var donhangReturnVM = Mapper.Map<donhang, donhangDetailViewModel>(donhangReturn);
                var khachhangPrint = _khachhangRepository.GetSingleById(donhangReturnVM.makh.Value);
                var khachhangPrintVM = Mapper.Map<khachhang, khachhangViewModel>(khachhangPrint);
                donhangReturnVM.khachhang = khachhangPrintVM;
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, donhangReturnVM);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Authorize(Roles = "OrderList")]
        [Route("search")]
        public HttpResponseMessage Search(HttpRequestMessage request, OrderFilterViewModel orderFilterVM)
        {
            int currentPage = orderFilterVM.page.Value;
            int currentPageSize = orderFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            IEnumerable<donhang> donhangs = null;
            int totaldonhangs = 0;
            long totalMoney = 0;

            if (orderFilterVM.selectedPaymentFilters.Count > 0)
            {
                if (orderFilterVM.selectedPaymentFilters.Any(x => x == (int)PaymentMethod.Cash))
                    orderFilterVM.selectedPaymentFilters.Add(3);// thu hộ
            }

            donhangs = _donhangRepository.GetAllPaging(currentPage, currentPageSize, out totaldonhangs, out totalMoney, orderFilterVM).ToList();

            IEnumerable<donhangListViewModel> donhangsVM = Mapper.Map<IEnumerable<donhang>, IEnumerable<donhangListViewModel>>(donhangs);

            foreach (var item in donhangsVM)
            {
                if (item.makh > 0)
                {
                    var khachhang = _khachhangRepository.GetSingleById(item.makh.Value);
                    if (khachhang != null)
                    {
                        if (khachhang.idquan.HasValue)
                        {
                            var tmpDis = _donhangchuyenphattinhRepository.GetSingleById(khachhang.idquan.Value);
                            if (tmpDis != null)
                                item.District = tmpDis.tentinh;
                        }
                        if (khachhang.idtp.HasValue)
                        {
                            var tmpCity = _donhangchuyenphattpRepository.GetSingleById(khachhang.idtp.Value);
                            if (tmpCity != null)
                                item.City = tmpCity.tentp;
                        }
                    }
                }
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
                if (item.pttt != null)
                    item.tenpttt = UtilExtensions.ConvertPaymentMethod(item.pttt.Value);
            }

            PaginationSet<donhangListViewModel> pagedSet = new PaginationSet<donhangListViewModel>()
            {
                Page = currentPage,
                TotalCount = totaldonhangs,
                TotalPages = (int)Math.Ceiling((decimal)totaldonhangs / currentPageSize),
                Items = donhangsVM,
                TotalMoney = totalMoney
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [HttpGet]
        [Route("detail")]
        public HttpResponseMessage Detail(HttpRequestMessage request, int orderId)
        {
            HttpResponseMessage response = null;
            try
            {
                var donhang = _donhangRepository.GetSingleById(orderId);
                var donhangVM = Mapper.Map<donhang, donhangDetailViewModel>(donhang);
                if (donhangVM.khachhang.idtp.HasValue)
                {
                    var tp = _donhangchuyenphattpRepository.GetSingleById(donhangVM.khachhang.idtp.Value);
                    if (tp != null)
                        donhangVM.tentp = tp.tentp;
                }
                if (donhangVM.khachhang.idquan.HasValue)
                {
                    var quan = _donhangchuyenphattinhRepository.GetSingleById(donhangVM.khachhang.idquan.Value);
                    if (quan != null)
                        donhangVM.tenquan = quan.tentinh;
                }
                if (donhang.ChannelId == 2)
                {
                    if (donhangVM.pttt != null)
                        donhangVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangVM.pttt.Value);
                    if (donhangVM.idgiogiao != null)
                        donhangVM.giogiao = UtilExtensions.ConvertDeliveryTime(donhangVM.idgiogiao.Value);
                }
                if (donhang.ChannelId != 2)
                {
                    if (donhangVM.pttt != null)
                        donhangVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangVM.pttt.Value);
                }
                response = request.CreateResponse(HttpStatusCode.OK, donhangVM);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpGet]
        [Route("detailtoprint")]
        public HttpResponseMessage DetailToPrint(HttpRequestMessage request, int orderId, string username)
        {
            HttpResponseMessage response = null;
            try
            {
                var donhang = _donhangRepository.GetSingleById(orderId);
                var datePrint = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                donhang.StatusPrint += "<li>" + username + " đã in (" + datePrint + ")</li>";
                _donhangRepository.Update(donhang);
                _unitOfWork.Commit();
                var donhangVM = Mapper.Map<donhang, donhangDetailViewModel>(donhang);
                if (donhangVM.khachhang.idtp.HasValue)
                {
                    var tp = _donhangchuyenphattpRepository.GetSingleById(donhangVM.khachhang.idtp.Value);
                    if (tp != null)
                        donhangVM.tentp = tp.tentp;
                }
                if (donhangVM.khachhang.idquan.HasValue)
                {
                    var quan = _donhangchuyenphattinhRepository.GetSingleById(donhangVM.khachhang.idquan.Value);
                    if (quan != null)
                        donhangVM.tenquan = quan.tentinh;
                }
                if (donhang.ChannelId == 2)
                {
                    if (donhangVM.pttt != null)
                        donhangVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangVM.pttt.Value);
                    if (donhangVM.idgiogiao != null)
                        donhangVM.giogiao = UtilExtensions.ConvertDeliveryTime(donhangVM.idgiogiao.Value);
                }
                if (donhang.ChannelId != 2)
                {
                    if (donhangVM.pttt != null)
                        donhangVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangVM.pttt.Value);
                }
                if (!string.IsNullOrEmpty(donhangVM.tenptgh))
                {
                    var pattern = "- từ|[0-9]|- trong";
                    var ptghs = Regex.Split(donhangVM.tenptgh, pattern);
                    donhangVM.tenptgh = ptghs[0];
                }
                response = request.CreateResponse(HttpStatusCode.OK, donhangVM);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updateshipped")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateShipped(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var oldOrder = _donhangRepository.GetSingleById((int)donhangVM.id);

                if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                if (oldOrder.ChannelId == 1)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                    return response;
                }
                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == donhangVM.UserId && x.BranchId == oldOrder.BranchId);
                if (userBranch != null)
                {
                    var donhang = _donhangRepository.GetSingleById((int)donhangVM.id);
                    donhang.ghichu = donhangVM.ghichu;
                    if (donhangVM.ShipperId > 0)
                        donhang.ShipperId = donhangVM.ShipperId;
                    else
                        donhang.ShipperId = null;
                    donhang.Status = 2;
                    donhang.UpdatedBy = donhangVM.UserId;
                    donhang.UpdatedDate = DateTime.Now;
                    _donhangRepository.Update(donhang);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updaterefund")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateRefund(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var oldOrder = _donhangRepository.GetSingleById((int)donhangVM.id);

                if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                if (oldOrder.ChannelId == 1)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                    return response;
                }
                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == donhangVM.UserId && x.BranchId == oldOrder.BranchId);
                if (userBranch != null)
                {
                    var donhang = _donhangRepository.GetSingleById((int)donhangVM.id);
                    donhang.ghichu = donhangVM.ghichu;
                    if (donhangVM.ShipperId > 0)
                        donhang.ShipperId = donhangVM.ShipperId;
                    else
                        donhang.ShipperId = null;
                    donhang.Status = 5;
                    donhang.UpdatedBy = donhangVM.UserId;
                    donhang.UpdatedDate = DateTime.Now;
                    _donhangRepository.Update(donhang);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updateprocess")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateProcess(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var oldOrder = _donhangRepository.GetSingleById((int)donhangVM.id);

                if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                if (oldOrder.ChannelId == 1)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                    return response;
                }
                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == donhangVM.UserId && x.BranchId == oldOrder.BranchId);
                if (userBranch != null)
                {
                    var donhang = _donhangRepository.GetSingleById((int)donhangVM.id);
                    donhang.ghichu = donhangVM.ghichu;
                    if (donhangVM.ShipperId > 0)
                        donhang.ShipperId = donhangVM.ShipperId;
                    else
                        donhang.ShipperId = null;
                    donhang.Status = 1;
                    donhang.UpdatedBy = donhangVM.UserId;
                    donhang.UpdatedDate = DateTime.Now;
                    _donhangRepository.Update(donhang);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updatedone")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateDone(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;

            try
            {
                var oldOrder = _donhangRepository.GetSingleById((int)donhangVM.id);

                if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                if (oldOrder.ChannelId == 1)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                    return response;
                }
                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == donhangVM.UserId && x.BranchId == oldOrder.BranchId);
                if (userBranch != null)
                {
                    oldOrder.ghichu = donhangVM.ghichu;
                    if (donhangVM.ShipperId > 0)
                        oldOrder.ShipperId = donhangVM.ShipperId;
                    else
                        oldOrder.ShipperId = null;
                    oldOrder.Status = 3;
                    oldOrder.UpdatedBy = donhangVM.UserId;
                    oldOrder.UpdatedDate = DateTime.Now;
                    _donhangRepository.Update(oldOrder);
                    _unitOfWork.Commit();

                    //if (oldOrder.ChannelId != 2)
                    //{
                    //    if (donhangVM.makh > 0)
                    //    {
                    //        var khachhang = _khachhangRepository.GetSingleById(donhangVM.makh.Value);
                    //        if (oldOrder.datru_diem > 0)
                    //        {
                    //            if (int.Parse(khachhang.diem) < 1000)
                    //            {
                    //                return request.CreateResponse(HttpStatusCode.BadRequest, "Khách hàng không đủ 1000 điểm để giảm giá");
                    //            }
                    //            khachhang.diem = (int.Parse(khachhang.diem) + oldOrder.diemsp.Value - 1000).ToString();
                    //        }
                    //        else
                    //        {
                    //            khachhang.diem = (int.Parse(khachhang.diem) + oldOrder.diemsp.Value).ToString();
                    //        }
                    //        _khachhangRepository.Update(khachhang);
                    //        _unitOfWork.Commit();
                    //    }

                    //    foreach (var item in oldOrder.donhang_ct)
                    //    {
                    //        var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                    //        var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == oldOrder.BranchId && x.ProductId == bienthe.idsp);
                    //        if (stockCurrent == null)
                    //        {
                    //            var newStockCurrent = new SoftBranchProductStock();
                    //            newStockCurrent.BranchId = oldOrder.BranchId;
                    //            newStockCurrent.ProductId = bienthe.idsp;
                    //            newStockCurrent.StockTotal = 0;
                    //            newStockCurrent.CreatedDate = DateTime.Now;
                    //            newStockCurrent.CreatedBy = 0;
                    //            stockCurrent = _softStockRepository.Add(newStockCurrent);
                    //            _unitOfWork.Commit();
                    //        }
                    //        stockCurrent.StockTotal -= item.Soluong;
                    //        _softStockRepository.Update(stockCurrent);
                    //    }
                    //    _unitOfWork.Commit();
                    //}

                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updatecancel")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateCancel(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var donhang = _donhangRepository.GetSingleById((int)donhangVM.id);
                var channelOrder = _softChannelRepository.GetSingleById(donhang.ChannelId.Value);

                if (donhang.Status == 3 || donhang.Status == 4 || donhang.Status == 6)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                if (donhang.ChannelId == 1)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                    return response;
                }

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == donhangVM.UserId && x.BranchId == donhang.BranchId);
                if (userBranch != null)
                {
                    donhang.ghichu = donhangVM.ghichu;
                    if (donhangVM.ShipperId > 0)
                        donhang.ShipperId = donhangVM.ShipperId;
                    else
                        donhang.ShipperId = null;
                    donhang.Status = 4;
                    donhang.UpdatedBy = donhangVM.UserId;
                    donhang.UpdatedDate = DateTime.Now;
                    _donhangRepository.Update(donhang);
                    _unitOfWork.Commit();

                    if (donhang.pttt != (int)PaymentMethod.OnlinePayment || (donhang.pttt == (int)PaymentMethod.OnlinePayment && donhang.tinhtrang == "00"))
                    {
                        if (donhang.makh > 0)
                        {
                            var khachhang = _khachhangRepository.GetSingleById(donhang.makh.Value);
                            if (donhang.datru_diem > 0)
                            {
                                khachhang.diem = (int.Parse(khachhang.diem) - donhang.diemsp.Value + 1000).ToString();
                            }
                            else
                            {
                                khachhang.diem = (int.Parse(khachhang.diem) - donhang.diemsp.Value).ToString();
                            }
                            _khachhangRepository.Update(khachhang);
                            _unitOfWork.Commit();
                        }

                        foreach (var item in donhang.donhang_ct)
                        {
                            var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                            var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == donhang.BranchId && x.ProductId == bienthe.idsp);
                            if (stockCurrent == null)
                            {
                                var newStockCurrent = new SoftBranchProductStock();
                                newStockCurrent.BranchId = donhang.BranchId;
                                newStockCurrent.ProductId = bienthe.idsp;
                                newStockCurrent.StockTotal = 0;
                                newStockCurrent.CreatedDate = DateTime.Now;
                                newStockCurrent.CreatedBy = 0;
                                stockCurrent = _softStockRepository.Add(newStockCurrent);
                                _unitOfWork.Commit();
                            }
                            stockCurrent.StockTotal += item.Soluong;
                            _softStockRepository.Update(stockCurrent);

                            _unitOfWork.Commit();
                            shop_sanphamLogs productLog = new shop_sanphamLogs();
                            productLog.ProductId = bienthe.idsp;
                            productLog.Description = "Cập nhật huỷ đơn hàng " + channelOrder.Name + ", mã đơn: " + donhang.id;
                            productLog.Quantity = item.Soluong;
                            productLog.CreatedBy = donhangVM.UserId;
                            productLog.CreatedDate = DateTime.Now;
                            productLog.BranchId = donhang.BranchId;
                            productLog.StockTotal = stockCurrent.StockTotal.Value;
                            productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value) ;
                            _shopSanPhamLogRepository.Add(productLog);
                        }
                        _unitOfWork.Commit();
                    }
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updateshipcancel")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateShipCancel(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var donhang = _donhangRepository.GetSingleById((int)donhangVM.id);
                var channelOrder = _softChannelRepository.GetSingleById(donhang.ChannelId.Value);

                if (donhang.Status == 3 || donhang.Status == 4 || donhang.Status == 6)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                if (donhang.ChannelId == 1)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                    return response;
                }

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == donhangVM.UserId && x.BranchId == donhang.BranchId);
                if (userBranch != null)
                {
                    donhang.ghichu = donhangVM.ghichu;
                    if (donhangVM.ShipperId > 0)
                        donhang.ShipperId = donhangVM.ShipperId;
                    else
                        donhang.ShipperId = null;
                    donhang.Status = 6;


                    if (donhang.pttt != (int)PaymentMethod.OnlinePayment || (donhang.pttt == (int)PaymentMethod.OnlinePayment && donhang.tinhtrang == "00"))
                    {
                        if (donhang.makh > 0)
                        {
                            var khachhang = _khachhangRepository.GetSingleById(donhang.makh.Value);
                            if (donhang.datru_diem > 0)
                            {
                                khachhang.diem = (int.Parse(khachhang.diem) - donhang.diemsp.Value + 1000).ToString();
                            }
                            else
                            {
                                khachhang.diem = (int.Parse(khachhang.diem) - donhang.diemsp.Value).ToString();
                            }
                            _khachhangRepository.Update(khachhang);
                            _unitOfWork.Commit();
                        }

                        foreach (var item in donhang.donhang_ct)
                        {
                            var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                            var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == donhang.BranchId && x.ProductId == bienthe.idsp);
                            if (stockCurrent == null)
                            {
                                var newStockCurrent = new SoftBranchProductStock();
                                newStockCurrent.BranchId = donhang.BranchId;
                                newStockCurrent.ProductId = bienthe.idsp;
                                newStockCurrent.StockTotal = 0;
                                newStockCurrent.CreatedDate = DateTime.Now;
                                newStockCurrent.CreatedBy = 0;
                                stockCurrent = _softStockRepository.Add(newStockCurrent);
                                _unitOfWork.Commit();
                            }
                            stockCurrent.StockTotal += item.Soluong;
                            _softStockRepository.Update(stockCurrent);

                            _unitOfWork.Commit();
                            shop_sanphamLogs productLog = new shop_sanphamLogs();
                            productLog.ProductId = bienthe.idsp;
                            productLog.Description = "Cập nhật gh thất bại đơn hàng " + channelOrder.Name + ", mã đơn: " + donhang.id;
                            productLog.Quantity = item.Soluong;
                            productLog.CreatedBy = donhangVM.UserId;
                            productLog.CreatedDate = DateTime.Now;
                            productLog.BranchId = donhang.BranchId;
                            productLog.StockTotal = stockCurrent.StockTotal.Value;
                            productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value);
                            _shopSanPhamLogRepository.Add(productLog);
                        }
                        _unitOfWork.Commit();
                    }
                    donhang.UpdatedBy = donhangVM.UserId;
                    donhang.UpdatedDate = DateTime.Now;
                    _donhangRepository.Update(donhang);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("update")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage Update(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var donhang = _donhangRepository.GetSingleById((int)donhangVM.id);
                donhang.ghichu = donhangVM.ghichu;
                if (donhangVM.ShipperId > 0)
                    donhang.ShipperId = donhangVM.ShipperId;
                else
                    donhang.ShipperId = null;
                donhang.UpdatedBy = donhangVM.UserId;
                donhang.UpdatedDate = DateTime.Now;
                _donhangRepository.Update(donhang);
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

        [HttpGet]
        [Route("getfeeregioncode")]
        public HttpResponseMessage GetFeeVung(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var feeVung = _donhangchuyenphatvungRepository.GetAll();
                var feeVungVm = Mapper.Map<IEnumerable<donhang_chuyenphat_vung>, IEnumerable<donhangchuyenphatvungViewModel>>(feeVung);
                response = request.CreateResponse(HttpStatusCode.OK, feeVungVm);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpGet]
        [Route("getfutaaddress")]
        public HttpResponseMessage GetFutaAddress(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var futaAddresses = _donhangchuyenphatdiachifutaRepository.GetAll();
                var futaAddressesVm = Mapper.Map<IEnumerable<donhang_chuyenphat_danhsachdiachifuta>, IEnumerable<donhangchuyenphatdiachifutaViewModel>>(futaAddresses);
                response = request.CreateResponse(HttpStatusCode.OK, futaAddressesVm);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updateprocessorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateProcessOrders(HttpRequestMessage request, StatusOrdersViewModel statusOrdersViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var orderVM in statusOrdersViewModel.Orders)
                {
                    var checkOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                    if (checkOrder.Status == 3 || checkOrder.Status == 4 || checkOrder.Status == 6)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Tình trạng đơn " + checkOrder.id.ToString() + " đã được cập nhật!");
                        return response;
                    }
                    if (checkOrder.ChannelId == 1)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                        return response;
                    }
                }

                var oldOrder = _donhangRepository.GetSingleById((int)statusOrdersViewModel.Orders.ToList()[0].id);

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == statusOrdersViewModel.UserId && x.BranchId == oldOrder.BranchId);
                if (userBranch != null)
                {
                    foreach (var orderVM in statusOrdersViewModel.Orders)
                    {
                        var updateOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        updateOrder.Status = 1;
                        updateOrder.UpdatedBy = statusOrdersViewModel.UserId;
                        updateOrder.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(updateOrder);
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updateshippedorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateShippedOrders(HttpRequestMessage request, StatusOrdersViewModel statusOrdersViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var orderVM in statusOrdersViewModel.Orders)
                {
                    var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                    if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Tình trạng đơn " + oldOrder.id.ToString() + " đã được cập nhật!");
                        return response;
                    }
                    if (oldOrder.ChannelId == 1)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                        return response;
                    }
                }

                var checkOrder = _donhangRepository.GetSingleById((int)statusOrdersViewModel.Orders.ToList()[0].id);

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == statusOrdersViewModel.UserId && x.BranchId == checkOrder.BranchId);
                if (userBranch != null)
                {
                    foreach (var orderVM in statusOrdersViewModel.Orders)
                    {
                        var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        oldOrder.Status = 2;
                        oldOrder.UpdatedBy = statusOrdersViewModel.UserId;
                        oldOrder.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(oldOrder);
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updaterefundorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateRefundOrders(HttpRequestMessage request, StatusOrdersViewModel statusOrdersViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var orderVM in statusOrdersViewModel.Orders)
                {
                    var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                    if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Tình trạng đơn " + oldOrder.id.ToString() + " đã được cập nhật!");
                        return response;
                    }
                    if (oldOrder.ChannelId == 1)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                        return response;
                    }
                }

                var checkOrder = _donhangRepository.GetSingleById((int)statusOrdersViewModel.Orders.ToList()[0].id);

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == statusOrdersViewModel.UserId && x.BranchId == checkOrder.BranchId);
                if (userBranch != null)
                {
                    foreach (var orderVM in statusOrdersViewModel.Orders)
                    {
                        var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        oldOrder.Status = 5;
                        oldOrder.UpdatedBy = statusOrdersViewModel.UserId;
                        oldOrder.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(oldOrder);
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;


            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updatedoneorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateDoneOrders(HttpRequestMessage request, StatusOrdersViewModel statusOrdersViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var orderVM in statusOrdersViewModel.Orders)
                {
                    var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                    if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Tình trạng đơn " + oldOrder.id.ToString() + " đã được cập nhật!");
                        return response;
                    }
                    if (oldOrder.ChannelId == 1)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                        return response;
                    }
                }


                var checkOrder = _donhangRepository.GetSingleById((int)statusOrdersViewModel.Orders.ToList()[0].id);

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == statusOrdersViewModel.UserId && x.BranchId == checkOrder.BranchId);
                if (userBranch != null)
                {
                    foreach (var orderVM in statusOrdersViewModel.Orders)
                    {
                        var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        oldOrder.Status = 3;
                        oldOrder.UpdatedBy = statusOrdersViewModel.UserId;
                        oldOrder.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(oldOrder);
                        _unitOfWork.Commit();

                        if (oldOrder.ChannelId != 2)
                        {
                            if (oldOrder.makh > 0)
                            {
                                var khachhang = _khachhangRepository.GetSingleById(oldOrder.makh.Value);
                                if (oldOrder.datru_diem > 0)
                                {
                                    if (int.Parse(khachhang.diem) < 1000)
                                    {
                                        return request.CreateResponse(HttpStatusCode.BadRequest, "Khách hàng không đủ 1000 điểm để giảm giá");
                                    }
                                    khachhang.diem = (int.Parse(khachhang.diem) + oldOrder.diemsp.Value - 1000).ToString();
                                }
                                else
                                {
                                    khachhang.diem = (int.Parse(khachhang.diem) + oldOrder.diemsp.Value).ToString();
                                }
                                _khachhangRepository.Update(khachhang);
                                _unitOfWork.Commit();
                            }

                            foreach (var item in oldOrder.donhang_ct)
                            {
                                var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                                var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == oldOrder.BranchId && x.ProductId == bienthe.idsp);
                                if (stockCurrent == null)
                                {
                                    var newStockCurrent = new SoftBranchProductStock();
                                    newStockCurrent.BranchId = oldOrder.BranchId;
                                    newStockCurrent.ProductId = bienthe.idsp;
                                    newStockCurrent.StockTotal = 0;
                                    newStockCurrent.CreatedDate = DateTime.Now;
                                    newStockCurrent.CreatedBy = 0;
                                    stockCurrent = _softStockRepository.Add(newStockCurrent);
                                    _unitOfWork.Commit();
                                }
                                stockCurrent.StockTotal -= item.Soluong;
                                _softStockRepository.Update(stockCurrent);
                            }
                        }
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;


            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updatecancelorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateCancelOrders(HttpRequestMessage request, StatusOrdersViewModel statusOrdersViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var orderVM in statusOrdersViewModel.Orders)
                {
                    var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                    if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Tình trạng đơn " + oldOrder.id.ToString() + " đã được cập nhật!");
                        return response;
                    }
                    if (oldOrder.ChannelId == 1)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                        return response;
                    }
                }

                var checkOrder = _donhangRepository.GetSingleById((int)statusOrdersViewModel.Orders.ToList()[0].id);

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == statusOrdersViewModel.UserId && x.BranchId == checkOrder.BranchId);
                if (userBranch != null)
                {
                    foreach (var orderVM in statusOrdersViewModel.Orders)
                    {
                        var channelOrder = _softChannelRepository.GetSingleById(orderVM.ChannelId.Value);
                        var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        oldOrder.Status = 4;
                        oldOrder.UpdatedBy = statusOrdersViewModel.UserId;
                        oldOrder.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(oldOrder);
                        _unitOfWork.Commit();

                        if (oldOrder.pttt != (int)PaymentMethod.OnlinePayment || (oldOrder.pttt == (int)PaymentMethod.OnlinePayment && oldOrder.tinhtrang == "00"))
                        {
                            //nếu có makh và không phải đơn từ shopee
                            if (oldOrder.makh > 0 && oldOrder.ChannelId != (int)ChannelEnum.SPE)
                            {
                                var khachhang = _khachhangRepository.GetSingleById(oldOrder.makh.Value);
                                if (oldOrder.datru_diem > 0)
                                {
                                    khachhang.diem = (int.Parse(khachhang.diem) - oldOrder.diemsp.Value + 1000).ToString();
                                }
                                else
                                {
                                    khachhang.diem = (int.Parse(khachhang.diem) - oldOrder.diemsp.Value).ToString();
                                }
                                _khachhangRepository.Update(khachhang);
                                _unitOfWork.Commit();
                            }

                            foreach (var item in oldOrder.donhang_ct)
                            {
                                var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                                if (bienthe != null)
                                {
                                    var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == oldOrder.BranchId && x.ProductId == bienthe.idsp);
                                    if (stockCurrent == null)
                                    {
                                        var newStockCurrent = _softStockRepository.Init(oldOrder.BranchId.Value, bienthe.idsp.Value);
                                        stockCurrent = _softStockRepository.Add(newStockCurrent);
                                        _unitOfWork.Commit();
                                    }
                                    stockCurrent.StockTotal += item.Soluong;
                                    _softStockRepository.Update(stockCurrent);

                                    _unitOfWork.Commit();
                                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                                    productLog.ProductId = bienthe.idsp;
                                    productLog.Description = "Cập nhật huỷ đơn hàng " + channelOrder.Name + ", mã đơn: " + oldOrder.id;
                                    productLog.Quantity = item.Soluong;
                                    productLog.CreatedBy = statusOrdersViewModel.UserId;
                                    productLog.CreatedDate = DateTime.Now;
                                    productLog.BranchId = oldOrder.BranchId;
                                    productLog.StockTotal = stockCurrent.StockTotal.Value;
                                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value);
                                    _shopSanPhamLogRepository.Add(productLog);
                                }
                            }
                            _unitOfWork.Commit();
                        }
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updateshipcancelorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateShipCancelOrders(HttpRequestMessage request, StatusOrdersViewModel statusOrdersViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var orderVM in statusOrdersViewModel.Orders)
                {
                    var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                    if (oldOrder.Status == 3 || oldOrder.Status == 4 || oldOrder.Status == 6)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Tình trạng đơn " + oldOrder.id.ToString() + " đã được cập nhật!");
                        return response;
                    }
                    if (oldOrder.ChannelId == 1)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                        return response;
                    }
                }

                var checkOrder = _donhangRepository.GetSingleById((int)statusOrdersViewModel.Orders.ToList()[0].id);

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == statusOrdersViewModel.UserId && x.BranchId == checkOrder.BranchId);
                if (userBranch != null)
                {
                    foreach (var orderVM in statusOrdersViewModel.Orders)
                    {
                        var channelOrder = _softChannelRepository.GetSingleById(orderVM.ChannelId.Value);
                        var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        oldOrder.Status = 6;
                        oldOrder.UpdatedBy = statusOrdersViewModel.UserId;
                        oldOrder.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(oldOrder);
                        _unitOfWork.Commit();

                        if (oldOrder.pttt != (int)PaymentMethod.OnlinePayment || (oldOrder.pttt == (int)PaymentMethod.OnlinePayment && oldOrder.tinhtrang == "00"))
                        {
                            if (oldOrder.makh > 0)
                            {
                                var khachhang = _khachhangRepository.GetSingleById(oldOrder.makh.Value);
                                if (oldOrder.datru_diem > 0)
                                {
                                    khachhang.diem = (int.Parse(khachhang.diem) - oldOrder.diemsp.Value + 1000).ToString();
                                }
                                else
                                {
                                    khachhang.diem = (int.Parse(khachhang.diem) - oldOrder.diemsp.Value).ToString();
                                }
                                _khachhangRepository.Update(khachhang);
                                _unitOfWork.Commit();
                            }

                            foreach (var item in oldOrder.donhang_ct)
                            {
                                var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                                var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == oldOrder.BranchId && x.ProductId == bienthe.idsp);
                                if (stockCurrent == null)
                                {
                                    var newStockCurrent = _softStockRepository.Init(oldOrder.BranchId.Value, bienthe.idsp.Value);
                                    stockCurrent = _softStockRepository.Add(newStockCurrent);
                                    _unitOfWork.Commit();
                                }
                                stockCurrent.StockTotal += item.Soluong;
                                _softStockRepository.Update(stockCurrent);

                                _unitOfWork.Commit();
                                shop_sanphamLogs productLog = new shop_sanphamLogs();
                                productLog.ProductId = bienthe.idsp;
                                productLog.Description = "Cập nhật gh thất bại đơn hàng " + channelOrder.Name + ", mã đơn: " + oldOrder.id;
                                productLog.Quantity = item.Soluong;
                                productLog.CreatedBy = statusOrdersViewModel.UserId;
                                productLog.CreatedDate = DateTime.Now;
                                productLog.BranchId = oldOrder.BranchId;
                                productLog.StockTotal = stockCurrent.StockTotal.Value;
                                productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value);
                                _shopSanPhamLogRepository.Add(productLog);
                            }
                            _unitOfWork.Commit();
                        }
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;


            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updateshipperorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateShipperOrders(HttpRequestMessage request, ShipperOrdersViewModel shipperOrdersViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var orderVM in shipperOrdersViewModel.Orders)
                {
                    var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id && (x.Status == 3 || x.Status == 4 || x.Status == 6));
                    if (oldOrder != null)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Đơn " + oldOrder.id.ToString() + " đã được cập nhật!");
                        return response;
                    }
                }

                var checkOrder = _donhangRepository.GetSingleById((int)shipperOrdersViewModel.Orders.ToList()[0].id);

                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == shipperOrdersViewModel.UserId && x.BranchId == checkOrder.BranchId);
                if (userBranch != null)
                {
                    foreach (var orderVM in shipperOrdersViewModel.Orders)
                    {
                        var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        oldOrder.ShipperId = shipperOrdersViewModel.ShipperId;
                        oldOrder.UpdatedBy = shipperOrdersViewModel.UserId;
                        oldOrder.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(oldOrder);
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;


            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("updatecancelforedit")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateCancelForEdit(HttpRequestMessage request, donhangUpdateViewModel donhangVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var donhang = _donhangRepository.GetSingleById((int)donhangVM.id);

                if (donhang.Status == 3 || donhang.Status == 4 || donhang.Status == 6)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi, Đơn đã được cập nhật !");
                    return response;
                }
                if (donhang.ChannelId == 1)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found !");
                    return response;
                }
                var userBranch = _applicationUserSoftBranchRepository.GetSingleByCondition(x => x.UserId == donhangVM.UserId && x.BranchId == donhang.BranchId);
                if (userBranch != null)
                {
                    khachhangViewModel khachhangVM = new khachhangViewModel();
                    var channel = _softChannelRepository.GetSingleById(donhang.ChannelId.Value);
                    var channelVM = Mapper.Map<SoftChannel, SoftChannelViewModel>(channel);
                    //if (donhang.makh.Value > 0)
                    //{
                    //    var khachhangModel = _khachhangRepository.GetSingleById(donhang.makh.Value);
                    //    khachhangVM = Mapper.Map<khachhang, khachhangViewModel>(khachhangModel);
                    //}

                    List<SoftOrderDetailViewModel> orderDetails = new List<SoftOrderDetailViewModel>();
                    foreach (var item in donhang.donhang_ct)
                    {
                        var bienthe = _shopbientheRepository.GetSingleById((int)item.IdPro);
                        var shopSanPham = _shopSanPhamRepository.GetSingleById(bienthe.idsp.Value);
                        var shopSanPhamVM = Mapper.Map<shop_sanpham, SoftOrderDetailViewModel>(shopSanPham);
                        shopSanPhamVM.Quantity = item.Soluong;
                        var model = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == bienthe.idsp.Value && x.ChannelId == channelVM.Id);
                        if (model != null)
                        {
                            var softChannnelProductPrice = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == bienthe.idsp.Value && x.ChannelId == channelVM.Id);
                            if (softChannnelProductPrice.PriceDiscount > 0)
                            {
                                shopSanPhamVM.Price = softChannnelProductPrice.PriceDiscount;
                                shopSanPhamVM.PriceBeforeDiscount = softChannnelProductPrice.Price;
                            }
                            else
                            {
                                shopSanPhamVM.Price = softChannnelProductPrice.Price;
                                shopSanPhamVM.PriceBeforeDiscount = 0;
                            }
                        }
                        else if (donhang.ChannelId == (int)ChannelEnum.BSI)
                        {
                            shopSanPhamVM.Price = shopSanPhamVM.PriceWholesale;
                            shopSanPhamVM.PriceBeforeDiscount = 0;
                        }

                        orderDetails.Add(shopSanPhamVM);
                    }

                    donhangAfterEditViewModel donhangAfterEdit = new donhangAfterEditViewModel();
                    donhangAfterEdit.channel = channelVM;
                    //donhangAfterEdit.customer = khachhangVM;
                    donhangAfterEdit.orderDetails = orderDetails;
                    //donhangAfterEdit.shipperId = donhang.ShipperId;

                    var historyOrder = "Thông tin đơn cũ:<br>";
                    if (donhang.khachhang.idtp.HasValue)
                    {
                        var tp = _donhangchuyenphattpRepository.GetSingleById(donhang.khachhang.idtp.Value);
                        historyOrder += "TP: " + tp.tentp;
                    }
                    if (donhang.khachhang.idquan.HasValue)
                    {
                        var quan = _donhangchuyenphattinhRepository.GetSingleById(donhang.khachhang.idquan.Value);
                        historyOrder += ", Quận: " + quan.tentinh;
                    }
                    historyOrder += "<br>";
                    if (!string.IsNullOrEmpty(donhang.tenptgh))
                        historyOrder += "PTGH: " + donhang.tenptgh + "<br>";
                    if (donhang.pttt != null)
                        historyOrder += "PTTT: " + UtilExtensions.ConvertPaymentMethod(donhang.pttt.Value);
                    if (donhang.ShipperId > 0)
                    {
                        var shipper = _applicationUserRepository.GetSingleById(donhang.ShipperId.Value);
                        historyOrder += ", NVGH: " + shipper.UserName;
                    }
                    historyOrder += "<br>";
                    if (donhang.idgiogiao != null)
                        historyOrder += "TG: " + UtilExtensions.ConvertDeliveryTime(donhang.idgiogiao.Value) + "<br>";
                    donhangAfterEdit.historyOrder = historyOrder;

                    donhang.ghichu = donhangVM.ghichu;
                    if (donhangVM.ShipperId > 0)
                        donhang.ShipperId = donhangVM.ShipperId;
                    else
                        donhang.ShipperId = null;
                    donhang.Status = 4;
                    donhang.UpdatedBy = donhangVM.UserId;
                    donhang.UpdatedDate = DateTime.Now;
                    _donhangRepository.Update(donhang);
                    _unitOfWork.Commit();

                    if (donhang.pttt != (int)PaymentMethod.OnlinePayment || (donhang.pttt == (int)PaymentMethod.OnlinePayment && donhang.tinhtrang == "00"))
                    {
                        if (donhang.makh > 0)
                        {
                            var khachhang = _khachhangRepository.GetSingleById(donhang.makh.Value);
                            if (donhang.datru_diem > 0)
                            {
                                khachhang.diem = (int.Parse(khachhang.diem) - donhang.diemsp.Value + 1000).ToString();
                            }
                            else
                            {
                                khachhang.diem = (int.Parse(khachhang.diem) - donhang.diemsp.Value).ToString();
                            }
                            _khachhangRepository.Update(khachhang);
                            _unitOfWork.Commit();
                            khachhangVM = Mapper.Map<khachhang, khachhangViewModel>(khachhang);
                            donhangAfterEdit.customer = khachhangVM;
                        }

                        foreach (var item in donhang.donhang_ct)
                        {
                            var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                            var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == donhang.BranchId && x.ProductId == bienthe.idsp);
                            if (stockCurrent == null)
                            {
                                var newStockCurrent = _softStockRepository.Init(donhang.BranchId.Value, bienthe.idsp.Value);
                                stockCurrent = _softStockRepository.Add(newStockCurrent);
                                _unitOfWork.Commit();
                            }
                            stockCurrent.StockTotal += item.Soluong;
                            _softStockRepository.Update(stockCurrent);

                            var channelOrder = _softChannelRepository.GetSingleById(donhang.ChannelId.Value);
                            _unitOfWork.Commit();
                            shop_sanphamLogs productLog = new shop_sanphamLogs();
                            productLog.ProductId = bienthe.idsp;
                            productLog.Description = "Cập nhật chỉnh sửa đơn hàng " + channelOrder.Name + ", mã đơn: " + donhang.id;
                            productLog.Quantity = item.Soluong;
                            productLog.CreatedBy = donhangVM.UserId;
                            productLog.CreatedDate = DateTime.Now;
                            productLog.BranchId = donhang.BranchId;
                            productLog.StockTotal = stockCurrent.StockTotal.Value;
                            productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value);
                            _shopSanPhamLogRepository.Add(productLog);
                        }
                        _unitOfWork.Commit();
                    }

                    response = request.CreateResponse(HttpStatusCode.OK, donhangAfterEdit);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("detailorderstoprint")]
        public HttpResponseMessage DetailOrdersToPrint(HttpRequestMessage request, OrdersToPrintInputViewModel orders)
        {
            HttpResponseMessage response = null;
            try
            {
                List<donhangDetailViewModel> donhangVMs = new List<donhangDetailViewModel>();
                foreach (var item in orders.orderIds)
                {
                    var donhang = _donhangRepository.GetSingleById(item);
                    var datePrint = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    donhang.StatusPrint += "<li>" + orders.username + " đã in (" + datePrint + ")</li>";
                    _donhangRepository.Update(donhang);
                    _unitOfWork.Commit();
                    var donhangVM = Mapper.Map<donhang, donhangDetailViewModel>(donhang);
                    if (donhangVM.khachhang.idtp.HasValue)
                    {
                        var tp = _donhangchuyenphattpRepository.GetSingleById(donhangVM.khachhang.idtp.Value);
                        if (tp != null)
                            donhangVM.tentp = tp.tentp;
                    }
                    if (donhangVM.khachhang.idquan.HasValue)
                    {
                        var quan = _donhangchuyenphattinhRepository.GetSingleById(donhangVM.khachhang.idquan.Value);
                        if (quan != null)
                            donhangVM.tenquan = quan.tentinh;
                    }
                    if (donhang.ChannelId == 2)
                    {
                        if (donhangVM.pttt != null)
                            donhangVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangVM.pttt.Value);
                        if (donhangVM.idgiogiao != null)
                            donhangVM.giogiao = UtilExtensions.ConvertDeliveryTime(donhangVM.idgiogiao.Value);
                    }
                    if (donhang.ChannelId > 2)
                    {
                        if (donhangVM.pttt != null)
                            donhangVM.tenpttt = UtilExtensions.ConvertPaymentMethod(donhangVM.pttt.Value);
                    }
                    if (!string.IsNullOrEmpty(donhangVM.tenptgh))
                    {
                        var pattern = "- từ|[0-9]|- trong";
                        var ptghs = Regex.Split(donhangVM.tenptgh, pattern);
                        donhangVM.tenptgh = ptghs[0];
                    }
                    donhangVMs.Add(donhangVM);
                }
                response = request.CreateResponse(HttpStatusCode.OK, donhangVMs);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("channelrevenuesreport")]
        public HttpResponseMessage ChannelRevenuesReport(HttpRequestMessage request, ChannelSalesRevenuesReportParamsViewModel ChannelSalesReportParamsVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var startDate = ChannelSalesReportParamsVm.startDate.ToLocalTime();
                var startDateStr = startDate.ToString("yyyy-MM-dd") + " 00:00:00";
                var endDate = ChannelSalesReportParamsVm.endDate.ToLocalTime();
                var endDateStr = endDate.ToString("yyyy-MM-dd") + " 23:59:59.999";
                var model = _donhangRepository.GetChannelRevenuesReport(startDateStr, endDateStr, ChannelSalesReportParamsVm.branchId);
                response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("revenuesreport")]
        public HttpResponseMessage RevenuesReport(HttpRequestMessage request, SalesRevenuesReportParamsViewModel SalesReportParamsVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var startDate = SalesReportParamsVm.startDate.ToLocalTime();
                var startDateStr = startDate.ToString("yyyy-MM-dd") + " 00:00:00";
                var endDate = SalesReportParamsVm.endDate.ToLocalTime();
                var endDateStr = endDate.ToString("yyyy-MM-dd") + " 23:59:59.999";
                var model = _donhangRepository.GetRevenuesReport(startDateStr, endDateStr, SalesReportParamsVm.branchId, SalesReportParamsVm.channelId).ToList();
                response = request.CreateResponse(HttpStatusCode.OK, model);
                foreach (var item in model)
                {
                    if (item.NewDay == 1)
                    {
                        item.NewDayStr = item.NewDate;
                    }
                    else
                        item.NewDayStr = item.NewDay.ToString();
                }
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Report")]
        [Route("salesreport")]
        public HttpResponseMessage SalesReport(HttpRequestMessage request, SalesRevenuesReportParamsViewModel SalesReportParamsVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var startDate = SalesReportParamsVm.startDate.ToLocalTime();
                var startDateStr = startDate.ToString("yyyy-MM-dd") + " 00:00:00";
                var endDate = SalesReportParamsVm.endDate.ToLocalTime();
                var endDateStr = endDate.ToString("yyyy-MM-dd") + " 23:59:59.999";
                var model = _donhangRepository.GetSalesReport(startDateStr, endDateStr, SalesReportParamsVm.branchId, SalesReportParamsVm.channelId).ToList();
                response = request.CreateResponse(HttpStatusCode.OK, model);
                foreach (var item in model)
                {
                    if (item.NewDay == 1)
                    {
                        item.NewDayStr = item.NewDate;
                    }
                    else
                        item.NewDayStr = item.NewDay.ToString();
                }
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Report")]
        [Route("channelsalesreport")]
        public HttpResponseMessage ChannelSalesReport(HttpRequestMessage request, ChannelSalesRevenuesReportParamsViewModel ChannelSalesReportParamsVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var startDate = ChannelSalesReportParamsVm.startDate.ToLocalTime();
                var startDateStr = startDate.ToString("yyyy-MM-dd") + " 00:00:00";
                var endDate = ChannelSalesReportParamsVm.endDate.ToLocalTime();
                var endDateStr = endDate.ToString("yyyy-MM-dd") + " 23:59:59.999";
                var model = _donhangRepository.GetChannelSalesReport(startDateStr, endDateStr, ChannelSalesReportParamsVm.branchId).ToList();
                response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Report")]
        [Route("salesreportmonth")]
        public HttpResponseMessage SalesReportMonth(HttpRequestMessage request, SalesRevenuesReportParamsViewModel SalesReportParamsVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var startDate = SalesReportParamsVm.startDate.ToLocalTime();
                var startDateStr = startDate.ToString("yyyy-MM-01 00:00:00");

                var end = SalesReportParamsVm.endDate.ToLocalTime();
                var newEndDate = new DateTime(end.Year, end.Month, 1, 0, 0, 0);
                newEndDate = newEndDate.AddMonths(1).AddDays(-1);
                var endDateStr = newEndDate.ToString("yyyy-MM-dd 23:59:59.999");
                var model = _donhangRepository.GetSalesReportMonth(startDateStr, endDateStr, SalesReportParamsVm.branchId, SalesReportParamsVm.channelId).ToList();
                response = request.CreateResponse(HttpStatusCode.OK, model);
                foreach (var item in model)
                {
                    if (item.NewDay == 1)
                    {
                        item.NewDayStr = item.NewDate;
                    }
                    else
                        item.NewDayStr = item.NewDay.ToString();
                }
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Report")]
        [Route("salesreportyear")]
        public HttpResponseMessage SalesReportYear(HttpRequestMessage request, SalesRevenuesReportParamsViewModel SalesReportParamsVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var startDate = SalesReportParamsVm.startDate.ToLocalTime();
                var startDateStr = startDate.ToString("yyyy-01-01 00:00:00");
                var end = SalesReportParamsVm.endDate.ToLocalTime();
                var newEndDate = new DateTime(end.Year, 12, 1, 0, 0, 0);
                newEndDate = newEndDate.AddMonths(1).AddDays(-1);
                var endDateStr = newEndDate.ToString("yyyy-MM-dd 23:59:59.999");
                var model = _donhangRepository.GetSalesReportYear(startDateStr, endDateStr, SalesReportParamsVm.branchId, SalesReportParamsVm.channelId).ToList();
                response = request.CreateResponse(HttpStatusCode.OK, model);
                foreach (var item in model)
                {
                    if (item.NewDay == 1)
                    {
                        item.NewDayStr = item.NewDate;
                    }
                    else
                        item.NewDayStr = item.NewDay.ToString();
                }
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("authenexport")]
        [Authorize(Roles = "OrderExport")]
        [HttpGet]
        public HttpResponseMessage AuthenExport(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                response = request.CreateResponse(HttpStatusCode.OK, true);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("exportordersexcel")]
        [Authorize(Roles = "OrderExport")]
        [HttpPost]
        public HttpResponseMessage ExportOrdersExcel(HttpRequestMessage request, OrderFilterViewModel orderFilterVM)
        {
            HttpResponseMessage response = null;
            try
            {

                response = request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(GetExcelSheet(orderFilterVM));
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Orders.xlsx";
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        public byte[] GetExcelSheet(OrderFilterViewModel orderFilterVM)
        {
            IEnumerable<donhang> donhangs = null;
            donhangs = _donhangRepository.GetAllPagingFilter(orderFilterVM).ToList();
            if (donhangs.Count() > 0)
            {

            }
            var donhangsVM = Mapper.Map<IEnumerable<donhang>, IEnumerable<donhangExcel>>(donhangs);
            foreach (var item in donhangsVM)
            {
                string chitiet = "";
                var orderDetails = _donhangctRepository.GetMulti(x => x.Sodh == item.id).ToList();
                var lengthOrderDetails = orderDetails.Count;
                if (lengthOrderDetails > 0)
                    for (int i = 0; i < lengthOrderDetails; i++)
                    {
                        var orderDetail = orderDetails[i];
                        var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == orderDetail.IdPro);
                        if (bienthe != null)
                        {
                            var product = _shopSanPhamRepository.GetSingleById(bienthe.idsp.Value);
                            chitiet += string.Format("{0}. {1} | Số lượng: {2}\r\n", i + 1, product.tensp, orderDetail.Soluong);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(orderDetail.Description))
                            {
                                var detailJson = JsonConvert.DeserializeObject<ShopeeOrderDetailItem>(orderDetail.Description);
                                chitiet += string.Format("{0}. {1} | Số lượng: {2}\r\n", i + 1, detailJson.item_name, orderDetail.Soluong);
                            }
                        }
                    }
                item.chitiet = chitiet;
                item.CreatedDateConvert = item.CreatedDate.ToString("dd-MM-yyyy");
            }
            var donhangsVMExcel = Mapper.Map<IEnumerable<donhangExcel>, IEnumerable<donhangExcelNoId>>(donhangsVM);
            using (var package = new ExcelPackage())
            {
                // Tạo author cho file Excel
                package.Workbook.Properties.Author = "SoftBBM";
                // Tạo title cho file Excel
                package.Workbook.Properties.Title = "Export Orders";
                // thêm tí comments vào làm màu 
                package.Workbook.Properties.Comments = "This is my generated Comments";
                // Add Sheet vào file Excel
                package.Workbook.Worksheets.Add("Orders");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells["A1"].LoadFromCollection(donhangsVMExcel, true, TableStyles.Dark9);
                worksheet.DefaultColWidth = 15;
                worksheet.Column(2).Width = 80;
                worksheet.Column(2).Style.WrapText = true;
                worksheet.Column(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Column(2).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Column(3).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Column(4).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Column(5).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Cells[2, 3, donhangsVMExcel.Count() + 1, 3].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A1"].Value = "Ngày tạo";
                worksheet.Cells["B1"].Value = "SP bán";
                worksheet.Cells["C1"].Value = "Tổng tiền";
                worksheet.Cells["D1"].Value = "Ghi chú";
                worksheet.Cells["E1"].Value = "Tình trạng";
                //package.Save();
                return package.GetAsByteArray();
            }
        }

        [HttpPost]
        [Route("updatenewcustomerphone")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateNewCustomerPhone(HttpRequestMessage request, donhangUpdateNewPhoneInput input)
        {
            HttpResponseMessage response = null;
            try
            {
                if (!string.IsNullOrEmpty(input.Phone))
                {
                    var cus = _khachhangRepository.GetSingleByCondition(x => x.dienthoai == input.Phone);
                    if (cus != null)
                    {
                        var order = _donhangRepository.GetSingleById(input.Id);
                        order.makh = cus.MaKH;
                        order.UpdatedBy = input.UserId;
                        order.UpdatedDate = DateTime.Now;
                        _donhangRepository.Update(order);

                        var log = new SoftPointUpdateLog();
                        log.OrderId = order.id;
                        log.CustomerId = cus.MaKH;
                        log.PointBefore = int.Parse(cus.diem);
                        log.PointAdd = (int)order.tongtien / 1000;
                        log.CreatedBy = input.UserId;
                        log.CreatedDate = DateTime.Now;
                        _softPointUpdateLogRepository.Add(log);

                        cus.diem = (int.Parse(cus.diem) + (int)(order.tongtien / 1000)).ToString();
                        _khachhangRepository.Update(cus);

                        _unitOfWork.Commit();
                        response = request.CreateResponse(HttpStatusCode.OK, true);
                    }
                    else
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Không tồn tại khách hàng này!");
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Nhập SĐT KH cần cộng điểm!");
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("shopeeconfirm")]
        //[Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage ShopeeConfirm(HttpRequestMessage request, donhangUpdateViewModel item)
        {
            HttpResponseMessage response = null;
            try
            {
                if (!string.IsNullOrEmpty(item.OrderIdShopeeApi))
                {
                    var pickup_time_id = _shopeeRepository.getTimeSlot(item.OrderIdShopeeApi);
                    if (!string.IsNullOrEmpty(pickup_time_id))
                    {
                        var result = _shopeeRepository.confirmOrder(item.OrderIdShopeeApi, pickup_time_id);
                        if (!string.IsNullOrEmpty(result))
                        {
                            var donhang = _donhangRepository.GetSingleById((int)item.id);
                            var orderDetail = _shopeeRepository.getOrder(item.OrderIdShopeeApi);
                            if (donhang != null)
                            {
                                donhang.TrackingNo = result;
                                donhang.ShipperNameShopeeApi = orderDetail.shipping_carrier;
                                donhang.ShipperTypeShopeeApi = orderDetail.checkout_shipping_carrier;
                                donhang.UpdatedDate = DateTime.Now;
                                donhang.UpdatedBy = 0;
                                _donhangRepository.Update(donhang);
                                _unitOfWork.Commit();
                                return request.CreateResponse(HttpStatusCode.OK, true);
                            }
                        }
                    }
                }
                return request.CreateResponse(HttpStatusCode.BadRequest, "Xác nhận thất bại!");
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }

        [HttpPost]
        [Route("shopeeconfirmall")]
        //[Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage ShopeeConfirmAll(HttpRequestMessage request, List<string> items)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var item in items)
                {
                    var pickup_time_id = _shopeeRepository.getTimeSlot(item);
                    if (!string.IsNullOrEmpty(pickup_time_id))
                    {
                        _shopeeRepository.confirmOrderNoDelay(item, pickup_time_id);
                    }
                    //_shopeeRepository.confirmOrderNoDelayNoTimeAdd(item);

                }
                //Shopee delay
                Thread.Sleep(2000);

                foreach (var item in items)
                {
                    var donhang = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == item);
                    if (donhang != null)
                    {
                        var orderDetail = _shopeeRepository.getOrder(item);
                        if (!string.IsNullOrEmpty(orderDetail.tracking_no))
                        {
                            donhang.TrackingNo = orderDetail.tracking_no;
                            donhang.ShipperNameShopeeApi = orderDetail.shipping_carrier;
                            donhang.ShipperTypeShopeeApi = orderDetail.checkout_shipping_carrier;
                            donhang.UpdatedDate = DateTime.Now;
                            donhang.UpdatedBy = 0;
                            _donhangRepository.Update(donhang);
                        }
                    }
                }

                _unitOfWork.Commit();

                //Kiểm tra đơn sót
                GetLackOrders();

                return request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }

        [HttpPost]
        [Route("shopeesynctrackingno")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage ShopeeSyncTrackingNo(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var shopeeOrderIds = new List<string>();

                shopeeOrderIds = _donhangRepository.GetMulti(x => x.OrderIdShopeeApi != null && x.TrackingNo == null && x.Status != (int)StatusOrder.Cancel).Select(x => x.OrderIdShopeeApi).ToList();

                foreach (var item in shopeeOrderIds)
                {
                    var orderShopee = _shopeeRepository.getOrder(item);
                    if (orderShopee != null)
                        if (!string.IsNullOrEmpty(orderShopee.tracking_no))
                        {
                            var donhangBBM = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == item);
                            if (donhangBBM != null)
                            {
                                donhangBBM.TrackingNo = orderShopee.tracking_no;
                                donhangBBM.UpdatedDate = DateTime.Now;
                                donhangBBM.UpdatedBy = 0;
                                _unitOfWork.Commit();
                            }
                        }
                }
                return request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }

        [HttpPost]
        [Route("updatecompletedtikiorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage updatecompletedtikiorders(HttpRequestMessage request, UpdateCompletedTikiOrderInputVM item)
        {
            try
            {
                string inValidOrders = "";
                string cancelOrders = "";
                if (item.orderIds.Count > 0)
                {
                    foreach (var orderId in item.orderIds)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.ghichu.Trim() == orderId.Trim());
                        if (order != null)
                        {
                            if (order.Status != (int)StatusOrder.Done && order.Status != (int)StatusOrder.Cancel)
                            {
                                order.Status = (int)StatusOrder.Done;
                                order.UpdatedBy = item.UserId;
                                order.UpdatedDate = DateTime.Now;
                                _donhangRepository.Update(order);
                            }
                            else if (order.Status == (int)StatusOrder.Cancel)
                            {
                                cancelOrders += orderId + '\n';
                            }
                        }
                        else
                            inValidOrders += orderId + '\n';
                    }
                    _unitOfWork.Commit();
                }
                return request.CreateResponse(HttpStatusCode.OK, new
                {
                    inValidOrders = inValidOrders,
                    cancelOrders = cancelOrders
                });
            }
            catch (Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("shopeegetlackorderswithday")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage ShopeeGetLackOrdersWithDay(HttpRequestMessage request, int quantity)
        {
            HttpResponseMessage response = null;
            try
            {
                //Kiểm tra đơn sót
                GetLackOrdersWithDay(5);

                return request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }

        [HttpGet]
        [Route("shopeeupdatestatusorderswithday")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage ShopeeUpdateStatusOrdersWithDay(HttpRequestMessage request, int quantity)
        {
            HttpResponseMessage response = null;
            try
            {
                UpdateStatusShopeeOrders(quantity);

                return request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }

        [HttpGet]
        [Route("updatestatusshopeeincompleteorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateStatusShopeeIncompleteOrders(HttpRequestMessage request)
        {
            var result = new List<Object>();
            try
            {
                UpdateStatusShopeeIncompleteOrders();
                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //var contentLog = new SoftPointUpdateLog();
                //contentLog.Description = "Error (GetLackOrders): " + JsonConvert.SerializeObject(ex);
                //contentLog.CreatedDate = DateTime.Now;
                //_softPointUpdateLogRepository.Add(contentLog);
                //_unitOfWork.Commit();
                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public void GetLackOrders()
        {
            var result = new List<Object>();
            try
            {
                var shopeeOrderLastDay = _shopeeRepository.GetOrdersListLastDay();
                if (shopeeOrderLastDay.orders.Count > 0)
                {
                    var orderDB = _shopeeRepository.GetOrdersListLastDayDB();
                    foreach (var orderSPE in shopeeOrderLastDay.orders)
                    {
                        if (!orderDB.Any(x => x.OrderIdShopeeApi == orderSPE.ordersn))
                        {
                            DateTime? updateDate = null;
                            if (orderSPE.update_time > 0)
                                updateDate = UtilExtensions.UnixTimeStampToDateTime(orderSPE.update_time);
                            _shopeeRepository.AddOrderLack(orderSPE.ordersn, orderSPE.order_status, updateDate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "Error (GetLackOrders): " + JsonConvert.SerializeObject(ex);
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();
            }
        }

        public void GetLackOrdersWithDay(int quantity)
        {
            var result = new List<Object>();
            try
            {
                var shopeeOrderLastDay = _shopeeRepository.GetOrdersListLastWithDay(quantity);
                if (shopeeOrderLastDay.orders.Count > 0)
                {
                    var orderDB = _shopeeRepository.GetOrdersListLastWithDayDB(quantity);
                    foreach (var orderSPE in shopeeOrderLastDay.orders)
                    {
                        if (!orderDB.Any(x => x.OrderIdShopeeApi == orderSPE.ordersn))
                        {
                            DateTime? updateDate = null;
                            if (orderSPE.update_time > 0)
                                updateDate = UtilExtensions.UnixTimeStampToDateTime(orderSPE.update_time);
                            _shopeeRepository.AddOrderLack(orderSPE.ordersn, orderSPE.order_status, updateDate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "Error (GetLackOrders): " + JsonConvert.SerializeObject(ex);
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();
            }
        }

        public void UpdateStatusShopeeOrders(int quantity)
        {
            var result = new List<Object>();
            try
            {
                var orderDBs = _shopeeRepository.GetOrdersListLastWithDayDB(quantity);
                if (orderDBs.Count > 0)
                {
                    foreach (var orderDB in orderDBs)
                    {
                        if (!string.IsNullOrEmpty(orderDB.OrderIdShopeeApi))
                        {
                            var orderSPE = _shopeeRepository.getOrder(orderDB.OrderIdShopeeApi);
                            var order = _donhangRepository.GetSingleByCondition(x => x.id == orderDB.id);
                            if (order != null)
                            {
                                switch (orderSPE.order_status)
                                {
                                    case CommonClass.COMPLETED:
                                        order.Status = (int)StatusOrder.Done;
                                        break;
                                    case CommonClass.SHIPPED:
                                        order.Status = (int)StatusOrder.Shipping;
                                        break;
                                    case CommonClass.TO_CONFIRM_RECEIVE:
                                        order.Status = (int)StatusOrder.Shipped;
                                        break;
                                    case CommonClass.READY_TO_SHIP:
                                        order.Status = (int)StatusOrder.ReadyToShip;
                                        break;
                                    case CommonClass.CANCELLED:
                                        if (order.Status != (int)StatusOrder.Cancel)
                                        {
                                            order.Status = (int)StatusOrder.Cancel;
                                            RollBackOrder(orderDB.OrderIdShopeeApi);
                                        }
                                        break;
                                }
                                order.UpdatedBy = 0;
                                order.UpdatedDate = DateTime.Now;
                                _donhangRepository.Update(order);
                                _unitOfWork.Commit();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "Error (GetLackOrders): " + JsonConvert.SerializeObject(ex);
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();
            }
        }

        //Cùng logic vs ShopeeController Hook Cancel Order
        public void RollBackOrder(string orderShopeeId)
        {
            var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi.Trim() == orderShopeeId.Trim());
            if (order != null)
            {
                foreach (var item in order.donhang_ct)
                {
                    var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                    var stockCurrent = _softStockRepository.GetMulti(x => x.BranchId == (int)BranchEnum.KHO_CHINH && x.ProductId == bienthe.idsp).FirstOrDefault();
                    if (stockCurrent == null)
                    {
                        var newStockCurrent = new SoftBranchProductStock();
                        newStockCurrent.BranchId = (int)BranchEnum.KHO_CHINH;
                        newStockCurrent.ProductId = bienthe.idsp;
                        newStockCurrent.StockTotal = 0;
                        newStockCurrent.CreatedDate = DateTime.Now;
                        newStockCurrent.CreatedBy = 0;
                        stockCurrent = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    stockCurrent.StockTotal += item.Soluong;
                    _softStockRepository.Update(stockCurrent);

                    _unitOfWork.Commit();
                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.UpdateShopSanPhamLog(bienthe.idsp, "Cập nhật huỷ đơn hàng Api Shopee, mã đơn Api Shopee: " + order.OrderIdShopeeApi + ", mã đơn: " + order.id, item.Soluong, 0, (int)BranchEnum.KHO_CHINH, stockCurrent.StockTotal.Value);
                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value);
                    _shopSanPhamLogRepository.Add(productLog);

                }
            }
        }

        public void UpdateStatusShopeeIncompleteOrders()
        {
            var result = new List<Object>();
            try
            {
                var orderDBs = _donhangRepository.GetMulti(x => x.Status != (int)StatusOrder.Done
                                                                && x.Status != (int)StatusOrder.Cancel
                                                                && !string.IsNullOrEmpty(x.OrderIdShopeeApi))
                                                                    .Select(x => new { id = x.id, OrderIdShopeeApi = x.OrderIdShopeeApi }).ToList();
                if (orderDBs.Count > 0)
                {
                    foreach (var orderDB in orderDBs)
                    {
                        if (!string.IsNullOrEmpty(orderDB.OrderIdShopeeApi))
                        {
                            var orderSPE = _shopeeRepository.getOrder(orderDB.OrderIdShopeeApi);
                            var order = _donhangRepository.GetSingleByCondition(x => x.id == orderDB.id);
                            if (order != null)
                            {
                                switch (orderSPE.order_status)
                                {
                                    case CommonClass.COMPLETED:
                                        order.Status = (int)StatusOrder.Done;
                                        break;
                                    case CommonClass.SHIPPED:
                                        order.Status = (int)StatusOrder.Shipping;
                                        break;
                                    case CommonClass.TO_CONFIRM_RECEIVE:
                                        order.Status = (int)StatusOrder.Shipped;
                                        break;
                                    case CommonClass.READY_TO_SHIP:
                                        order.Status = (int)StatusOrder.ReadyToShip;
                                        break;
                                    case CommonClass.CANCELLED:
                                        if (order.Status != (int)StatusOrder.Cancel)
                                        {
                                            order.Status = (int)StatusOrder.Cancel;
                                            RollBackOrder(orderDB.OrderIdShopeeApi);
                                        }
                                        break;
                                }
                                order.UpdatedBy = 0;
                                order.UpdatedDate = DateTime.Now;
                                _donhangRepository.Update(order);
                                _unitOfWork.Commit();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "Error (UpdateStatusShopeeIncompleteOrders): " + JsonConvert.SerializeObject(ex);
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();
            }
        }

        [HttpGet]
        [Route("updateordernullproductid")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage UpdateOrderNullProductId(HttpRequestMessage request)
        {
            try
            {
                var desList = _donhangctRepository.GetMulti(x => x.IdPro == null).Select(x => new
                {
                    Id = x.Id,
                    Description = x.Description

                }).ToList();
                if (desList.Count > 0)
                {
                    foreach (var des in desList)
                    {
                        var item = JsonConvert.DeserializeObject<ShopeeOrderDetailItem>(des.Description);
                        var donhangct = _donhangctRepository.GetSingleById((int)des.Id);
                        var shopeeProduct = _shopeeRepository.GetItemDetail(item.item_id);
                        if (shopeeProduct != null)
                        {
                            var sku = "";
                            if (!string.IsNullOrEmpty(shopeeProduct.item_sku))
                                sku = shopeeProduct.item_sku;
                            if (!string.IsNullOrEmpty(sku))
                            {
                                var productDB = _shopSanPhamRepository.GetSingleByCondition(x => x.masp.Trim().ToLower() == sku.Trim().ToLower());
                                if (productDB != null)
                                {
                                    donhangct.IdPro = productDB.id;
                                    _donhangctRepository.Update(donhangct);

                                    var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == (int)BranchEnum.KHO_CHINH && x.ProductId == productDB.id);
                                    if (stockCurrent == null)
                                    {
                                        var newStockCurrent = new SoftBranchProductStock();
                                        newStockCurrent.BranchId = (int)BranchEnum.KHO_CHINH;
                                        newStockCurrent.ProductId = productDB.id;
                                        newStockCurrent.StockTotal = 0;
                                        newStockCurrent.CreatedDate = DateTime.Now;
                                        newStockCurrent.CreatedBy = 0;
                                        stockCurrent = _softStockRepository.Add(newStockCurrent);
                                    }
                                    stockCurrent.StockTotal -= donhangct.Soluong;
                                    _softStockRepository.Update(stockCurrent);

                                    var channelOrder = _softChannelRepository.GetSingleById((int)ChannelEnum.SPE);
                                    _unitOfWork.Commit();
                                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                                    productLog.ProductId = productDB.id;
                                    productLog.Description = "Đơn hàng " + channelOrder.Name + ", mã đơn: " + donhangct.Sodh;
                                    productLog.Quantity = donhangct.Soluong;
                                    productLog.CreatedBy = 0;
                                    productLog.CreatedDate = DateTime.Now;
                                    productLog.BranchId = (int)BranchEnum.KHO_CHINH;
                                    productLog.StockTotal = stockCurrent.StockTotal.Value;
                                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(productDB.id);
                                    _shopSanPhamLogRepository.Add(productLog);
                                    _unitOfWork.Commit();
                                }
                            }
                        }
                    }
                }
                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var log = new SystemLog();
                log.InitSystemLog(0, "", "UpdateOrderNullProductId", JsonConvert.SerializeObject(ex), 99, "OrderUpdate");
                _systemLogRepository.Add(log);
                _unitOfWork.Commit();
                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("gettrackingnoshopeeorders")]
        //[Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage GetTrackingNoShopeeOrders(HttpRequestMessage request, List<string> items)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var item in items)
                {
                    var order = _shopeeRepository.getOrder(item);
                    if (!string.IsNullOrEmpty(order.tracking_no))
                    {
                        var donhang = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == item);
                        if (donhang != null)
                        {
                            donhang.TrackingNo = order.tracking_no;
                            _donhangRepository.Update(donhang);
                        }
                    }
                    _unitOfWork.Commit();
                }
                //Shopee delay
                Thread.Sleep(2000);

                foreach (var item in items)
                {
                    var donhang = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == item);
                    if (donhang != null)
                    {
                        var orderDetail = _shopeeRepository.getOrder(item);
                        if (!string.IsNullOrEmpty(orderDetail.tracking_no))
                        {
                            donhang.TrackingNo = orderDetail.tracking_no;
                            donhang.UpdatedDate = DateTime.Now;
                            donhang.UpdatedBy = 0;
                            _donhangRepository.Update(donhang);
                        }
                    }
                }

                _unitOfWork.Commit();

                //Kiểm tra đơn sót
                GetLackOrders();

                return request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }

        [HttpPost]
        [Route("addofflineorderwindow")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage AddOfflineOrderWindow(HttpRequestMessage request, AddOfflineOrderWindowInputVM item)
        {
            try
            {
                var offlineOrderWindow = _softOfflineOrderWindowRepository.GetSingleByCondition(x => x.UserId == item.UserId);
                if (offlineOrderWindow != null)
                {
                    offlineOrderWindow.Value = item.value;
                    _softOfflineOrderWindowRepository.Update(offlineOrderWindow);
                }
                else
                {
                    offlineOrderWindow = new SoftOfflineOrderWindow();
                    offlineOrderWindow.UserId = item.UserId;
                    offlineOrderWindow.Value = item.value;
                    _softOfflineOrderWindowRepository.Add(offlineOrderWindow);
                }
                _unitOfWork.Commit();
                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getofflineorderwindow")]
        public HttpResponseMessage GetOfflineOrderWindow(HttpRequestMessage request, int userId)
        {
            try
            {
                if (userId > 0)
                {
                    var offlineOrderWindow = _softOfflineOrderWindowRepository.GetSingleByCondition(x => x.UserId == userId);
                    if (offlineOrderWindow != null)
                        return request.CreateResponse(HttpStatusCode.OK, offlineOrderWindow.Value);
                }
                return request.CreateResponse(HttpStatusCode.BadRequest, "Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message + " | " + ex.StackTrace);
            }
        }
    }
}