using AutoMapper;
using Newtonsoft.Json;
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
using System.Web.Http;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/adjustmentstock")]
    public class SoftAdjustmentStockController : ApiController
    {
        ISoftStockRepository _softStockRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftAdjustmentStockRepository _softAdjustmentStockRepository;
        ISoftAdjustmentStockDetailRepository _softAdjustmentStockDetailRepository;
        IShopSanPhamLogRepository _shopSanPhamLogRepository;
        IUnitOfWork _unitOfWork;

        public SoftAdjustmentStockController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IUnitOfWork unitOfWork, ISoftAdjustmentStockRepository softAdjustmentStockRepository, ISoftAdjustmentStockDetailRepository softAdjustmentStockDetailRepository, IShopSanPhamLogRepository shopSanPhamLogRepository)
        {
            _softStockRepository = softStockRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _unitOfWork = unitOfWork;
            _softAdjustmentStockRepository = softAdjustmentStockRepository;
            _softAdjustmentStockDetailRepository = softAdjustmentStockDetailRepository;
            _shopSanPhamLogRepository = shopSanPhamLogRepository;
        }

        [HttpPost]
        [Route("search")]
        [Authorize(Roles = "AdjustmentStockList")]
        public HttpResponseMessage Search(HttpRequestMessage request, AdjustmentStockFilterViewModel adjustmentStockFilterVM)
        {
            int currentPage = adjustmentStockFilterVM.page.Value;
            int currentPageSize = adjustmentStockFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftAdjustmentStock> adjustmentStocks = null;
            int totalAdjustmentStocks = new int();

            adjustmentStocks = _softAdjustmentStockRepository.GetAllPaging(currentPage, currentPageSize, out totalAdjustmentStocks, adjustmentStockFilterVM).ToList();

            IEnumerable<SoftAdjustmentStockViewModel> adjustmentStocksVM = Mapper.Map<IEnumerable<SoftAdjustmentStock>, IEnumerable<SoftAdjustmentStockViewModel>>(adjustmentStocks);

            PaginationSet<SoftAdjustmentStockViewModel> pagedSet = new PaginationSet<SoftAdjustmentStockViewModel>()
            {
                Page = currentPage,
                TotalCount = totalAdjustmentStocks,
                TotalPages = (int)Math.Ceiling((decimal)totalAdjustmentStocks / currentPageSize),
                Items = adjustmentStocksVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("add")]
        [Authorize(Roles = "AdjustmentStockAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, SoftAdjustmentStockAddViewModel softAdjustmentStockAddVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var newSoftAdjustmentStock = new SoftAdjustmentStock();
                newSoftAdjustmentStock.UpdateAdjustmentStockAdd(softAdjustmentStockAddVM);
                _softAdjustmentStockRepository.Add(newSoftAdjustmentStock);
                _unitOfWork.Commit();
                foreach (var item in softAdjustmentStockAddVM.SoftAdjustmentStockDetails)
                {
                    var newSoftAdjustmentStockDetail = new SoftAdjustmentStockDetail();
                    newSoftAdjustmentStockDetail.AdjustmentId = newSoftAdjustmentStock.Id;
                    newSoftAdjustmentStockDetail.UpdateAdjustmentStockDetailAdd(item);
                    _softAdjustmentStockDetailRepository.Add(newSoftAdjustmentStockDetail);
                    var stock = _softStockRepository.GetSingleByCondition(x => x.BranchId == softAdjustmentStockAddVM.BranchId && x.ProductId == item.id);
                    if (stock == null)
                    {
                        var newstock = UtilExtensions.InitStock(softAdjustmentStockAddVM.BranchId.Value, item.id);
                        _softStockRepository.Add(newstock);
                        _unitOfWork.Commit();
                        stock = newstock;
                    }
                    stock.StockTotal = item.Quantity;
                    stock.UpdatedDate = DateTime.Now;
                    stock.UpdatedBy = softAdjustmentStockAddVM.CreatedBy;
                    _softStockRepository.Update(stock);

                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.id;
                    productLog.Description = "Cập nhật từ phiếu điều chỉnh, mã phiếu: " + newSoftAdjustmentStock.Id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = softAdjustmentStockAddVM.CreatedBy;
                    productLog.CreatedDate = DateTime.Now;
                    productLog.BranchId = softAdjustmentStockAddVM.BranchId;
                    _shopSanPhamLogRepository.Add(productLog);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.Created, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("detail")]
        [Authorize(Roles = "AdjustmentStockDetail")]
        [HttpGet]
        public HttpResponseMessage Detail(HttpRequestMessage request, int adjustmentStockId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softAdjustmentStock = _softAdjustmentStockRepository.GetSingleById(adjustmentStockId);
                var responsedata = Mapper.Map<SoftAdjustmentStock, SoftAdjustmentStockViewModel>(softAdjustmentStock);
                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("authenadd")]
        [Authorize(Roles = "AdjustmentStockAdd")]
        [HttpGet]
        public HttpResponseMessage AuthenAdd(HttpRequestMessage request)
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
    }
}

//namespace SoftBBM.Web.api
//{
//    [RoutePrefix("api/adjustmentstock")]
//    public class SoftAdjustmentStockController : ApiController
//    {
//        ISoftStockRepository _softStockRepository;
//        IShopSanPhamRepository _shopSanPhamRepository;
//        ISoftAdjustmentStockRepository _softAdjustmentStockRepository;
//        ISoftAdjustmentStockDetailRepository _softAdjustmentStockDetailRepository;
//        IShopSanPhamLogRepository _shopSanPhamLogRepository;
//        IShopeeRepository _shopeeRepository;
//        IUnitOfWork _unitOfWork;

//        public SoftAdjustmentStockController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IUnitOfWork unitOfWork, ISoftAdjustmentStockRepository softAdjustmentStockRepository, ISoftAdjustmentStockDetailRepository softAdjustmentStockDetailRepository, IShopSanPhamLogRepository shopSanPhamLogRepository, IShopeeRepository shopeeRepository)
//        {
//            _softStockRepository = softStockRepository;
//            _shopSanPhamRepository = shopSanPhamRepository;
//            _unitOfWork = unitOfWork;
//            _softAdjustmentStockRepository = softAdjustmentStockRepository;
//            _softAdjustmentStockDetailRepository = softAdjustmentStockDetailRepository;
//            _shopSanPhamLogRepository = shopSanPhamLogRepository;
//            _shopeeRepository = shopeeRepository;
//        }

//        [HttpPost]
//        [Route("search")]
//        [Authorize(Roles = "AdjustmentStockList")]
//        public HttpResponseMessage Search(HttpRequestMessage request, AdjustmentStockFilterViewModel adjustmentStockFilterVM)
//        {
//            int currentPage = adjustmentStockFilterVM.page.Value;
//            int currentPageSize = adjustmentStockFilterVM.pageSize.Value;
//            HttpResponseMessage response = null;
//            List<SoftAdjustmentStock> adjustmentStocks = null;
//            int totalAdjustmentStocks = new int();

//            adjustmentStocks = _softAdjustmentStockRepository.GetAllPaging(currentPage, currentPageSize, out totalAdjustmentStocks, adjustmentStockFilterVM).ToList();

//            IEnumerable<SoftAdjustmentStockViewModel> adjustmentStocksVM = Mapper.Map<IEnumerable<SoftAdjustmentStock>, IEnumerable<SoftAdjustmentStockViewModel>>(adjustmentStocks);

//            PaginationSet<SoftAdjustmentStockViewModel> pagedSet = new PaginationSet<SoftAdjustmentStockViewModel>()
//            {
//                Page = currentPage,
//                TotalCount = totalAdjustmentStocks,
//                TotalPages = (int)Math.Ceiling((decimal)totalAdjustmentStocks / currentPageSize),
//                Items = adjustmentStocksVM
//            };

//            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
//            return response;
//        }

//        [Route("add")]
//        [Authorize(Roles = "AdjustmentStockAdd")]
//        [HttpPost]
//        public HttpResponseMessage Add(HttpRequestMessage request, SoftAdjustmentStockAddViewModel softAdjustmentStockAddVM)
//        {
//            HttpResponseMessage response = null;
//            var message = new ResponseBBM();
//            try
//            {
//                var newSoftAdjustmentStock = new SoftAdjustmentStock();
//                newSoftAdjustmentStock.UpdateAdjustmentStockAdd(softAdjustmentStockAddVM);
//                _softAdjustmentStockRepository.Add(newSoftAdjustmentStock);
//                _unitOfWork.Commit();
//                foreach (var item in softAdjustmentStockAddVM.SoftAdjustmentStockDetails)
//                {
//                    var newSoftAdjustmentStockDetail = new SoftAdjustmentStockDetail();
//                    newSoftAdjustmentStockDetail.AdjustmentId = newSoftAdjustmentStock.Id;
//                    newSoftAdjustmentStockDetail.UpdateAdjustmentStockDetailAdd(item);
//                    _softAdjustmentStockDetailRepository.Add(newSoftAdjustmentStockDetail);
//                    var stock = _softStockRepository.GetSingleByCondition(x => x.BranchId == softAdjustmentStockAddVM.BranchId && x.ProductId == item.id);
//                    if (stock == null)
//                    {
//                        var newstock = UtilExtensions.InitStock(softAdjustmentStockAddVM.BranchId.Value, item.id);
//                        _softStockRepository.Add(newstock);
//                        _unitOfWork.Commit();
//                        stock = newstock;
//                    }
//                    stock.StockTotal = item.Quantity;
//                    stock.UpdatedDate = DateTime.Now;
//                    stock.UpdatedBy = softAdjustmentStockAddVM.CreatedBy;
//                    _softStockRepository.Update(stock);
//                    var product = _shopSanPhamRepository.GetSingleById(item.id);
//                    message.Success += "Cập nhật SoftBBM - " + product.masp + " - thành công !<br>";

//                    var responseShopee = _shopeeRepository.update_stock(1545703, 20, 101110, 213916);
//                    if (!string.IsNullOrEmpty(responseShopee) && !responseShopee.Contains("error"))
//                    {
//                        var resultJson = JsonConvert.DeserializeObject<UpdateStockRes>(responseShopee);
//                        if (resultJson.item.stock==20)
//                        {
//                            message.Success += "Cập nhật Shopee - " + product.masp + " - thành công!<br>";
//                        }
//                        else
//                        {
//                            message.Error += "Cập nhật Shopee - " + product.masp + " - thất bại!<br>";
//                        }
//                    }
//                    else
//                    {
//                        message.Error += " Cập nhật Shopee - " + product.masp + " - thất bại!<br>";
//                    }

//                    shop_sanphamLogs productLog = new shop_sanphamLogs();
//                    productLog.ProductId = item.id;
//                    productLog.Description = "Cập nhật từ phiếu điều chỉnh, mã phiếu: " + newSoftAdjustmentStock.Id;
//                    productLog.Quantity = item.Quantity;
//                    productLog.CreatedBy = softAdjustmentStockAddVM.CreatedBy;
//                    productLog.CreatedDate = DateTime.Now;
//                    productLog.BranchId = softAdjustmentStockAddVM.BranchId;
//                    _shopSanPhamLogRepository.Add(productLog);
//                }
//                _unitOfWork.Commit();
//                response = request.CreateResponse(HttpStatusCode.OK, message);
//            }
//            catch (Exception ex)
//            {
//                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
//            }
//            return response;
//        }

//        [Route("detail")]
//        [Authorize(Roles = "AdjustmentStockDetail")]
//        [HttpGet]
//        public HttpResponseMessage Detail(HttpRequestMessage request, int adjustmentStockId)
//        {
//            HttpResponseMessage response = null;
//            try
//            {
//                var softAdjustmentStock = _softAdjustmentStockRepository.GetSingleById(adjustmentStockId);
//                var responsedata = Mapper.Map<SoftAdjustmentStock, SoftAdjustmentStockViewModel>(softAdjustmentStock);
//                response = request.CreateResponse(HttpStatusCode.OK, responsedata);
//                return response;
//            }

//            catch (Exception ex)
//            {
//                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
//                return response;
//            }
//        }

//        [Route("authenadd")]
//        [Authorize(Roles = "AdjustmentStockAdd")]
//        [HttpGet]
//        public HttpResponseMessage AuthenAdd(HttpRequestMessage request)
//        {
//            HttpResponseMessage response = null;
//            try
//            {
//                response = request.CreateResponse(HttpStatusCode.OK, true);
//                return response;
//            }
//            catch (Exception ex)
//            {

//                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
//                return response;
//            }

//        }
//    }
//}