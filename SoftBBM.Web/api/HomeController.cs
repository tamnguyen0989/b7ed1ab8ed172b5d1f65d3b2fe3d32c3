using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/home")]
    [Authorize]
    public class HomeController : ApiController
    {
        ISoftChannelRepository _softChannelRepository;
        ISoftBranchRepository _softBranchRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        ISoftStockRepository _softStockRepository;
        ISoftStockInRepository _softStockInRepository;
        ISoftStockInDetailRepository _softStockInDetailRepository;
        IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork, ISoftChannelRepository softChannelRepository, IShopSanPhamRepository shopSanPhamRepository, ISoftChannelProductPriceRepository softChannelProductPriceRepository, ISoftBranchRepository softBranchRepository, ISoftStockRepository softStockRepository, ISoftStockInRepository softStockInRepository, ISoftStockInDetailRepository softStockInDetailRepository)
        {
            _unitOfWork = unitOfWork;
            _softChannelRepository = softChannelRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _softBranchRepository = softBranchRepository;
            _softStockRepository = softStockRepository;
            _softStockInRepository = softStockInRepository;
            _softStockInDetailRepository = softStockInDetailRepository;
        }

        [HttpPost]
        [Route("TestMethod")]
        public string TestMethod()
        {
            return "Hello, BBM Member. ";
        }

        //[Route("migrationchannelpriceandproduct")]
        //[HttpGet]
        //public HttpResponseMessage MigrationChannelPriceAndProduct(HttpRequestMessage request)
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        var products = _shopSanPhamRepository.GetAllIds();
        //        var channels = _softChannelRepository.GetAllIds();
        //        foreach (var product in products.ToList())
        //        {
        //            foreach (var channel in channels.ToList())
        //            {
        //                var price = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == product && x.ChannelId == channel);
        //                if (price == null)
        //                {
        //                    var newPrice = new SoftChannelProductPrice();
        //                    newPrice.ProductId = product;
        //                    newPrice.ChannelId = channel;
        //                    newPrice.Price = 0;
        //                    _softChannelProductPriceRepository.Add(newPrice);
        //                    _unitOfWork.Commit();
        //                }
        //            }
        //        }


        //        response = request.CreateResponse(HttpStatusCode.OK, true);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //        return response;
        //    }
        //}

        [Route("updatepricewholesale")]
        [HttpGet]
        public HttpResponseMessage UpdatePriceWholesale(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                //var product = _shopSanPhamRepository.GetSingleByCondition(x => x.masp.Contains("M395"));
                var products = _shopSanPhamRepository.GetAll();
                foreach (var product in products)
                {
                    var priceWholesale = 0;
                    var priceONL = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == product.id && x.ChannelId == 2);
                    if (priceONL != null)
                    {
                        var avg = product.PriceAvg == null ? 0 : product.PriceAvg.Value;
                        var onl = priceONL.Price == null ? 0 : priceONL.Price.Value;
                        priceWholesale = UtilExtensions.GetPriceWholesaleByPriceAvgOnl(avg, onl);
                    }
                    product.PriceWholesale = priceWholesale;
                    _shopSanPhamRepository.Update(product);
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

        [Route("migrationbranchproductstock")]
        [HttpGet]
        public HttpResponseMessage MigrationChannelPriceAndProduct(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var products = _shopSanPhamRepository.GetAllIds();
                var branches = _softBranchRepository.GetAllIds();
                foreach (var product in products.ToList())
                {
                    foreach (var branch in branches.ToList())
                    {
                        var stock = _softStockRepository.GetSingleByCondition(x => x.ProductId == product && x.BranchId == branch);
                        if (stock == null)
                        {
                            var nwStock = new SoftBranchProductStock();
                            nwStock.ProductId = product;
                            nwStock.BranchId = branch;
                            nwStock.StockTotal = 0;
                            nwStock.UpdatedDate = DateTime.Now;
                            nwStock.UpdatedBy = 0;
                            _softStockRepository.Add(nwStock);
                            _unitOfWork.Commit();
                        }
                    }
                }
                response = request.CreateResponse(HttpStatusCode.OK, true);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("migrationstockinsupplier")]
        [HttpGet]
        public HttpResponseMessage MigrationStockinSupplier(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var softStockin = _softStockInRepository.GetAll();
                foreach (var itemStockin in softStockin.ToList())
                {
                    if (itemStockin.CategoryId == "00" || itemStockin.CategoryId == "02")
                    {
                        var fromSuppliers = "";
                        var softStockinDetail = _softStockInDetailRepository.GetMulti(x => x.StockInId == itemStockin.Id);
                        foreach (var itemSoftStockinDetail in softStockinDetail)
                        {
                            var ctProduct = _shopSanPhamRepository.GetSingleById(itemSoftStockinDetail.ProductId);
                            if (ctProduct.SupplierId.HasValue && !fromSuppliers.Contains(ctProduct.SoftSupplier.Name))
                            {
                                fromSuppliers += ctProduct.SoftSupplier.Name + "|";
                            }
                        }
                        if (!string.IsNullOrEmpty(fromSuppliers))
                        {
                            itemStockin.FromSuppliers = fromSuppliers;
                            _softStockInRepository.Update(itemStockin);
                            _unitOfWork.Commit();
                        }
                    }
                }
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