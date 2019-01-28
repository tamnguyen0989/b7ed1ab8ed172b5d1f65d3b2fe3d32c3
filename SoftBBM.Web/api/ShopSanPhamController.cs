using AutoMapper;
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
using System.Web.Script.Serialization;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/product")]
    public class ShopSanPhamController : ApiController
    {
        ISoftStockRepository _softStockRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        IShopSanPhamStatusRepository _shopSanPhamStatusRepository;
        ISoftChannelRepository _softChannelRepository;
        ISoftBranchRepository _softBranchRepository;
        IdonhangRepository _donhangRepository;
        IdonhangctRepository _donhangctRepository;
        IshopbientheRepository _shopbientheRepository;
        IUnitOfWork _unitOfWork;

        public ShopSanPhamController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IUnitOfWork unitOfWork, ISoftChannelProductPriceRepository softChannelProductPriceRepository, IShopSanPhamStatusRepository shopSanPhamStatusRepository, ISoftChannelRepository softChannelRepository, ISoftBranchRepository softBranchRepository, IdonhangRepository donhangRepository, IdonhangctRepository donhangctRepository, IshopbientheRepository shopbientheRepository)
        {
            _shopSanPhamRepository = shopSanPhamRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _softStockRepository = softStockRepository;
            _shopSanPhamStatusRepository = shopSanPhamStatusRepository;
            _softChannelRepository = softChannelRepository;
            _softBranchRepository = softBranchRepository;
            _donhangRepository = donhangRepository;
            _donhangctRepository = donhangctRepository;
            _shopbientheRepository = shopbientheRepository;
            _unitOfWork = unitOfWork;
        }

        [Route("getallbysupplier")]
        [HttpGet]
        public HttpResponseMessage GetAllBySupplier(HttpRequestMessage request, int supplierId)
        {
            HttpResponseMessage response = null;
            try
            {
                var shopsanphams = _shopSanPhamRepository.GetMulti(x => x.SupplierId == supplierId);
                var shopsanphamsVm = Mapper.Map<IEnumerable<shop_sanpham>, IEnumerable<ShopSanPhamViewModel>>(shopsanphams);
                response = request.CreateResponse(HttpStatusCode.OK, shopsanphamsVm);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("getbyid")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int productId)
        {
            HttpResponseMessage response = null;
            try
            {
                var shopsanpham = _shopSanPhamRepository.GetSingleById(productId);
                var shopsanphamVm = Mapper.Map<shop_sanpham, ShopSanPhamViewModel>(shopsanpham);
                response = request.CreateResponse(HttpStatusCode.OK, shopsanphamVm);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("getbystring")]
        [HttpGet]
        public HttpResponseMessage GetByString(HttpRequestMessage request, int channelId, string searchText, string searchType = "masp")
        {
            HttpResponseMessage response = null;
            searchText = searchText.Trim().ToLower();
            IEnumerable<shop_sanpham> shopsanpham = null;
            if (searchType == "masp")
            {
                shopsanpham = _shopSanPhamRepository.GetMulti(x => x.masp.ToLower() == searchText || x.Barcode == searchText).OrderBy(x => x.masp);
            }
            else
            {
                shopsanpham = _shopSanPhamRepository.GetMulti(x => x.masp.ToLower().Contains(searchText) || x.tensp.ToLower().Contains(searchText)).OrderBy(x => x.tensp).ThenBy(y => y.masp);
            }

            var shopsanphamVm = Mapper.Map<IEnumerable<shop_sanpham>, IEnumerable<SoftOrderDetailViewModel>>(shopsanpham);
            foreach (var item in shopsanphamVm)
            {
                item.Quantity = 1;
                item.Price = 0;
                item.PriceBeforeDiscount = 0;
                if (channelId == 7)
                {
                    item.Price = item.PriceWholesale;
                    item.PriceBeforeDiscount = 0;
                }
                else
                {
                    var model = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == item.id && x.ChannelId == channelId);
                    if (model != null)
                    {
                        if (model.PriceDiscount > 0)
                        {
                            item.Price = model.PriceDiscount;
                            item.PriceBeforeDiscount = model.Price;
                        }
                        else
                        {
                            item.Price = model.Price;
                            item.PriceBeforeDiscount = 0;
                        }
                    }
                }

            }
            response = request.CreateResponse(HttpStatusCode.OK, shopsanphamVm);
            return response;
        }

        [Route("getbystringjson")]
        [HttpGet]
        public HttpResponseMessage GetByStringJson(HttpRequestMessage request, int channelId, string searchText)
        {
            HttpResponseMessage response = null;
            searchText = searchText.Trim().ToLower();
            var shopsanpham = _shopSanPhamRepository.GetMulti(x => x.masp.ToLower().Contains(searchText) || x.tensp.ToLower().Contains(searchText)).OrderBy(x => x.masp);
            var shopsanphamVm = Mapper.Map<IEnumerable<shop_sanpham>, IEnumerable<SoftOrderDetailViewModel>>(shopsanpham);
            foreach (var item in shopsanphamVm)
            {
                item.Quantity = 1;
                item.Price = 0;
                item.PriceBeforeDiscount = 0;
                var model = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == item.id && x.ChannelId == channelId);
                if (model != null)
                {
                    if (model.PriceDiscount > 0)
                    {
                        item.Price = model.PriceDiscount;
                        item.PriceBeforeDiscount = model.Price;
                    }
                    else
                    {
                        item.Price = model.Price;
                        item.PriceBeforeDiscount = 0;
                    }

                }
            }
            response = request.CreateResponse(HttpStatusCode.OK, shopsanphamVm);
            return response;
        }

        //[Route("getallpaging")]
        //[HttpGet]
        //public HttpResponseMessage GetAllPaging(HttpRequestMessage request, int page, int pageSize, int branchId, string stringSearch)
        //{
        //    int currentPage = page;
        //    int currentPageSize = pageSize;
        //    HttpResponseMessage response = null;
        //    IEnumerable<shop_sanpham> products = null;
        //    int totalproducts = new int();

        //    products = _shopSanPhamRepository.GetAllPaging(currentPage, currentPageSize, out totalproducts, branchId, stringSearch).ToList();

        //    IEnumerable<ShopSanPhamSearchBookViewModel> productsVM = Mapper.Map<IEnumerable<shop_sanpham>, IEnumerable<ShopSanPhamSearchBookViewModel>>(products);

        //    foreach (var item in productsVM)
        //    {
        //        item.StockTotal = _softStockRepository.GetStockTotal(item.id, branchId);
        //        item.StockTotalAll = _softStockRepository.GetStockTotalAll(item.id);
        //    }
        //    PaginationSet<ShopSanPhamSearchBookViewModel> pagedSet = new PaginationSet<ShopSanPhamSearchBookViewModel>()
        //    {
        //        Page = currentPage,
        //        TotalCount = totalproducts,
        //        TotalPages = (int)Math.Ceiling((decimal)totalproducts / currentPageSize),
        //        Items = productsVM
        //    };



        //    response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
        //    return response;
        //}

        [Route("getallpaging")]
        [HttpPost]
        public HttpResponseMessage GetAllPaging(HttpRequestMessage request, ShopSanPhamFilterBookViewModel viewModel)
        {
            HttpResponseMessage response = null;

            int currentPage = viewModel.page;
            int currentPageSize = viewModel.pageSize;
            int currentBranchId = viewModel.branchId;
            IEnumerable<shop_sanpham> products = null;
            int totalproducts = new int();

            products = _shopSanPhamRepository.GetAllPaging(currentPage, currentPageSize, out totalproducts, currentBranchId, viewModel).ToList();

            IEnumerable<ShopSanPhamSearchBookViewModel> productsVM = Mapper.Map<IEnumerable<shop_sanpham>, IEnumerable<ShopSanPhamSearchBookViewModel>>(products);
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-30);
            startDate = UtilExtensions.ConvertStartDate(startDate);
            endDate = UtilExtensions.ConvertEndDate(endDate);
            foreach (var item in productsVM)
            {
                item.StockTotal = _softStockRepository.GetStockTotal(item.id, viewModel.branchId);
                item.StockTotalAll = _softStockRepository.GetStockTotalAll(item.id);
                item.AvgSoldQuantity = 0;
                var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.idsp == item.id);
                if (bienthe != null)
                {
                    var quantityTmp = _donhangctRepository.GetMulti(x => x.donhang.ngaydat >= startDate && x.donhang.ngaydat <= endDate).Where(y => y.IdPro == bienthe.id);
                    if (quantityTmp != null)
                        item.AvgSoldQuantity = quantityTmp.Sum(x => x.Soluong);
                }
            }

            PaginationSet<ShopSanPhamSearchBookViewModel> pagedSet = new PaginationSet<ShopSanPhamSearchBookViewModel>()
            {
                Page = currentPage,
                TotalCount = totalproducts,
                TotalPages = (int)Math.Ceiling((decimal)totalproducts / currentPageSize),
                Items = productsVM
            };



            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("getallproductstatus")]
        [HttpGet]
        public HttpResponseMessage GetAllProductStatus(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var shopSanPhamStatus = _shopSanPhamStatusRepository.GetAll().OrderBy(x => x.Id);
                var shopSanPhamStatusVm = Mapper.Map<IEnumerable<shop_sanphamStatus>, IEnumerable<ShopSanPhamStatusViewModel>>(shopSanPhamStatus);
                response = request.CreateResponse(HttpStatusCode.OK, shopSanPhamStatusVm);
                return response;
            }

            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("authenstampprint")]
        [Authorize(Roles = "StampPrint")]
        [HttpGet]
        public HttpResponseMessage AuthenStampPrint(HttpRequestMessage request)
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

        [Route("add")]
        [Authorize(Roles = "ProductAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, ShopSanPhamViewModel productVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var existShopSanPham = _shopSanPhamRepository.GetSingleByCondition(x => x.masp.Trim() == productVM.masp.Trim());
                if (existShopSanPham == null)
                {
                    shop_sanpham newShopSanPham = new shop_sanpham();
                    newShopSanPham.UpdateNewShopSanPham(productVM);
                    _shopSanPhamRepository.Add(newShopSanPham);
                    _unitOfWork.Commit();

                    var branches = _softBranchRepository.GetAllIds();
                    //var channels = _softChannelRepository.GetAllIds();
                    //foreach (var channel in channels.ToList())
                    //{
                    //    var newPrice = new SoftChannelProductPrice();
                    //    newPrice.ProductId = newShopSanPham.id;
                    //    newPrice.ChannelId = channel;
                    //    newPrice.Price = 0;
                    //    newPrice.CreatedDate = DateTime.Now;
                    //    _softChannelProductPriceRepository.Add(newPrice);
                    //}
                    foreach (var branch in branches.ToList())
                    {
                        var newstock = new SoftBranchProductStock();
                        newstock.ProductId = newShopSanPham.id;
                        newstock.BranchId = branch;
                        newstock.StockTotal = 0;
                        newstock.CreatedDate = DateTime.Now;
                        _softStockRepository.Add(newstock);
                    }
                    shop_bienthe newbt = new shop_bienthe();
                    newbt.idsp = newShopSanPham.id;
                    newbt.title = "default";
                    newbt.gia = 100000;
                    newbt.giasosanh = 0;
                    newbt.isdelete = false;
                    _shopbientheRepository.Add(newbt);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Đã có mã sản phẩm này !");

                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("authenadd")]
        [Authorize(Roles = "ProductAdd")]
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

        [Route("gennewproductcodebycategory")]
        [HttpGet]
        public HttpResponseMessage GenNewProductCodeByCategory(HttpRequestMessage request, int categoryId)
        {
            HttpResponseMessage response = null;
            try
            {
                var lastestProduct = _shopSanPhamRepository.GetMulti(x => x.CategoryId == categoryId).OrderByDescending(x => x.id).Take(1).ToList();
                if (lastestProduct.Count() > 0)
                {
                    var prefix = "";
                    var newProductCode = "";
                    int quantityZero = 0;
                    var productCode = lastestProduct[0].masp.Trim();
                    var cutProductCode = productCode.Substring(1);
                    int numberProductCode;
                    bool isNumeric = int.TryParse(cutProductCode, out numberProductCode);
                    if (isNumeric)
                    {
                        int.TryParse(cutProductCode, out numberProductCode);
                        prefix = productCode.Substring(0, 1);
                    }
                    else
                    {
                        cutProductCode = productCode.Substring(2);
                        prefix = productCode.Substring(0, 2);
                        int.TryParse(cutProductCode, out numberProductCode);
                    }
                    var lenBeforeGen = cutProductCode.Length;
                    var lenAfterGen = numberProductCode.ToString().Length;
                    numberProductCode++;
                    var strNumberAfterSum = numberProductCode.ToString();
                    if (strNumberAfterSum.Length >= lenBeforeGen)
                        newProductCode = prefix + strNumberAfterSum;
                    else
                    {
                        quantityZero = lenBeforeGen - strNumberAfterSum.Length;
                        for (int i = 1; i <= quantityZero; i++)
                        {
                            strNumberAfterSum = "0" + strNumberAfterSum;
                        }
                        newProductCode = prefix + strNumberAfterSum;
                    }
                    ProductCodeGenViewModel vm = new ProductCodeGenViewModel()
                    {
                        lastest = lastestProduct[0].masp.Trim(),
                        brandnew = newProductCode
                    };
                    response = request.CreateResponse(HttpStatusCode.OK, vm);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Nhóm đã chọn chưa có sản phẩm nào! ");
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("updatekg")]
        [HttpGet]
        public HttpResponseMessage UpdateKg(HttpRequestMessage request, int productId, double kg, double chieudai, double chieurong, double chieucao)
        {
            HttpResponseMessage response = null;
            try
            {
                var product = _shopSanPhamRepository.GetSingleById(productId);
                product.kg = kg;
                product.chieucao = chieucao;
                product.chieudai = chieudai;
                product.chieurong = chieurong;
                _shopSanPhamRepository.Update(product);
                _unitOfWork.Commit();
                var productInfo = new ShopSanPhamInformation();
                productInfo.kg = kg;
                productInfo.chieudai = chieudai;
                productInfo.chieucao = chieucao;
                productInfo.chieurong = chieurong;
                response = request.CreateResponse(HttpStatusCode.OK, productInfo);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("getinfo")]
        [HttpGet]
        public HttpResponseMessage GetInfo(HttpRequestMessage request, int productId)
        {
            HttpResponseMessage response = null;
            try
            {
                var product = _shopSanPhamRepository.GetSingleById(productId);
                var productInfo = new ShopSanPhamInformation();
                productInfo.kg = product.kg;
                productInfo.chieudai = product.chieudai;
                productInfo.chieurong = product.chieurong;
                productInfo.chieucao = product.chieucao;
                productInfo.masp = product.masp.Trim();
                productInfo.tensp = product.tensp;
                response = request.CreateResponse(HttpStatusCode.OK, productInfo);
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