using AutoMapper;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Infrastructure.Core;
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
    [RoutePrefix("api/returnsupplier")]
    public class SoftReturnSupplierController : ApiController
    {
        ISoftStockRepository _softStockRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftReturnSupplierRepository _softReturnSupplierRepository;
        ISoftReturnSupplierDetailRepository _softReturnSupplierDetailRepository;
        IShopSanPhamLogRepository _shopSanPhamLogRepository;
        IUnitOfWork _unitOfWork;

        public SoftReturnSupplierController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IUnitOfWork unitOfWork, ISoftReturnSupplierRepository softReturnSupplierRepository, ISoftReturnSupplierDetailRepository softReturnSupplierDetailRepository, IShopSanPhamLogRepository shopSanPhamLogRepository)
        {
            _softStockRepository = softStockRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _unitOfWork = unitOfWork;
            _softReturnSupplierRepository = softReturnSupplierRepository;
            _softReturnSupplierDetailRepository = softReturnSupplierDetailRepository;
            _shopSanPhamLogRepository = shopSanPhamLogRepository;
        }

        [HttpPost]
        [Route("search")]
        [Authorize(Roles = "ReturnSupplierList")]
        public HttpResponseMessage Search(HttpRequestMessage request, SoftReturnSupplierFilterViewModel softReturnSupplierFilterVM)
        {
            int currentPage = softReturnSupplierFilterVM.page.Value;
            int currentPageSize = softReturnSupplierFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftReturnSupplier> softReturnSuppliers = null;
            int totalReturnSuppliers = new int();

            if (softReturnSupplierFilterVM.branchId != 2)
            {
                response = request.CreateResponse(HttpStatusCode.NotFound, "404 not found!");
                return response;
            }
            softReturnSuppliers = _softReturnSupplierRepository.GetAllPaging(currentPage, currentPageSize, out totalReturnSuppliers, softReturnSupplierFilterVM).ToList();

            IEnumerable<SoftReturnSupplierViewModel> softReturnSuppliersVM = Mapper.Map<IEnumerable<SoftReturnSupplier>, IEnumerable<SoftReturnSupplierViewModel>>(softReturnSuppliers);

            var pagedSet = new PaginationSet<SoftReturnSupplierViewModel>()
            {
                Page = currentPage,
                TotalCount = totalReturnSuppliers,
                TotalPages = (int)Math.Ceiling((decimal)totalReturnSuppliers / currentPageSize),
                Items = softReturnSuppliersVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("add")]
        [Authorize(Roles = "ReturnSupplierAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, SoftReturnSupplierAddViewModel softReturnSupplierVm)
        {
            HttpResponseMessage response = null;
            try
            {
                foreach (var item in softReturnSupplierVm.SoftReturnSupplierDetails)
                {
                    double stockTmp = 0;
                    var itemStock = _softStockRepository.GetSingleByCondition(x => x.ProductId == item.Id && x.BranchId == softReturnSupplierVm.BranchId);
                    if (itemStock != null)
                        stockTmp = itemStock.StockTotal.Value;
                    if (item.Quantity > stockTmp)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Tồn không đủ trả hàng");
                        return response;
                    }
                }

                var newSoftReturnSupplier = new SoftReturnSupplier();
                newSoftReturnSupplier.BranchId = softReturnSupplierVm.BranchId;
                if (softReturnSupplierVm.SupplierId > 0)
                    newSoftReturnSupplier.SupplierId = softReturnSupplierVm.SupplierId;
                newSoftReturnSupplier.Description = softReturnSupplierVm.Description;
                newSoftReturnSupplier.TotalQuantity = softReturnSupplierVm.TotalQuantity;
                newSoftReturnSupplier.CreatedBy = softReturnSupplierVm.CreatedBy;
                newSoftReturnSupplier.CreatedDate = DateTime.Now;
                _softReturnSupplierRepository.Add(newSoftReturnSupplier);
                _unitOfWork.Commit();
                foreach (var item in softReturnSupplierVm.SoftReturnSupplierDetails)
                {
                    var newSoftReturnSupplierDetailVm = new SoftReturnSupplierDetail();
                    newSoftReturnSupplierDetailVm.ReturnSupplierId = newSoftReturnSupplier.Id;
                    newSoftReturnSupplierDetailVm.ProductId = item.Id;
                    newSoftReturnSupplierDetailVm.Quantity = item.Quantity;
                    _softReturnSupplierDetailRepository.Add(newSoftReturnSupplierDetailVm);
                    var itemStock = _softStockRepository.GetSingleByCondition(x => x.ProductId == item.Id && x.BranchId == softReturnSupplierVm.BranchId);
                    if (itemStock == null)
                    {
                        var newStockCurrent = _softStockRepository.Init(softReturnSupplierVm.BranchId.Value, item.Id);
                        itemStock = _softStockRepository.Add(newStockCurrent);
                        _unitOfWork.Commit();
                    }
                    itemStock.StockTotal = itemStock.StockTotal - item.Quantity;
                    _softStockRepository.Update(itemStock);

                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                    productLog.ProductId = item.Id;
                    productLog.Description = "Xuất từ phiếu trả hàng, mã phiếu: " + newSoftReturnSupplier.Id;
                    productLog.Quantity = item.Quantity;
                    productLog.CreatedBy = softReturnSupplierVm.CreatedBy;
                    productLog.CreatedDate = DateTime.Now;
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
        [Authorize(Roles = "ReturnSupplierDetail")]
        [HttpGet]
        public HttpResponseMessage Detail(HttpRequestMessage request, int returnSupplierId)
        {
            HttpResponseMessage response = null;
            try
            {
                var softReturnSupplier = _softReturnSupplierRepository.GetSingleById(returnSupplierId);
                var responsedata = Mapper.Map<SoftReturnSupplier, SoftReturnSupplierViewModel>(softReturnSupplier);
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
        [Authorize(Roles = "ReturnSupplierAdd")]
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
