using AutoMapper;
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
using System.Web.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.IO;
using System.Net.Http.Headers;
using System.Drawing;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/stock")]
    public class SoftStockController : ApiController
    {
        ISoftStockRepository _softStockRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        IShopSanPhamStatusRepository _shopSanPhamStatusRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        ISoftBranchRepository _softBranchRepository;
        ISoftChannelRepository _softChannelRepository;
        ISoftStockInPaymentTypeRepository _softStockInPaymentTypeRepository;
        ISoftStockInPaymentMethodRepository _softStockInPaymentMethodRepository;


        IUnitOfWork _unitOfWork;

        public SoftStockController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IShopSanPhamStatusRepository shopSanPhamStatusRepository, IUnitOfWork unitOfWork, ISoftChannelProductPriceRepository softChannelProductPriceRepository, ISoftBranchRepository softBranchRepository, ISoftChannelRepository softChannelRepository, ISoftStockInPaymentTypeRepository softStockInPaymentTypeRepository, ISoftStockInPaymentMethodRepository softStockInPaymentMethodRepository)
        {
            _softStockRepository = softStockRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _shopSanPhamStatusRepository = shopSanPhamStatusRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _softBranchRepository = softBranchRepository;
            _softChannelRepository = softChannelRepository;
            _softStockInPaymentTypeRepository = softStockInPaymentTypeRepository;
            _softStockInPaymentMethodRepository = softStockInPaymentMethodRepository;
            _unitOfWork = unitOfWork;
        }

        //[HttpGet]
        //[Authorize(Roles = "ProductList")]
        //[Route("search")]
        //public HttpResponseMessage Search(HttpRequestMessage request, int branchId, int? page, int? pageSize, string filter = null)
        //{
        //    int currentPage = page.Value;
        //    int currentPageSize = pageSize.Value;
        //    HttpResponseMessage response = null;
        //    IEnumerable<SoftBranchProductStock> stocks = null;
        //    int totalStocks = new int();

        //    stocks = _softStockRepository.GetAllPaging(currentPage, currentPageSize, out totalStocks, filter, branchId).ToList();

        //    IEnumerable<SoftStockViewModel> stocksVM = Mapper.Map<IEnumerable<SoftBranchProductStock>, IEnumerable<SoftStockViewModel>>(stocks);
        //    foreach (var item in stocksVM)
        //    {
        //        item.Stock_Total_All = _softStockRepository.GetStockTotalAll(item.ProductId);
        //        var priceShop = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 2 && x.ProductId == item.ProductId);
        //        if (priceShop != null)
        //            item.PriceShop = priceShop.Price.Value;
        //        else
        //        {
        //            item.PriceShop = 0;
        //            var newPrice = new SoftChannelProductPrice();
        //            newPrice.ProductId = item.ProductId;
        //            newPrice.Price = 0;
        //            newPrice.ChannelId = 2;
        //            newPrice.CreatedDate = DateTime.Now;
        //            _softChannelProductPriceRepository.Add(newPrice);
        //            _unitOfWork.Commit();
        //        }
        //        var priceWholesale = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 7 && x.ProductId == item.ProductId);
        //        if (priceWholesale != null)
        //            item.PriceWholesale = priceWholesale.Price.Value;
        //        else
        //        {
        //            item.PriceWholesale = 0;
        //            var newPrice = new SoftChannelProductPrice();
        //            newPrice.ProductId = item.ProductId;
        //            newPrice.Price = 0;
        //            newPrice.ChannelId = 7;
        //            newPrice.CreatedDate = DateTime.Now;
        //            _softChannelProductPriceRepository.Add(newPrice);
        //            _unitOfWork.Commit();
        //        }
        //    }

        //    PaginationSet<SoftStockViewModel> pagedSet = new PaginationSet<SoftStockViewModel>()
        //    {
        //        Page = currentPage,
        //        TotalCount = totalStocks,
        //        TotalPages = (int)Math.Ceiling((decimal)totalStocks / currentPageSize),
        //        Items = stocksVM
        //    };

        //    response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
        //    return response;
        //}

        [HttpPost]
        [Authorize(Roles = "ProductList")]
        [Route("search")]
        public HttpResponseMessage Search(HttpRequestMessage request, SoftStockSearchFilterViewModel softStockSearchFilterVM)
        {
            HttpResponseMessage response = null;
            IEnumerable<SoftBranchProductStock> stocks = null;
            int totalStocks = new int();

            stocks = _softStockRepository.GetAllPagingFilter(out totalStocks, softStockSearchFilterVM).ToList();

            IEnumerable<SoftStockViewModel> stocksVM = Mapper.Map<IEnumerable<SoftBranchProductStock>, IEnumerable<SoftStockViewModel>>(stocks);
            foreach (var item in stocksVM)
            {
                item.Stock_Total_All = _softStockRepository.GetStockTotalAll(item.ProductId);
                var priceShop = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 1 && x.ProductId == item.ProductId);
                item.PriceShop = 0;
                if (priceShop != null)
                    item.PriceShop = priceShop.Price.Value;
                //var priceWholesale = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 7 && x.ProductId == item.ProductId);
                //item.PriceWholesale = 0;
                //if (priceWholesale != null)
                //    item.PriceWholesale = priceWholesale.Price.Value;
                if (softStockSearchFilterVM.channelId != 7)
                {
                    item.PriceChannel = 0;
                    item.PriceDiscount = 0;
                    var priceChannel = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == softStockSearchFilterVM.channelId && x.ProductId == item.ProductId);
                    if (priceChannel != null)
                    {
                        item.PriceChannel = priceChannel.Price > 0 ? priceChannel.Price : 0;
                        item.PriceDiscount = priceChannel.PriceDiscount > 0 ? priceChannel.PriceDiscount : 0;
                        item.StartDateDiscount = priceChannel.StartDateDiscount;
                        item.EndDateDiscount = priceChannel.EndDateDiscount;
                    }
                }
                else
                    item.PriceChannel = item.shop_sanpham.PriceWholesale;

                if (item.shop_sanpham.hide == null)
                    item.shop_sanpham.hide = true;
            }

            PaginationSet<SoftStockViewModel> pagedSet = new PaginationSet<SoftStockViewModel>()
            {
                Page = softStockSearchFilterVM.page,
                TotalCount = totalStocks,
                TotalPages = (int)Math.Ceiling((decimal)totalStocks / softStockSearchFilterVM.pageSize),
                Items = stocksVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, SoftStockViewModel softStockVM)
        {
            HttpResponseMessage response = null;
            if (ModelState.IsValid)
            {
                var stock = _softStockRepository.GetSingleById(softStockVM.Id);
                stock.UpdateSoftStock(softStockVM);
                _softStockRepository.Update(stock);
                _unitOfWork.Commit();
                var responseData = Mapper.Map<SoftBranchProductStock, SoftStockViewModel>(stock);
                response = request.CreateResponse(HttpStatusCode.Created, responseData);
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray();
                string outputError = "";
                foreach (var item in errors)
                {
                    outputError += item + " | ";
                }
                response = request.CreateResponse(HttpStatusCode.BadRequest, outputError);
            }

            return response;
        }

        [HttpGet]
        [Route("getallproductstatus")]
        public HttpResponseMessage GetAllProductStatus(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var shopSanPhamStatus = _shopSanPhamStatusRepository.GetAll();
                var shopSanPhamStatusVM = Mapper.Map<IEnumerable<shop_sanphamStatus>, IEnumerable<ShopSanPhamStatusViewModel>>(shopSanPhamStatus);
                response = request.CreateResponse(HttpStatusCode.OK, shopSanPhamStatusVM);
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpGet]
        [Route("getallbyproductid")]
        [Authorize(Roles = "ProductStockAll")]
        public HttpResponseMessage GetAllByProductId(HttpRequestMessage request, int productId)
        {
            HttpResponseMessage respone = null;
            try
            {
                var branches = _softBranchRepository.GetAll();
                var branchesVm = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<BranchProductStockOutputViewModel>>(branches);
                foreach (var item in branchesVm)
                {
                    item.StockTotal = 0;
                    var stock = _softStockRepository.GetSingleByCondition(x => x.ProductId == productId && x.BranchId == item.Id);
                    if (stock != null)
                        item.StockTotal = stock.StockTotal.Value;
                }
                respone = request.CreateResponse(HttpStatusCode.OK, branchesVm);
                return respone;
            }
            catch (Exception ex)
            {
                respone = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return respone;
            }

        }

        [Route("authenedit")]
        [Authorize(Roles = "ProductEdit")]
        [HttpGet]
        public HttpResponseMessage AuthenEdit(HttpRequestMessage request)
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

        [Route("exportproductsexcel")]
        [Authorize(Roles = "ProductExport")]
        [HttpPost]
        public HttpResponseMessage ExportProductsExcel(HttpRequestMessage request, SoftStockSearchFilterViewModel softStockSearchFilterVM)
        {
            HttpResponseMessage response = null;
            try
            {

                response = request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(GetExcelSheet(softStockSearchFilterVM));
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Products.xlsx";
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        public byte[] GetExcelSheet(SoftStockSearchFilterViewModel softStockSearchFilterVM)
        {
            IEnumerable<SoftBranchProductStock> stocks = null;
            stocks = _softStockRepository.GetAllPagingFilter(softStockSearchFilterVM).ToList();
            IEnumerable<SoftStockViewModelExcel> stocksVM = Mapper.Map<IEnumerable<SoftBranchProductStock>, IEnumerable<SoftStockViewModelExcel>>(stocks);
            foreach (var item in stocksVM)
            {
                item.StockAll = _softStockRepository.GetStockTotalAll(item.Id);

                var priceONL = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 2 && x.ProductId == item.Id);
                if (priceONL != null)
                {
                    item.ONL = priceONL.Price;
                    item.ONLkm = priceONL.PriceDiscount;
                    item.ONLstart = priceONL.StartDateDiscount == null ? null : priceONL.StartDateDiscount.Value.ToString("dd-MM-yyyy");
                    item.ONLend = priceONL.EndDateDiscount == null ? null : priceONL.EndDateDiscount.Value.ToString("dd-MM-yyyy");
                }
                else
                {
                    item.ONL = 0;
                    item.ONLkm = 0;
                    //var newPrice = new SoftChannelProductPrice();
                    //newPrice.ProductId = item.Id;
                    //newPrice.Price = 0;
                    //newPrice.ChannelId = 2;
                    //newPrice.CreatedDate = DateTime.Now;
                    //_softChannelProductPriceRepository.Add(newPrice);
                }

                var priceCHA = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 1 && x.ProductId == item.Id);
                if (priceCHA != null)
                {
                    item.CHA = priceCHA.Price;
                    item.CHAkm = priceCHA.PriceDiscount;
                    item.CHAstart = priceCHA.StartDateDiscount == null ? null : priceCHA.StartDateDiscount.Value.ToString("dd-MM-yyyy");
                    item.CHAend = priceCHA.EndDateDiscount == null ? null : priceCHA.EndDateDiscount.Value.ToString("dd-MM-yyyy");
                }
                else
                {
                    item.CHA = 0;
                    item.CHAkm = 0;
                    //var newPrice = new SoftChannelProductPrice();
                    //newPrice.ProductId = item.Id;
                    //newPrice.Price = 0;
                    //newPrice.ChannelId = 1;
                    //newPrice.CreatedDate = DateTime.Now;
                    //_softChannelProductPriceRepository.Add(newPrice);
                }

                var priceLZD = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 3 && x.ProductId == item.Id);
                if (priceLZD != null)
                {
                    item.LZD = priceLZD.Price;
                    item.LZDkm = priceLZD.PriceDiscount;
                    item.LZDstart = priceLZD.StartDateDiscount == null ? null : priceLZD.StartDateDiscount.Value.ToString("dd-MM-yyyy");
                    item.LZDend = priceLZD.EndDateDiscount == null ? null : priceLZD.EndDateDiscount.Value.ToString("dd-MM-yyyy");
                }
                else
                {
                    item.LZD = 0;
                    item.LZDkm = 0;
                    //var newPrice = new SoftChannelProductPrice();
                    //newPrice.ProductId = item.Id;
                    //newPrice.Price = 0;
                    //newPrice.ChannelId = 3;
                    //newPrice.CreatedDate = DateTime.Now;
                    //_softChannelProductPriceRepository.Add(newPrice);
                }

                var priceSPE = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 4 && x.ProductId == item.Id);
                if (priceSPE != null)
                {
                    item.SPE = priceSPE.Price;
                    item.SPEkm = priceSPE.PriceDiscount;
                    item.SPEstart = priceSPE.StartDateDiscount == null ? null : priceLZD.StartDateDiscount.Value.ToString("dd-MM-yyyy");
                    item.SPEend = priceSPE.EndDateDiscount == null ? null : priceSPE.EndDateDiscount.Value.ToString("dd-MM-yyyy");
                }
                else
                {
                    item.SPE = 0;
                    item.SPEkm = 0;
                    //var newPrice = new SoftChannelProductPrice();
                    //newPrice.ProductId = item.Id;
                    //newPrice.Price = 0;
                    //newPrice.ChannelId = 4;
                    //newPrice.CreatedDate = DateTime.Now;
                    //_softChannelProductPriceRepository.Add(newPrice);
                }

                //var priceWholesale = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 7 && x.ProductId == item.ProductId);
                //if (priceWholesale != null)
                //    item.PriceWholesale = priceWholesale.Price.Value;
                //else
                //{
                //    item.PriceWholesale = 0;
                //    var newPrice = new SoftChannelProductPrice();
                //    newPrice.ProductId = item.ProductId;
                //    newPrice.Price = 0;
                //    newPrice.ChannelId = 7;
                //    newPrice.CreatedDate = DateTime.Now;
                //    _softChannelProductPriceRepository.Add(newPrice);
                //    _unitOfWork.Commit();
                //}
            }
            _unitOfWork.Commit();
            IEnumerable<SoftStockViewModelExcelNoId> stocksVMNoId = Mapper.Map<IEnumerable<SoftStockViewModelExcel>, IEnumerable<SoftStockViewModelExcelNoId>>(stocksVM);
            using (var package = new ExcelPackage())
            {
                // Tạo author cho file Excel
                package.Workbook.Properties.Author = "SoftBBM";
                // Tạo title cho file Excel
                package.Workbook.Properties.Title = "Export Products";
                // thêm tí comments vào làm màu 
                package.Workbook.Properties.Comments = "This is my generated Comments";
                // Add Sheet vào file Excel
                package.Workbook.Worksheets.Add("Products");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells["A1"].LoadFromCollection(stocksVMNoId, true, TableStyles.Dark9);
                worksheet.DefaultColWidth = 10;
                worksheet.Cells[2, 6, stocksVM.Count() + 1, 6].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[2, 7, stocksVM.Count() + 1, 7].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[2, 9, stocksVM.Count() + 1, 9].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[2, 10, stocksVM.Count() + 1, 10].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[1, 9, stocksVM.Count() + 1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 9, stocksVM.Count() + 1, 12].Style.Fill.BackgroundColor.SetColor(Color.BurlyWood);
                worksheet.Cells[2, 9, stocksVM.Count() + 1, 13].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[2, 10, stocksVM.Count() + 1, 14].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[1, 13, stocksVM.Count() + 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 13, stocksVM.Count() + 1, 16].Style.Fill.BackgroundColor.SetColor(Color.Plum);
                worksheet.Cells[2, 9, stocksVM.Count() + 1, 17].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[2, 10, stocksVM.Count() + 1, 18].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[1, 17, stocksVM.Count() + 1, 20].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 17, stocksVM.Count() + 1, 20].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
                worksheet.Cells[2, 9, stocksVM.Count() + 1, 21].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[2, 10, stocksVM.Count() + 1, 12].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[1, 21, stocksVM.Count() + 1, 24].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 21, stocksVM.Count() + 1, 24].Style.Fill.BackgroundColor.SetColor
                    (Color.DarkKhaki);

                var branch = _softBranchRepository.GetSingleById(softStockSearchFilterVM.branchId);
                if (branch != null)
                {
                    worksheet.Cells["D1"].Value = "Stock" + branch.Code;
                }
                //package.Save();
                return package.GetAsByteArray();
            }
        }

        public byte[] GetExcelSheetSample()
        {
            List<SoftStockViewModelExcelSampleImport> stocksVM = new List<SoftStockViewModelExcelSampleImport>();
            using (var package = new ExcelPackage())
            {
                // Tạo author cho file Excel
                package.Workbook.Properties.Author = "SoftBBM";
                // Tạo title cho file Excel
                package.Workbook.Properties.Title = "Export Products";
                // thêm tí comments vào làm màu 
                package.Workbook.Properties.Comments = "This is my generated Comments";
                // Add Sheet vào file Excel
                package.Workbook.Worksheets.Add("ImportProducts");
                package.Workbook.Worksheets.Add("ReadMe");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells["A1"].LoadFromCollection(stocksVM, true, TableStyles.Dark9);
                worksheet.DefaultColWidth = 10;
                worksheet.Cells[1, 6, 1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 6, 1, 9].Style.Fill.BackgroundColor.SetColor(Color.BurlyWood);
                worksheet.Cells[1, 10, 1, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 10, 1, 13].Style.Fill.BackgroundColor.SetColor(Color.Plum);
                worksheet.Cells[1, 14, 1, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 14, 1, 17].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
                worksheet.Cells[1, 18, 1, 21].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 18, 1, 21].Style.Fill.BackgroundColor.SetColor
                    (Color.DarkKhaki);

                var worksheet2 = package.Workbook.Worksheets[2];
                worksheet2.Cells["A1"].Value = "- Định dạng ngày tháng kiểu text: 'ngày-tháng-năm. Vd: ONLstart = '20-01-2018";
                worksheet2.Cells["A2"].Value = "- Thời gian ngày start : 00:00:01 , ngày end: 23:59:59";
                worksheet2.Cells["A3"].Value = "- Tồn kho được cập nhật theo kho đang truy cập ! ";
                worksheet2.Cells["A4"].Value = "- Xoá các cột Supplier, StockAll ! (Nếu import từ file export)";

                worksheet2.Cells["A6"].LoadFromCollection(stocksVM, true, TableStyles.Dark9);
                worksheet2.DefaultColWidth = 10;
                worksheet2.Cells[6, 6, 6, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet2.Cells[6, 6, 6, 9].Style.Fill.BackgroundColor.SetColor(Color.BurlyWood);
                worksheet2.Cells[6, 10, 6, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet2.Cells[6, 10, 6, 13].Style.Fill.BackgroundColor.SetColor(Color.Plum);
                worksheet2.Cells[6, 14, 6, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet2.Cells[6, 14, 6, 17].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
                worksheet2.Cells[6, 18, 6, 21].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet2.Cells[6, 18, 6, 21].Style.Fill.BackgroundColor.SetColor
                    (Color.DarkKhaki);
                worksheet2.Cells["C6"].Value = "Stock";
                worksheet2.Cells["H7"].Value = "20-01-2018";
                //var branches = _softBranchRepository.GetAll();
                //var branchesVM = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<SoftBranchImportSampleViewModel>>(branches);
                //worksheet2.Cells["A7"].Value = "Danh sách code của kho";
                //worksheet2.Cells["A8"].LoadFromCollection(branchesVM, true, TableStyles.Dark9);
                return package.GetAsByteArray();
            }
        }

        [Route("downloadimportsample")]
        [HttpGet]
        public HttpResponseMessage DownloadImportSample(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {

                response = request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(GetExcelSheetSample());
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "ImportProducts.xlsx";
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("importproductsexcel")]
        [Authorize(Roles = "ProductImport")]
        [HttpPost]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Định dạng không được server hỗ trợ");
            }

            var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/Excels");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            //do stuff with files if you wish
            if (result.FormData["userId"] == null)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bạn chưa đăng nhập.");
            }
            if (result.FormData["branchId"] == null)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bạn chưa chọn kho.");
            }

            //Upload files
            int addedCount = 0;
            int updatedCount = 0;
            int userId = 0;
            int branchId = 0;
            int priceOld = 0;
            int priceNew = 0;
            string resultStr = "";
            int.TryParse(result.FormData["userId"], out userId);
            int.TryParse(result.FormData["branchId"], out branchId);
            foreach (MultipartFileData fileData in result.FileData)
            {
                if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Yêu cầu không đúng định dạng");
                }
                string fileName = fileData.Headers.ContentDisposition.FileName;
                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }
                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }
                var dtNow = DateTime.Now;
                fileName = userId.ToString() + fileName;
                var fullPath = Path.Combine(root, fileName);
                File.Copy(fileData.LocalFileName, fullPath, true);
                //insert,update to DB
                using (var package = new ExcelPackage(new FileInfo(fullPath)))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    if (workSheet.Cells[2, 1].Value != null)
                    {
                        for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                        {
                            if (workSheet.Cells[i, 1].Value != null)
                            {
                                var code = workSheet.Cells[i, 1].Value.ToString().Trim();
                                var product = _shopSanPhamRepository.GetSingleByCondition(x => x.masp.Trim() == code);
                                bool isUpdatedProduct = false;
                                //update
                                if (product != null)
                                {
                                    var ONL = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 2 && x.ProductId == product.id);
                                    bool isUpdatedONL = false;
                                    var CHA = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 1 && x.ProductId == product.id);
                                    bool isUpdatedCHA = false;
                                    var LZD = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 3 && x.ProductId == product.id);
                                    bool isUpdatedLZD = false;
                                    var SPE = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == 4 && x.ProductId == product.id);
                                    bool isUpdatedSPE = false;

                                    for (int j = 2; j < 22; j++)
                                    {
                                        if (workSheet.Cells[i, j].Value != null)
                                        {
                                            var val = workSheet.Cells[i, j].Value;
                                            switch (j)
                                            {
                                                case 2:
                                                    product.tensp = val.ToString();
                                                    //product.spurl = val.ToString().ConvertToUnSign();
                                                    isUpdatedProduct = true;
                                                    break;
                                                case 3:
                                                    double stockTotalImport = 0;
                                                    var stockImport = _softStockRepository.GetSingleByCondition(x => x.ProductId == product.id && x.BranchId == branchId);
                                                    double.TryParse(val.ToString(), out stockTotalImport);
                                                    stockImport.StockTotal = stockTotalImport;
                                                    stockImport.UpdatedBy = userId;
                                                    stockImport.UpdatedDate = DateTime.Now;
                                                    _softStockRepository.Update(stockImport);
                                                    break;
                                                case 4:
                                                    int priceBaseOldImport = 0;
                                                    int.TryParse(val.ToString(), out priceBaseOldImport);
                                                    product.PriceBaseOld = priceBaseOldImport;
                                                    isUpdatedProduct = true;
                                                    break;
                                                case 5:
                                                    int priceBaseImport = 0;
                                                    int.TryParse(val.ToString(), out priceBaseImport);
                                                    product.PriceBase = priceBaseImport;
                                                    isUpdatedProduct = true;
                                                    break;
                                                case 6:
                                                    int priceONL = 0;
                                                    int.TryParse(val.ToString(), out priceONL);
                                                    ONL.Price = priceONL;
                                                    isUpdatedONL = true;
                                                    break;
                                                case 7:
                                                    int priceDisONL = 0;
                                                    int.TryParse(val.ToString(), out priceDisONL);
                                                    ONL.PriceDiscount = priceDisONL;
                                                    isUpdatedONL = true;
                                                    break;
                                                case 8:
                                                    var startDateONL = val.ToString().Split('-');
                                                    int dayONL = 1;
                                                    int.TryParse(startDateONL[0], out dayONL);
                                                    if (dayONL == 0) dayONL = 1;
                                                    int monthONL = 1;
                                                    int.TryParse(startDateONL[1], out monthONL);
                                                    if (monthONL == 0) monthONL = 1;
                                                    int yearONL = 2018;
                                                    int.TryParse(startDateONL[2], out yearONL);
                                                    if (yearONL == 0) yearONL = 2018;
                                                    DateTime dtONL = new DateTime(yearONL, monthONL, dayONL, 0, 0, 1);
                                                    ONL.StartDateDiscount = dtONL;
                                                    isUpdatedONL = true;
                                                    break;
                                                case 9:
                                                    var endDateONL = val.ToString().Split('-');
                                                    int dayEndONL = 1;
                                                    int.TryParse(endDateONL[0], out dayEndONL);
                                                    if (dayEndONL == 0) dayEndONL = 1;
                                                    int monthEndONL = 1;
                                                    int.TryParse(endDateONL[1], out monthEndONL);
                                                    if (monthEndONL == 0) monthEndONL = 1;
                                                    int yearEndONL = 2018;
                                                    int.TryParse(endDateONL[2], out yearEndONL);
                                                    if (yearEndONL == 0) yearEndONL = 2018;
                                                    DateTime dtEndONL = new DateTime(yearEndONL, monthEndONL, dayEndONL, 0, 0, 1);
                                                    ONL.EndDateDiscount = dtEndONL;
                                                    isUpdatedONL = true;
                                                    break;
                                                case 10:
                                                    int priceCHA = 0;
                                                    int.TryParse(val.ToString(), out priceCHA);
                                                    CHA.Price = priceCHA;
                                                    isUpdatedCHA = true;
                                                    break;
                                                case 11:
                                                    int priceDisCHA = 0;
                                                    int.TryParse(val.ToString(), out priceDisCHA);
                                                    CHA.PriceDiscount = priceDisCHA;
                                                    isUpdatedCHA = true;
                                                    break;
                                                case 12:
                                                    var startDateCHA = val.ToString().Split('-');
                                                    int dayCHA = 1;
                                                    int.TryParse(startDateCHA[0], out dayCHA);
                                                    if (dayCHA == 0) dayCHA = 1;
                                                    int monthCHA = 1;
                                                    int.TryParse(startDateCHA[1], out monthCHA);
                                                    if (monthCHA == 0) monthCHA = 1;
                                                    int yearCHA = 2018;
                                                    int.TryParse(startDateCHA[2], out yearCHA);
                                                    if (yearCHA == 0) yearCHA = 2018;
                                                    DateTime dtCHA = new DateTime(yearCHA, monthCHA, dayCHA, 0, 0, 1);
                                                    CHA.StartDateDiscount = dtCHA;
                                                    isUpdatedCHA = true;
                                                    break;
                                                case 13:
                                                    var endDateCHA = val.ToString().Split('-');
                                                    int dayEndCHA = 1;
                                                    int.TryParse(endDateCHA[0], out dayEndCHA);
                                                    if (dayEndCHA == 0) dayEndCHA = 1;
                                                    int monthEndCHA = 1;
                                                    int.TryParse(endDateCHA[1], out monthEndCHA);
                                                    if (monthEndCHA == 0) monthEndCHA = 1;
                                                    int yearEndCHA = 2018;
                                                    int.TryParse(endDateCHA[2], out yearEndCHA);
                                                    if (yearEndCHA == 0) yearEndCHA = 2018;
                                                    DateTime dtEndCHA = new DateTime(yearEndCHA, monthEndCHA, dayEndCHA, 0, 0, 1);
                                                    CHA.EndDateDiscount = dtEndCHA;
                                                    isUpdatedCHA = true;
                                                    break;
                                                case 14:
                                                    int priceLZD = 0;
                                                    int.TryParse(val.ToString(), out priceLZD);
                                                    LZD.Price = priceLZD;
                                                    isUpdatedLZD = true;
                                                    break;
                                                case 15:
                                                    int priceDisLZD = 0;
                                                    int.TryParse(val.ToString(), out priceDisLZD);
                                                    LZD.PriceDiscount = priceDisLZD;
                                                    isUpdatedLZD = true;
                                                    break;
                                                case 16:
                                                    var startDateLZD = val.ToString().Split('-');
                                                    int dayLZD = 1;
                                                    int.TryParse(startDateLZD[0], out dayLZD);
                                                    if (dayLZD == 0) dayLZD = 1;
                                                    int monthLZD = 1;
                                                    int.TryParse(startDateLZD[1], out monthLZD);
                                                    if (monthLZD == 0) monthLZD = 1;
                                                    int yearLZD = 2018;
                                                    int.TryParse(startDateLZD[2], out yearLZD);
                                                    if (yearLZD == 0) yearLZD = 2018;
                                                    DateTime dtLZD = new DateTime(yearLZD, monthLZD, dayLZD, 0, 0, 1);
                                                    LZD.StartDateDiscount = dtLZD;
                                                    isUpdatedLZD = true;
                                                    break;
                                                case 17:
                                                    var endDateLZD = val.ToString().Split('-');
                                                    int dayEndLZD = 1;
                                                    int.TryParse(endDateLZD[0], out dayEndLZD);
                                                    if (dayEndLZD == 0) dayEndLZD = 1;
                                                    int monthEndLZD = 1;
                                                    int.TryParse(endDateLZD[1], out monthEndLZD);
                                                    if (monthEndLZD == 0) monthEndLZD = 1;
                                                    int yearEndLZD = 2018;
                                                    int.TryParse(endDateLZD[2], out yearEndLZD);
                                                    if (yearEndLZD == 0) yearEndLZD = 2018;
                                                    DateTime dtEndLZD = new DateTime(yearEndLZD, monthEndLZD, dayEndLZD, 0, 0, 1);
                                                    LZD.EndDateDiscount = dtEndLZD;
                                                    isUpdatedLZD = true;
                                                    break;
                                                case 18:
                                                    int priceSPE = 0;
                                                    int.TryParse(val.ToString(), out priceSPE);
                                                    SPE.Price = priceSPE;
                                                    isUpdatedSPE = true;
                                                    break;
                                                case 19:
                                                    int priceDisSPE = 0;
                                                    int.TryParse(val.ToString(), out priceDisSPE);
                                                    SPE.PriceDiscount = priceDisSPE;
                                                    isUpdatedSPE = true;
                                                    break;
                                                case 20:
                                                    var startDateSPE = val.ToString().Split('-');
                                                    int daySPE = 1;
                                                    int.TryParse(startDateSPE[0], out daySPE);
                                                    if (daySPE == 0) daySPE = 1;
                                                    int monthSPE = 1;
                                                    int.TryParse(startDateSPE[1], out monthSPE);
                                                    if (monthSPE == 0) monthSPE = 1;
                                                    int yearSPE = 2018;
                                                    int.TryParse(startDateSPE[2], out yearSPE);
                                                    if (yearSPE == 0) yearSPE = 2018;
                                                    DateTime dtSPE = new DateTime(yearSPE, monthSPE, daySPE, 0, 0, 1);
                                                    SPE.StartDateDiscount = dtSPE;
                                                    isUpdatedSPE = true;
                                                    break;
                                                case 21:
                                                    var endDateSPE = val.ToString().Split('-');
                                                    int dayEndSPE = 1;
                                                    int.TryParse(endDateSPE[0], out dayEndSPE);
                                                    if (dayEndSPE == 0) dayEndSPE = 1;
                                                    int monthEndSPE = 1;
                                                    int.TryParse(endDateSPE[1], out monthEndSPE);
                                                    if (monthEndSPE == 0) monthEndSPE = 1;
                                                    int yearEndSPE = 2018;
                                                    int.TryParse(endDateSPE[2], out yearEndSPE);
                                                    if (yearEndSPE == 0) yearEndSPE = 2018;
                                                    DateTime dtEndSPE = new DateTime(yearEndSPE, monthEndSPE, dayEndSPE, 0, 0, 1);
                                                    SPE.EndDateDiscount = dtEndSPE;
                                                    isUpdatedSPE = true;
                                                    break;
                                            }
                                        }
                                    }
                                    if (isUpdatedProduct == true)
                                    {
                                        product.UpdatedBy = userId;
                                        product.UpdatedDate = DateTime.Now;
                                        _shopSanPhamRepository.Update(product);
                                    }
                                    if (isUpdatedONL == true)
                                    {
                                        ONL.UpdatedBy = userId;
                                        ONL.UpdatedDate = DateTime.Now;
                                        _softChannelProductPriceRepository.Update(ONL);
                                    }
                                    if (isUpdatedCHA == true)
                                    {
                                        CHA.UpdatedBy = userId;
                                        CHA.UpdatedDate = DateTime.Now;
                                        _softChannelProductPriceRepository.Update(CHA);
                                    }
                                    if (isUpdatedLZD == true)
                                    {
                                        LZD.UpdatedBy = userId;
                                        LZD.UpdatedDate = DateTime.Now;
                                        _softChannelProductPriceRepository.Update(LZD);
                                    }
                                    if (isUpdatedSPE == true)
                                    {
                                        SPE.UpdatedBy = userId;
                                        SPE.UpdatedDate = DateTime.Now;
                                        _softChannelProductPriceRepository.Update(SPE);
                                    }
                                    if (isUpdatedProduct || isUpdatedONL || isUpdatedCHA || isUpdatedLZD || isUpdatedSPE)
                                        updatedCount++;
                                    _unitOfWork.Commit();
                                }
                                //add
                                else
                                {
                                    shop_sanpham sp = new shop_sanpham();
                                    sp.masp = code;
                                    if (workSheet.Cells[i, 2].Value != null)
                                    {
                                        sp.tensp = workSheet.Cells[i, 2].Value.ToString();
                                        //sp.spurl = sp.tensp.ConvertToUnSign();
                                    }
                                    if (workSheet.Cells[i, 4].Value != null)
                                    {
                                        int priceBaseOld = 0;
                                        int.TryParse(workSheet.Cells[i, 4].Value.ToString(), out priceBaseOld);
                                        sp.PriceBaseOld = priceBaseOld;
                                    }
                                    else
                                        sp.PriceBaseOld = 0;
                                    if (workSheet.Cells[i, 5].Value != null)
                                    {
                                        int priceBase = 0;
                                        int.TryParse(workSheet.Cells[i, 5].Value.ToString(), out priceBase);
                                        sp.PriceBase = priceBase;
                                    }
                                    else
                                        sp.PriceBase = 0;
                                    sp.PriceAvg = 0;
                                    sp.PriceRef = 0;
                                    sp.CreatedBy = userId;
                                    sp.CreatedDate = DateTime.Now;
                                    sp.StatusId = "00";
                                    var newsp = _shopSanPhamRepository.Add(sp);
                                    _unitOfWork.Commit();
                                    addedCount++;

                                    var branches = _softBranchRepository.GetAllIds();
                                    foreach (var item in branches.ToList())
                                    {
                                        if (item == branchId && workSheet.Cells[i, 3].Value != null)
                                        {
                                            var newStock = new SoftBranchProductStock();
                                            newStock.ProductId = newsp.id;
                                            newStock.BranchId = item;
                                            int stockTotalImport = 0;
                                            int.TryParse(workSheet.Cells[i, 3].Value.ToString(), out stockTotalImport);
                                            newStock.StockTotal = stockTotalImport;
                                            newStock.CreatedBy = userId;
                                            newStock.CreatedDate = DateTime.Now;
                                            _softStockRepository.Add(newStock);
                                        }
                                    }

                                    var channels = _softChannelRepository.GetAllIds();
                                    foreach (var item in channels.ToList())
                                    {
                                        switch (item)
                                        {
                                            case 2:
                                                var newPrice = new SoftChannelProductPrice();
                                                newPrice.ProductId = newsp.id;
                                                newPrice.ChannelId = 2;
                                                if (workSheet.Cells[i, 6].Value != null)
                                                {
                                                    int priceONL = 0;
                                                    int.TryParse(workSheet.Cells[i, 6].Value.ToString(), out priceONL);
                                                    newPrice.Price = priceONL;
                                                }
                                                else
                                                    newPrice.Price = 0;
                                                if (workSheet.Cells[i, 7].Value != null)
                                                {
                                                    int priceDiscountONL = 0;
                                                    int.TryParse(workSheet.Cells[i, 7].Value.ToString(), out priceDiscountONL);
                                                    newPrice.PriceDiscount = priceDiscountONL;
                                                }
                                                else
                                                    newPrice.PriceDiscount = 0;
                                                if (workSheet.Cells[i, 8].Value != null)
                                                {
                                                    var startDateONL = workSheet.Cells[i, 8].Value.ToString().Split('-');
                                                    int dayONL = 1;
                                                    int.TryParse(startDateONL[0], out dayONL);
                                                    if (dayONL == 0) dayONL = 1;
                                                    int monthONL = 1;
                                                    int.TryParse(startDateONL[1], out monthONL);
                                                    if (monthONL == 0) monthONL = 1;
                                                    int yearONL = 2018;
                                                    int.TryParse(startDateONL[2], out yearONL);
                                                    if (yearONL == 0) yearONL = 2018;
                                                    DateTime dtONL = new DateTime(yearONL, monthONL, dayONL, 0, 0, 1);
                                                    newPrice.StartDateDiscount = dtONL;
                                                }
                                                if (workSheet.Cells[i, 9].Value != null)
                                                {
                                                    var endDateONL = workSheet.Cells[i, 9].Value.ToString().Split('-');
                                                    int dayEndONL = 1;
                                                    int.TryParse(endDateONL[0], out dayEndONL);
                                                    if (dayEndONL == 0) dayEndONL = 1;
                                                    int monthONL = 1;
                                                    int.TryParse(endDateONL[1], out monthONL);
                                                    if (monthONL == 0) monthONL = 1;
                                                    int yearONL = 2018;
                                                    int.TryParse(endDateONL[2], out yearONL);
                                                    if (yearONL == 0) yearONL = 2018;
                                                    DateTime dtEndONL = new DateTime(yearONL, monthONL, dayEndONL, 0, 0, 1);
                                                    newPrice.EndDateDiscount = dtEndONL;
                                                }
                                                newPrice.CreatedBy = userId;
                                                newPrice.CreatedDate = DateTime.Now;
                                                if (newPrice.Price > 0 || newPrice.PriceDiscount > 0)
                                                    _softChannelProductPriceRepository.Add(newPrice);
                                                break;
                                            case 1:
                                                var newPriceCHA = new SoftChannelProductPrice();
                                                newPriceCHA.ProductId = newsp.id;
                                                newPriceCHA.ChannelId = 1;
                                                if (workSheet.Cells[i, 10].Value != null)
                                                {
                                                    int priceCHA = 0;
                                                    int.TryParse(workSheet.Cells[i, 10].Value.ToString(), out priceCHA);
                                                    newPriceCHA.Price = priceCHA;
                                                }
                                                else
                                                    newPriceCHA.Price = 0;
                                                if (workSheet.Cells[i, 11].Value != null)
                                                {
                                                    int priceDiscountCHA = 0;
                                                    int.TryParse(workSheet.Cells[i, 11].Value.ToString(), out priceDiscountCHA);
                                                    newPriceCHA.PriceDiscount = priceDiscountCHA;
                                                }
                                                else
                                                    newPriceCHA.PriceDiscount = 0;
                                                if (workSheet.Cells[i, 12].Value != null)
                                                {
                                                    var startDateCHA = workSheet.Cells[i, 12].Value.ToString().Split('-');
                                                    int dayCHA = 1;
                                                    int.TryParse(startDateCHA[0], out dayCHA);
                                                    if (dayCHA == 0) dayCHA = 1;
                                                    int monthCHA = 1;
                                                    int.TryParse(startDateCHA[1], out monthCHA);
                                                    if (monthCHA == 0) monthCHA = 1;
                                                    int yearCHA = 2018;
                                                    int.TryParse(startDateCHA[2], out yearCHA);
                                                    if (yearCHA == 0) yearCHA = 2018;
                                                    DateTime dtCHA = new DateTime(yearCHA, monthCHA, dayCHA, 0, 0, 1);
                                                    newPriceCHA.StartDateDiscount = dtCHA;
                                                }
                                                if (workSheet.Cells[i, 13].Value != null)
                                                {
                                                    var endDateCHA = workSheet.Cells[i, 13].Value.ToString().Split('-');
                                                    int dayEndCHA = 1;
                                                    int.TryParse(endDateCHA[0], out dayEndCHA);
                                                    if (dayEndCHA == 0) dayEndCHA = 1;
                                                    int monthCHA = 1;
                                                    int.TryParse(endDateCHA[1], out monthCHA);
                                                    if (monthCHA == 0) monthCHA = 1;
                                                    int yearCHA = 2018;
                                                    int.TryParse(endDateCHA[2], out yearCHA);
                                                    if (yearCHA == 0) yearCHA = 2018;
                                                    DateTime dtEndCHA = new DateTime(yearCHA, monthCHA, dayEndCHA, 0, 0, 1);
                                                    newPriceCHA.EndDateDiscount = dtEndCHA;
                                                }
                                                newPriceCHA.CreatedBy = userId;
                                                newPriceCHA.CreatedDate = DateTime.Now;
                                                if (newPriceCHA.Price > 0 || newPriceCHA.PriceDiscount > 0)
                                                    _softChannelProductPriceRepository.Add(newPriceCHA);
                                                break;
                                            case 3:
                                                var newPriceLZD = new SoftChannelProductPrice();
                                                newPriceLZD.ProductId = newsp.id;
                                                newPriceLZD.ChannelId = 3;
                                                if (workSheet.Cells[i, 14].Value != null)
                                                {
                                                    int priceLZD = 0;
                                                    int.TryParse(workSheet.Cells[i, 14].Value.ToString(), out priceLZD);
                                                    newPriceLZD.Price = priceLZD;
                                                }
                                                else
                                                    newPriceLZD.Price = 0;
                                                if (workSheet.Cells[i, 15].Value != null)
                                                {
                                                    int priceDiscountLZD = 0;
                                                    int.TryParse(workSheet.Cells[i, 15].Value.ToString(), out priceDiscountLZD);
                                                    newPriceLZD.PriceDiscount = priceDiscountLZD;
                                                }
                                                else
                                                    newPriceLZD.PriceDiscount = 0;
                                                if (workSheet.Cells[i, 16].Value != null)
                                                {
                                                    var startDateLZD = workSheet.Cells[i, 16].Value.ToString().Split('-');
                                                    int dayLZD = 1;
                                                    int.TryParse(startDateLZD[0], out dayLZD);
                                                    if (dayLZD == 0) dayLZD = 1;
                                                    int monthLZD = 1;
                                                    int.TryParse(startDateLZD[1], out monthLZD);
                                                    if (monthLZD == 0) monthLZD = 1;
                                                    int yearLZD = 2018;
                                                    int.TryParse(startDateLZD[2], out yearLZD);
                                                    if (yearLZD == 0) yearLZD = 2018;
                                                    DateTime dtLZD = new DateTime(yearLZD, monthLZD, dayLZD, 0, 0, 1);
                                                    newPriceLZD.StartDateDiscount = dtLZD;
                                                }
                                                if (workSheet.Cells[i, 17].Value != null)
                                                {
                                                    var endDateLZD = workSheet.Cells[i, 17].Value.ToString().Split('-');
                                                    int dayEndLZD = 1;
                                                    int.TryParse(endDateLZD[0], out dayEndLZD);
                                                    if (dayEndLZD == 0) dayEndLZD = 1;
                                                    int monthLZD = 1;
                                                    int.TryParse(endDateLZD[1], out monthLZD);
                                                    if (monthLZD == 0) monthLZD = 1;
                                                    int yearLZD = 2018;
                                                    int.TryParse(endDateLZD[2], out yearLZD);
                                                    if (yearLZD == 0) yearLZD = 2018;
                                                    DateTime dtEndLZD = new DateTime(yearLZD, monthLZD, dayEndLZD, 0, 0, 1);
                                                    newPriceLZD.EndDateDiscount = dtEndLZD;
                                                }
                                                newPriceLZD.CreatedBy = userId;
                                                newPriceLZD.CreatedDate = DateTime.Now;
                                                if (newPriceLZD.Price > 0 || newPriceLZD.PriceDiscount > 0)
                                                    _softChannelProductPriceRepository.Add(newPriceLZD);
                                                break;
                                            case 4:
                                                var newPriceSPE = new SoftChannelProductPrice();
                                                newPriceSPE.ProductId = newsp.id;
                                                newPriceSPE.ChannelId = 4;
                                                if (workSheet.Cells[i, 18].Value != null)
                                                {
                                                    int priceSPE = 0;
                                                    int.TryParse(workSheet.Cells[i, 18].Value.ToString(), out priceSPE);
                                                    newPriceSPE.Price = priceSPE;
                                                }
                                                else
                                                    newPriceSPE.Price = 0;
                                                if (workSheet.Cells[i, 19].Value != null)
                                                {
                                                    int priceDiscountSPE = 0;
                                                    int.TryParse(workSheet.Cells[i, 19].Value.ToString(), out priceDiscountSPE);
                                                    newPriceSPE.PriceDiscount = priceDiscountSPE;
                                                }
                                                else
                                                    newPriceSPE.PriceDiscount = 0;
                                                if (workSheet.Cells[i, 20].Value != null)
                                                {
                                                    var startDateSPE = workSheet.Cells[i, 20].Value.ToString().Split('-');
                                                    int daySPE = 1;
                                                    int.TryParse(startDateSPE[0], out daySPE);
                                                    if (daySPE == 0) daySPE = 1;
                                                    int monthSPE = 1;
                                                    int.TryParse(startDateSPE[1], out monthSPE);
                                                    if (monthSPE == 0) monthSPE = 1;
                                                    int yearSPE = 2018;
                                                    int.TryParse(startDateSPE[2], out yearSPE);
                                                    if (yearSPE == 0) yearSPE = 2018;
                                                    DateTime dtSPE = new DateTime(yearSPE, monthSPE, daySPE, 0, 0, 1);
                                                    newPriceSPE.StartDateDiscount = dtSPE;
                                                }
                                                if (workSheet.Cells[i, 21].Value != null)
                                                {
                                                    var endDateSPE = workSheet.Cells[i, 21].Value.ToString().Split('-');
                                                    int dayEndSPE = 1;
                                                    int.TryParse(endDateSPE[0], out dayEndSPE);
                                                    if (dayEndSPE == 0) dayEndSPE = 1;
                                                    int monthSPE = 1;
                                                    int.TryParse(endDateSPE[1], out monthSPE);
                                                    if (monthSPE == 0) monthSPE = 1;
                                                    int yearSPE = 2018;
                                                    int.TryParse(endDateSPE[2], out yearSPE);
                                                    if (yearSPE == 0) yearSPE = 2018;
                                                    DateTime dtEndSPE = new DateTime(yearSPE, monthSPE, dayEndSPE, 0, 0, 1);
                                                    newPriceSPE.EndDateDiscount = dtEndSPE;
                                                }
                                                newPriceSPE.CreatedBy = userId;
                                                newPriceSPE.CreatedDate = DateTime.Now;
                                                if (newPriceSPE.Price > 0 || newPriceSPE.PriceDiscount > 0)
                                                    _softChannelProductPriceRepository.Add(newPriceSPE);
                                                break;
                                        }
                                    }
                                    _unitOfWork.Commit();
                                }
                            }
                        }
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "File rỗng!!!");

                }
                string[] files = Directory.GetFiles(root);
                foreach (string file in files)
                {
                    File.Delete(file);
                }

                if (addedCount > 0 && updatedCount > 0)
                    resultStr = "Đã thêm mới " + addedCount + " sp, cập nhật " + updatedCount + " sp thành công!";
                else if (addedCount > 0)
                    resultStr = "Đã thêm mới " + addedCount + " sp thành công!";
                else resultStr = "Đã cập nhật " + updatedCount + " sp thành công!";

            }
            return Request.CreateResponse(HttpStatusCode.OK, resultStr);
        }

        [Route("authenexport")]
        [Authorize(Roles = "ProductExport")]
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

        [Route("authenimport")]
        [Authorize(Roles = "ProductImport")]
        [HttpGet]
        public HttpResponseMessage AuthenImport(HttpRequestMessage request)
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

        [Route("updateproductchannelprice")]
        [Authorize(Roles = "ProductEdit")]
        [HttpPost]
        public HttpResponseMessage UpdateProductChannelPrice(HttpRequestMessage request, ProductChannelPriceInputViewModel productChannelPriceInputVm)
        {
            HttpResponseMessage response = null;
            try
            {
                if (productChannelPriceInputVm.startDateDiscount != null)
                    productChannelPriceInputVm.startDateDiscount = UtilExtensions.ConvertStartDate(productChannelPriceInputVm.startDateDiscount.Value.ToLocalTime());
                if (productChannelPriceInputVm.endDateDiscount != null)
                    productChannelPriceInputVm.endDateDiscount = UtilExtensions.ConvertEndDate(productChannelPriceInputVm.endDateDiscount.Value.ToLocalTime());

                var oldPrice = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == productChannelPriceInputVm.channelId && x.ProductId == productChannelPriceInputVm.productId);
                if (oldPrice != null)
                {
                    oldPrice.Price = productChannelPriceInputVm.priceChannel;
                    oldPrice.PriceDiscount = productChannelPriceInputVm.priceDiscount;
                    oldPrice.StartDateDiscount = productChannelPriceInputVm.startDateDiscount;
                    oldPrice.EndDateDiscount = productChannelPriceInputVm.endDateDiscount;
                    _softChannelProductPriceRepository.Update(oldPrice);
                }
                else
                {
                    var newPrice = new SoftChannelProductPrice();
                    newPrice.ProductId = productChannelPriceInputVm.productId;
                    newPrice.ChannelId = productChannelPriceInputVm.channelId;
                    newPrice.Price = productChannelPriceInputVm.priceChannel == null ? 0 : productChannelPriceInputVm.priceChannel;
                    newPrice.PriceDiscount = productChannelPriceInputVm.priceDiscount == null ? 0 : productChannelPriceInputVm.priceDiscount;
                    newPrice.StartDateDiscount = productChannelPriceInputVm.startDateDiscount;
                    newPrice.EndDateDiscount = productChannelPriceInputVm.endDateDiscount;
                    oldPrice = _softChannelProductPriceRepository.Add(newPrice);
                }
                if (productChannelPriceInputVm.channelId == 2)
                {
                    var product = _shopSanPhamRepository.GetSingleById(productChannelPriceInputVm.productId);
                    product.PriceWholesale = UtilExtensions.GetPriceWholesaleByPriceAvgOnl(product.PriceAvg, oldPrice.Price);
                    _shopSanPhamRepository.Update(product);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("detailproduct")]
        [Authorize(Roles = "ProductEdit")]
        [HttpGet]
        public HttpResponseMessage DetailProduct(HttpRequestMessage request, int productId)
        {
            HttpResponseMessage response = null;
            try
            {
                var product = _shopSanPhamRepository.GetSingleById(productId);
                var productVm = Mapper.Map<shop_sanpham, DetailProductOutputViewModel>(product);
                productVm.productCode = productVm.productCode.Trim();
                response = request.CreateResponse(HttpStatusCode.OK, productVm);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("updatedetailproduct")]
        [Authorize(Roles = "ProductEdit")]
        [HttpPost]
        public HttpResponseMessage UpdateDetailProduct(HttpRequestMessage request, DetailProductInputViewModel detailProductInputVm)
        {
            HttpResponseMessage response = null;
            try
            {
                var product = _shopSanPhamRepository.GetSingleById(detailProductInputVm.id);
                int num;
                var isNum = int.TryParse(detailProductInputVm.productCode.RemoveChar(), out num);
                product.CodeSuffix = num;
                product.masp = detailProductInputVm.productCode;
                product.Barcode = detailProductInputVm.barCode;
                product.tensp = detailProductInputVm.name;
                product.PriceAvg = detailProductInputVm.priceAvg;
                product.PriceBase = detailProductInputVm.priceBase;
                product.PriceBaseOld = detailProductInputVm.priceBaseOld;
                product.PriceRef = detailProductInputVm.priceRef;
                product.CategoryId = detailProductInputVm.categoryId;
                product.SupplierId = detailProductInputVm.supplierId;
                product.StatusId = detailProductInputVm.statusId;
                product.ShopeeId = detailProductInputVm.shopeeId;
                if (detailProductInputVm.statusId == "01" || detailProductInputVm.statusId == "02" || detailProductInputVm.statusId == "03")
                    product.ischeckout = true;
                else
                    product.ischeckout = false;
                product.UpdatedBy = detailProductInputVm.userId;
                product.UpdatedDate = DateTime.Now;
                _shopSanPhamRepository.Update(product);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("exportpricewholesalesexcel")]
        [Authorize(Roles = "ProductExport")]
        [HttpPost]
        public HttpResponseMessage ExportPricewholesaleExcel(HttpRequestMessage request, SoftStockSearchFilterViewModel softStockSearchFilterVM)
        {
            HttpResponseMessage response = null;
            try
            {

                response = request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(GetPriceWholesaleSheet(softStockSearchFilterVM));
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Products.xlsx";
                return response;
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        public byte[] GetPriceWholesaleSheet(SoftStockSearchFilterViewModel softStockSearchFilterVM)
        {
            var channelId = 2;
            List<ExportPriceNoIdViewModel> exportPricesExcel = new List<ExportPriceNoIdViewModel>();

            var stocks = _softStockRepository.GetAllPagingFilter(softStockSearchFilterVM).ToList();
            //IEnumerable<ExportPriceViewModel> stocksVM = Mapper.Map<IEnumerable<SoftBranchProductStock>, IEnumerable<ExportPriceViewModel>>(stocks);

            List<ExportPriceViewModel> stocksVM = new List<ExportPriceViewModel>();
            if (stocks.Count > 0)
            {
                foreach (var item in stocks)
                {
                    if (item.shop_sanpham != null)
                    {
                        var itemStock = new ExportPriceNoIdViewModel();
                        itemStock.Code = item.shop_sanpham.masp;
                        itemStock.Name = item.shop_sanpham.tensp;
                        itemStock.PriceOld = item.shop_sanpham.PriceBaseOld.HasValue ? item.shop_sanpham.PriceBaseOld.Value : 0;
                        itemStock.PriceBase = item.shop_sanpham.PriceBase.HasValue ? item.shop_sanpham.PriceBase.Value : 0;
                        itemStock.PriceAvg = item.shop_sanpham.PriceAvg.HasValue ? item.shop_sanpham.PriceAvg.Value : 0;
                        itemStock.PriceWholesale = item.shop_sanpham.PriceWholesale.HasValue ? item.shop_sanpham.PriceWholesale.Value : 0;
                        var price = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == item.shop_sanpham.id && x.ChannelId == channelId);
                        itemStock.PriceChannel = price == null ? 0 : price.Price.Value;
                        itemStock.Url = !string.IsNullOrEmpty(item.shop_sanpham.spurl) ? "https://babymart.vn/tin-tuc/" + item.shop_sanpham.spurl + ".html" : "";
                        exportPricesExcel.Add(itemStock);
                    }

                }
                exportPricesExcel = exportPricesExcel.OrderBy(x => x.Code).ToList();
            }


            //foreach (var item in stocksVM)
            //{
            //    var price = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == item.id && x.ChannelId == channelId);
            //    item.PriceChannel = price.Price == null ? 0 : price.Price.Value;
            //}
            //exportPricesExcel = Mapper.Map<IEnumerable<ExportPriceViewModel>, IEnumerable<ExportPriceNoIdViewModel>>(stocksVM);

            var len = exportPricesExcel.Count();
            using (var package = new ExcelPackage())
            {
                // Tạo author cho file Excel
                package.Workbook.Properties.Author = "SoftBBM";
                // Tạo title cho file Excel
                package.Workbook.Properties.Title = "Export Products";
                // thêm tí comments vào làm màu 
                package.Workbook.Properties.Comments = "This is my generated Comments";
                // Add Sheet vào file Excel
                package.Workbook.Worksheets.Add("Products");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells["A1"].LoadFromCollection(exportPricesExcel, true, TableStyles.Dark9);
                worksheet.DefaultColWidth = 10;
                worksheet.Cells["A1"].Value = "Mã";
                worksheet.Cells["B1"].Value = "Tên";
                worksheet.Cells["C1"].Value = "Giá nhập cũ";
                worksheet.Cells[2, 3, len + 1, 3].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D1"].Value = "Giá nhập mới";
                worksheet.Cells[2, 4, len + 1, 4].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["E1"].Value = "Giá cơ bản";
                worksheet.Cells[2, 5, len + 1, 5].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["F1"].Value = "Giá sỉ";
                worksheet.Cells[2, 6, len + 1, 6].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["G1"].Value = "Giá online";
                worksheet.Cells[2, 7, len + 1, 7].Style.Numberformat.Format = "#,##0";

                //package.Save();
                return package.GetAsByteArray();
            }
        }
    }
}