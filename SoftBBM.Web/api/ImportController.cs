using OfficeOpenXml;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/import")]
    public class ImportController : ApiController
    {
        IUnitOfWork _unitOfWork;
        ISoftStockRepository _softStockRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        IShopSanPhamStatusRepository _shopSanPhamStatusRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        ISoftBranchRepository _softBranchRepository;
        ISoftChannelRepository _softChannelRepository;
        ISoftStockInPaymentTypeRepository _softStockInPaymentTypeRepository;
        ISoftStockInPaymentMethodRepository _softStockInPaymentMethodRepository;
        ISoftSupplierRepository _softSupplierRepository;
        IShopSanPhamCategoryRepository _shopSanPhamCategoryRepository;
        IshopbientheRepository _shopbientheRepository;

        public ImportController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IShopSanPhamStatusRepository shopSanPhamStatusRepository, IUnitOfWork unitOfWork, ISoftChannelProductPriceRepository softChannelProductPriceRepository, ISoftBranchRepository softBranchRepository, ISoftChannelRepository softChannelRepository, ISoftStockInPaymentTypeRepository softStockInPaymentTypeRepository, ISoftStockInPaymentMethodRepository softStockInPaymentMethodRepository, ISoftSupplierRepository softSupplierRepository, IShopSanPhamCategoryRepository shopSanPhamCategoryRepository, IshopbientheRepository shopbientheRepository)
        {
            _softStockRepository = softStockRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _shopSanPhamStatusRepository = shopSanPhamStatusRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _softBranchRepository = softBranchRepository;
            _softChannelRepository = softChannelRepository;
            _softStockInPaymentTypeRepository = softStockInPaymentTypeRepository;
            _softStockInPaymentMethodRepository = softStockInPaymentMethodRepository;
            _softSupplierRepository = softSupplierRepository;
            _shopSanPhamCategoryRepository = shopSanPhamCategoryRepository;
            _shopbientheRepository = shopbientheRepository;
            _unitOfWork = unitOfWork;
        }

        [Route("productsexcel")]
        [Authorize(Roles = "ProductImport")]
        [HttpPost]
        public async Task<HttpResponseMessage> ProductsExcel()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Định dạng không được server hỗ trợ");
            }

            var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/Import");
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
            var resultVm = new ResultImportViewModel();
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
                                //update
                                if (product != null)
                                {
                                    bool updated = false;
                                    if (workSheet.Cells[i, 2].Value != null)
                                    {
                                        product.tensp = workSheet.Cells[i, 2].Value.ToString();
                                        product.spurl = product.tensp.ConvertToUnSign();
                                        updated = true;
                                    }
                                    if (workSheet.Cells[i, 4].Value != null)
                                    {
                                        int priceBase = 0;
                                        int.TryParse(workSheet.Cells[i, 4].Value.ToString(), out priceBase);
                                        product.PriceBase = priceBase;
                                        updated = true;
                                    }
                                    if (workSheet.Cells[i, 5].Value != null)
                                    {
                                        int priceRef = 0;
                                        int.TryParse(workSheet.Cells[i, 5].Value.ToString(), out priceRef);
                                        product.PriceRef = priceRef;
                                        updated = true;
                                    }
                                    if (workSheet.Cells[i, 6].Value != null)
                                    {
                                        int priceOnl = 0;
                                        int.TryParse(workSheet.Cells[i, 6].Value.ToString(), out priceOnl);
                                        var channelProductPrice = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == product.id && x.ChannelId == 2);
                                        if (channelProductPrice != null)
                                        {
                                            channelProductPrice.Price = priceOnl;
                                            channelProductPrice.UpdatedBy = userId;
                                            channelProductPrice.UpdatedDate = DateTime.Now;
                                            _softChannelProductPriceRepository.Update(channelProductPrice);
                                        }
                                        else
                                        {
                                            var newChannelProductPrice = new SoftChannelProductPrice();
                                            newChannelProductPrice.ChannelId = 2;
                                            newChannelProductPrice.ProductId = product.id;
                                            newChannelProductPrice.Price = priceOnl;
                                            newChannelProductPrice.CreatedBy = userId;
                                            newChannelProductPrice.CreatedDate = DateTime.Now;
                                            _softChannelProductPriceRepository.Add(newChannelProductPrice);
                                        }
                                        updated = true;
                                    }
                                    if (workSheet.Cells[i, 7].Value != null)
                                    {
                                        int priceCha = 0;
                                        int.TryParse(workSheet.Cells[i, 7].Value.ToString(), out priceCha);
                                        var channelProductPrice = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == product.id && x.ChannelId == 1);
                                        if (channelProductPrice != null)
                                        {
                                            channelProductPrice.Price = priceCha;
                                            channelProductPrice.UpdatedBy = userId;
                                            channelProductPrice.UpdatedDate = DateTime.Now;
                                            _softChannelProductPriceRepository.Update(channelProductPrice);
                                        }
                                        else
                                        {
                                            var newChannelProductPrice = new SoftChannelProductPrice();
                                            newChannelProductPrice.ChannelId = 1;
                                            newChannelProductPrice.ProductId = product.id;
                                            newChannelProductPrice.Price = priceCha;
                                            newChannelProductPrice.CreatedBy = userId;
                                            newChannelProductPrice.CreatedDate = DateTime.Now;
                                            _softChannelProductPriceRepository.Add(newChannelProductPrice);
                                        }
                                        updated = true;
                                    }
                                    if (workSheet.Cells[i, 8].Value != null)
                                    {
                                        var categoryEx = workSheet.Cells[i, 8].Value.ToString();
                                        var category = _shopSanPhamCategoryRepository.GetSingleByCondition(x => x.Name == categoryEx);
                                        if (category != null)
                                        {
                                            product.CategoryId = category.Id;
                                            updated = true;
                                        }
                                    }
                                    if (workSheet.Cells[i, 9].Value != null)
                                    {
                                        var supplierEx = workSheet.Cells[i, 9].Value.ToString();
                                        var supplier = _softSupplierRepository.GetSingleByCondition(x => x.Name == supplierEx);
                                        if (supplier != null)
                                        {
                                            product.SupplierId = supplier.Id;
                                            updated = true;
                                        }

                                    }
                                    if (workSheet.Cells[i, 10].Value != null)
                                    {
                                        product.Barcode = workSheet.Cells[i, 10].Value.ToString();
                                        updated = true;
                                    }
                                    //product.PriceAvg = 
                                    //product.PriceWholesale =

                                    if (workSheet.Cells[i, 3].Value != null)
                                    {
                                        int stockTotalImport;
                                        if (int.TryParse(workSheet.Cells[i, 3].Value.ToString(), out stockTotalImport))
                                        {
                                            var ctStock = _softStockRepository.GetSingleByCondition(x => x.ProductId == product.id && x.BranchId == branchId);
                                            ctStock.StockTotal = stockTotalImport;
                                            _softStockRepository.Update(ctStock);
                                            updated = true;
                                        }
                                    }
                                    if (updated)
                                    {
                                        product.CreatedBy = userId;
                                        product.CreatedDate = DateTime.Now;
                                        _shopSanPhamRepository.Update(product);
                                        _unitOfWork.Commit();
                                        updatedCount++;
                                    }
                                }
                                //add
                                else
                                {
                                    //add product
                                    shop_sanpham sp = new shop_sanpham();
                                    sp.masp = code;
                                    int num;
                                    var isNum = int.TryParse(code.RemoveChar(), out num);
                                    sp.CodeSuffix = num;
                                    if (workSheet.Cells[i, 2].Value != null)
                                    {
                                        sp.tensp = workSheet.Cells[i, 2].Value.ToString();
                                        sp.spurl = sp.tensp.ConvertToUnSign();
                                    }
                                    else
                                    {
                                        sp.tensp = code;
                                        sp.spurl = code;
                                    }
                                    if (workSheet.Cells[i, 4].Value != null)
                                    {
                                        int priceBase = 0;
                                        int.TryParse(workSheet.Cells[i, 4].Value.ToString(), out priceBase);
                                        sp.PriceBase = priceBase;
                                    }
                                    else
                                        sp.PriceBase = 0;
                                    if (workSheet.Cells[i, 5].Value != null)
                                    {
                                        int priceRef = 0;
                                        int.TryParse(workSheet.Cells[i, 5].Value.ToString(), out priceRef);
                                        sp.PriceRef = priceRef;
                                    }
                                    else
                                        sp.PriceRef = 0;
                                    if (workSheet.Cells[i, 8].Value != null)
                                    {
                                        var categoryEx = workSheet.Cells[i, 8].Value.ToString();
                                        var category = _shopSanPhamCategoryRepository.GetSingleByCondition(x => x.Name == categoryEx);
                                        if (category != null)
                                            sp.CategoryId = category.Id;
                                    }
                                    if (workSheet.Cells[i, 9].Value != null)
                                    {
                                        var supplierEx = workSheet.Cells[i, 9].Value.ToString();
                                        var supplier = _softSupplierRepository.GetSingleByCondition(x => x.Name == supplierEx);
                                        if (supplier != null)
                                            sp.SupplierId = supplier.Id;
                                    }
                                    if (workSheet.Cells[i, 10].Value != null)
                                    {
                                        sp.Barcode = workSheet.Cells[i, 10].Value.ToString();
                                    }
                                    sp.PriceBaseOld = 0;
                                    sp.PriceAvg = sp.PriceBase;
                                    sp.PriceWholesale = sp.PriceBase;
                                    sp.CreatedBy = userId;
                                    sp.CreatedDate = DateTime.Now;
                                    sp.StatusId = "00";
                                    sp.hide = true;
                                    sp.FromCreate = 2;
                                    var newsp = _shopSanPhamRepository.Add(sp);
                                    _unitOfWork.Commit();

                                    if (workSheet.Cells[i, 6].Value != null)
                                    {
                                        int priceOnl = 0;
                                        int.TryParse(workSheet.Cells[i, 6].Value.ToString(), out priceOnl);
                                        var channelProductPrice = new SoftChannelProductPrice();
                                        channelProductPrice.ChannelId = 2;
                                        channelProductPrice.ProductId = newsp.id;
                                        channelProductPrice.Price = priceOnl;
                                        channelProductPrice.CreatedBy = userId;
                                        channelProductPrice.CreatedDate = DateTime.Now;
                                        _softChannelProductPriceRepository.Add(channelProductPrice);
                                    }
                                    if (workSheet.Cells[i, 7].Value != null)
                                    {
                                        int priceCha = 0;
                                        int.TryParse(workSheet.Cells[i, 7].Value.ToString(), out priceCha);
                                        var channelProductPrice = new SoftChannelProductPrice();
                                        channelProductPrice.ChannelId = 1;
                                        channelProductPrice.ProductId = newsp.id;
                                        channelProductPrice.Price = priceCha;
                                        channelProductPrice.CreatedBy = userId;
                                        channelProductPrice.CreatedDate = DateTime.Now;
                                        _softChannelProductPriceRepository.Add(channelProductPrice);
                                    }

                                    addedCount++;

                                    //add stock
                                    var branches = _softBranchRepository.GetAllIds();
                                    foreach (var item in branches.ToList())
                                    {
                                        var newStock = new SoftBranchProductStock();
                                        if (item == branchId && workSheet.Cells[i, 3].Value != null)
                                        {
                                            int stockTotalImport = 0;
                                            int.TryParse(workSheet.Cells[i, 3].Value.ToString(), out stockTotalImport);
                                            newStock.StockTotal = stockTotalImport;
                                        }
                                        else
                                            newStock.StockTotal = 0;
                                        newStock.BranchId = item;
                                        newStock.ProductId = newsp.id;
                                        newStock.CreatedBy = userId;
                                        newStock.CreatedDate = DateTime.Now;
                                        _softStockRepository.Add(newStock);
                                    }

                                    //add bien the
                                    shop_bienthe newbt = new shop_bienthe();
                                    newbt.idsp = newsp.id;
                                    newbt.title = "default";
                                    newbt.gia = 100000;
                                    newbt.giasosanh = 0;
                                    newbt.isdelete = false;
                                    _shopbientheRepository.Add(newbt);
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
                if (addedCount > 0 || updatedCount > 0)
                {
                    if (addedCount > 0 && updatedCount > 0)
                        resultStr = "Đã thêm mới " + addedCount + " sp, cập nhật " + updatedCount + " sp thành công!";
                    else if (addedCount > 0)
                        resultStr = "Đã thêm mới " + addedCount + " sp thành công!";
                    else resultStr = "Đã cập nhật " + updatedCount + " sp thành công!";
                }
                resultVm.SuccessMessage = resultStr;
            }
            return Request.CreateResponse(HttpStatusCode.OK, resultVm);
        }

        [Route("channelpricesexcel")]
        [Authorize(Roles = "ProductImport")]
        [HttpPost]
        public async Task<HttpResponseMessage> ChannelPricesExcel()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Định dạng không được server hỗ trợ");
            }

            var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/Import");
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
            int channelId = 0;
            int priceOld = 0;
            int priceNew = 0;
            string resultStr = "";
            var resultVm = new ResultImportViewModel();
            List<string> errorProduct = new List<string>();
            int.TryParse(result.FormData["userId"], out userId);
            int.TryParse(result.FormData["branchId"], out branchId);
            int.TryParse(result.FormData["channelId"], out channelId);
            try
            {
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
                                    //update
                                    if (product != null)
                                    {
                                        var price = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ChannelId == channelId && x.ProductId == product.id);
                                        if (price != null)
                                        {
                                            bool updated = false;
                                            if (workSheet.Cells[i, 2].Value != null)
                                            {
                                                int priceTmp = 0;
                                                int.TryParse(workSheet.Cells[i, 2].Value.ToString(), out priceTmp);
                                                price.Price = priceTmp;
                                                updated = true;
                                            }
                                            if (workSheet.Cells[i, 3].Value != null)
                                            {
                                                int priceTmp = 0;
                                                int.TryParse(workSheet.Cells[i, 3].Value.ToString(), out priceTmp);
                                                price.PriceDiscount = priceTmp;
                                                updated = true;
                                            }
                                            if (workSheet.Cells[i, 4].Value != null)
                                            {
                                                var val = workSheet.Cells[i, 4].Value;
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
                                                price.StartDateDiscount = dtONL;
                                                updated = true;
                                            }
                                            if (workSheet.Cells[i, 5].Value != null)
                                            {
                                                var val = workSheet.Cells[i, 5].Value;
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
                                                price.EndDateDiscount = dtEndONL;
                                                updated = true;
                                            }
                                            if (updated)
                                            {
                                                price.UpdatedBy = userId;
                                                price.UpdatedDate = DateTime.Now;
                                                _softChannelProductPriceRepository.Update(price);
                                                _unitOfWork.Commit();
                                                updatedCount++;
                                            }
                                        }
                                        else
                                        {
                                            var nwPrice = new SoftChannelProductPrice();
                                            if (workSheet.Cells[i, 2].Value != null)
                                            {
                                                int priceTmp = 0;
                                                int.TryParse(workSheet.Cells[i, 2].Value.ToString(), out priceTmp);
                                                nwPrice.Price = priceTmp;
                                            }
                                            if (workSheet.Cells[i, 3].Value != null)
                                            {
                                                int priceTmp = 0;
                                                int.TryParse(workSheet.Cells[i, 3].Value.ToString(), out priceTmp);
                                                nwPrice.PriceDiscount = priceTmp;
                                            }
                                            if (workSheet.Cells[i, 4].Value != null)
                                            {
                                                var val = workSheet.Cells[i, 4].Value;
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
                                                nwPrice.StartDateDiscount = dtONL;
                                            }
                                            if (workSheet.Cells[i, 5].Value != null)
                                            {
                                                var val = workSheet.Cells[i, 5].Value;
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
                                                nwPrice.EndDateDiscount = dtEndONL;
                                            }
                                            nwPrice.ProductId = product.id;
                                            nwPrice.ChannelId = channelId;
                                            nwPrice.CreatedBy = userId;
                                            nwPrice.CreatedDate = DateTime.Now;
                                            _softChannelProductPriceRepository.Add(nwPrice);
                                            _unitOfWork.Commit();
                                            updatedCount++;
                                        }
                                    }
                                    else
                                    {
                                        errorProduct.Add(code.Trim());
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
                    if (updatedCount > 0)
                    {
                        resultStr = "Đã cập nhật giá kênh " + updatedCount + " sp thành công!";
                    }
                    resultVm.SuccessMessage = resultStr;
                    var errorMessage = "";
                    if (errorProduct.Count > 0)
                    {
                        errorMessage = "Không tìm thấy " + errorProduct.Count + " sp: ";
                        foreach (var item in errorProduct)
                        {
                            errorMessage += item + ", ";
                        }
                        errorMessage = errorMessage.Substring(0, errorMessage.Length - 2);
                    }
                    resultVm.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, resultVm);
        }
    }
}