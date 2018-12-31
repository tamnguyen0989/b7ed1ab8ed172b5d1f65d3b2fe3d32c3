using SoftBBM.Web.DAL.Infrastructure;
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
        IUnitOfWork _unitOfWork;

        public donhangController(IUnitOfWork unitOfWork, IdonhangRepository donhangRepository, IdonhangctRepository donhangctRepository, IkhachhangRepository khachhangRepository, IshopbientheRepository shopbientheRepository, ISoftStockRepository softStockRepository, IdonhangchuyenphattinhRepository donhangchuyenphattinhRepository, IdonhangchuyenphattpRepository donhangchuyenphattpRepository, IApplicationUserRepository applicationUserRepository, IdonhangchuyenphatvungRepository donhangchuyenphatvungRepository, IdonhangchuyenphatdiachifutaRepository donhangchuyenphatdiachifutaRepository, IApplicationUserSoftBranchRepository applicationUserSoftBranchRepository, ISoftChannelRepository softChannelRepository, IShopSanPhamRepository shopSanPhamRepository, ISoftChannelProductPriceRepository softChannelProductPriceRepository, ISoftBranchRepository softBranchRepository, IShopSanPhamLogRepository shopSanPhamLogRepository)
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
        }

        [Route("save")]
        [Authorize(Roles = "OrderAdd")]
        [HttpPost]
        public HttpResponseMessage Save(HttpRequestMessage request, OrderViewModel orderVM)
        {
            HttpResponseMessage response = null;
            try
            {
                donhang donhang = new donhang();
                donhang.Updatedonhang(orderVM);
                donhang.CreatedDate = DateTime.Now;
                var user = _applicationUserRepository.GetSingleById(orderVM.CreatedBy.Value);
                if (orderVM.ChannelId == 2)
                {
                    var datePrint = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    donhang.StatusPrint += "<li>" + user.UserName + " đã in (" + datePrint + ")</li>";
                }
                if (orderVM.Customer.dienthoai != null)
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
            try
            {
                donhang donhang = new donhang();
                donhang.Updatedonhang(orderVM);
                donhang.CreatedDate = DateTime.Now;
                if (orderVM.Customer.dienthoai != null && orderVM.Customer.dienthoai != "")
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
                    _donhangctRepository.Add(donhang_ct);

                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.id;
                    productLog.Description = "Đơn hàng " + channelOrder.Name + ", từ kho " + branchOrder.Name + ", mã đơn: " + result.id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = orderVM.CreatedBy;
                    productLog.CreatedDate = DateTime.Now;
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

            donhangs = _donhangRepository.GetAllPaging(currentPage, currentPageSize, out totaldonhangs, out totalMoney, orderFilterVM).ToList();

            IEnumerable<donhangListViewModel> donhangsVM = Mapper.Map<IEnumerable<donhang>, IEnumerable<donhangListViewModel>>(donhangs);

            foreach (var item in donhangsVM)
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
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
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
                        var oldOrder = _donhangRepository.GetSingleByCondition(x => x.id == orderVM.id);
                        oldOrder.Status = 4;
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
                    if (donhang.makh.Value > 0)
                    {
                        var khachhangModel = _khachhangRepository.GetSingleById(donhang.makh.Value);
                        khachhangVM = Mapper.Map<khachhang, khachhangViewModel>(khachhangModel);
                    }

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
                        orderDetails.Add(shopSanPhamVM);
                    }

                    donhangAfterEditViewModel donhangAfterEdit = new donhangAfterEditViewModel();
                    donhangAfterEdit.channel = channelVM;
                    donhangAfterEdit.customer = khachhangVM;
                    donhangAfterEdit.orderDetails = orderDetails;

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
                        if (donhangVM.makh > 0)
                        {
                            var khachhang = _khachhangRepository.GetSingleById(donhangVM.makh.Value);
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
                                var newStockCurrent = _softStockRepository.Init(donhang.BranchId.Value, bienthe.idsp.Value);
                                stockCurrent = _softStockRepository.Add(newStockCurrent);
                                _unitOfWork.Commit();
                            }
                            stockCurrent.StockTotal += item.Soluong;
                            _softStockRepository.Update(stockCurrent);
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
    }
}
