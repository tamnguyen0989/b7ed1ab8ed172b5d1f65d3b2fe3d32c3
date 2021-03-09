using Newtonsoft.Json;
using SoftBBM.Web.Common;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Enum;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/shopee")]
    public class ShopeeController : ApiController
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
        IShopeeRepository _shopeeRepository;
        IShopSanPhamLogRepository _shopSanPhamLogRepository;
        ISoftPointUpdateLogRepository _softPointUpdateLogRepository;
        IkhachhangRepository _khachhangRepository;
        IdonhangchuyenphattpRepository _donhangchuyenphattpRepository;
        IdonhangchuyenphattinhRepository _donhangchuyenphattinhRepository;
        IUnitOfWork _unitOfWork;

        public ShopeeController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IUnitOfWork unitOfWork, ISoftChannelProductPriceRepository softChannelProductPriceRepository, IShopSanPhamStatusRepository shopSanPhamStatusRepository, ISoftChannelRepository softChannelRepository, ISoftBranchRepository softBranchRepository, IdonhangRepository donhangRepository, IdonhangctRepository donhangctRepository, IshopbientheRepository shopbientheRepository, IShopeeRepository shopeeRepository, IShopSanPhamLogRepository shopSanPhamLogRepository, ISoftPointUpdateLogRepository softPointUpdateLogRepository, IkhachhangRepository khachhangRepository, IdonhangchuyenphattinhRepository donhangchuyenphattinhRepository, IdonhangchuyenphattpRepository donhangchuyenphattpRepository)
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
            _shopeeRepository = shopeeRepository;
            _shopSanPhamLogRepository = shopSanPhamLogRepository;
            _softPointUpdateLogRepository = softPointUpdateLogRepository;
            _khachhangRepository = khachhangRepository;
            _donhangchuyenphattinhRepository = donhangchuyenphattinhRepository;
            _donhangchuyenphattpRepository = donhangchuyenphattpRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("hook")]
        public async Task<HttpResponseMessage> Hook(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;

            string content = await request.Content.ReadAsStringAsync();

            //var contentLogBeforeDese = new SoftPointUpdateLog();
            //contentLogBeforeDese.Description = "content before desri: " + content;
            //contentLogBeforeDese.CreatedDate = DateTime.Now;
            //_softPointUpdateLogRepository.Add(contentLogBeforeDese);
            //_unitOfWork.Commit();

            var jsonContent = JsonConvert.DeserializeObject<ShopeeHookInpVM>(content);
            try
            {


                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "content: " + content;
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();


                if (jsonContent != null)
                {
                    if (jsonContent.Code == 0)
                    {
                        //var okLog = new SoftPointUpdateLog("authen: ");
                        //okLog.Description += "ok";
                        //_softPointUpdateLogRepository.Add(okLog);
                        //_unitOfWork.Commit();
                        return request.CreateResponse(HttpStatusCode.OK);
                    }
                    var author = request.Headers.GetValues("Authorization").FirstOrDefault();

                    //var jsonContentLog = new SoftPointUpdateLog("jsonContent: ");
                    //jsonContentLog.Description += JsonConvert.SerializeObject(jsonContent);
                    //_softPointUpdateLogRepository.Add(jsonContentLog);
                    //_unitOfWork.Commit();

                    if (author != null)
                    {
                        string url = Request.RequestUri.AbsoluteUri;
                        string dataJson = content;
                        string signatureBaseString = url + '|' + dataJson;
                        string signatureAuth = _shopeeRepository.createSignatureNoPass(signatureBaseString);
                        if (signatureAuth != author.ToString())
                        {
                            //var signatureLog = new SoftPointUpdateLog("Fail Signature: "+ author);
                            //_softPointUpdateLogRepository.Add(signatureLog);
                            //_unitOfWork.Commit();
                            return response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                        }
                    }
                    else
                        return response = request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
                    if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.UNPAID)
                    {
                        //var newLog = new shop_sanphamLogs();
                        //newLog.StockTotal = 0;
                        //newLog.StockTotalAll = 0;
                        //newLog.Description ="Order status update push: "+ JsonConvert.SerializeObject(jsonContent);
                        //_shopSanPhamLogRepository.Add(newLog);
                        //_unitOfWork.Commit();

                        var orderDB = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (orderDB == null)
                        {
                            //thêm mới đơn hàng
                            var orderShopee = _shopeeRepository.getOrder(jsonContent.Data.Ordersn);
                            if (orderShopee != null)
                            {
                                var info = orderShopee.recipient_address;
                                var phone = info.phone;
                                var phone2 = info.phone;
                                if (phone2.Substring(0, 2) == "84")
                                {
                                    phone2 = "0" + phone.Substring(2);
                                }

                                var khachang = _khachhangRepository.GetMulti(x => x.dienthoai == phone || x.dienthoai == phone2).LastOrDefault();
                                if (khachang == null)
                                {
                                    int? idtp = null;
                                    int? idquan = null;
                                    var tphoName = info.state.ToLower().Replace("tp. hồ chí minh", "TP.HCM");
                                    var tpho = _donhangchuyenphattpRepository.GetSingleByCondition(x => x.tentp.Contains(tphoName));
                                    if (tpho != null)
                                        idtp = tpho.id;
                                    var quanName = info.city.ToLower().Replace("quận ", "").Replace("huyện ", "").Replace("thị xã ", "").Replace("tp. hồ chí minh", "TP.HCM");
                                    var quan = _donhangchuyenphattinhRepository.GetSingleByCondition(x => x.tentinh.Contains(quanName));
                                    if (quan != null)
                                        idquan = quan.id;

                                    var newKH = new khachhang();
                                    newKH.UpdatekhachhangV2(info.full_address, info.phone, info.name, idtp, idquan);
                                    _khachhangRepository.Add(newKH);
                                    _unitOfWork.Commit();
                                    khachang = newKH;
                                }
                                long totalAmont = 0;
                                long.TryParse(orderShopee.total_amount, out totalAmont);
                                var newOrder = new donhang();
                                newOrder.UpdatedonhangFromShopee(khachang.MaKH, totalAmont, orderShopee.message_to_seller, orderShopee.shipping_carrier, (int)StatusOrder.Process, jsonContent.Data.Ordersn);
                                _donhangRepository.Add(newOrder);
                                _unitOfWork.Commit();

                                foreach (var item in orderShopee.items)
                                {
                                    var sku = "";
                                    if (!string.IsNullOrEmpty(item.item_sku))
                                        sku = item.item_sku.Trim();
                                    else
                                        sku = item.variation_sku.Trim();
                                    var product = _shopSanPhamRepository.GetSingleByCondition(x => x.masp.Trim() == sku);
                                    long? productId = null;
                                    if (product != null)
                                    {
                                        var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.idsp == product.id);
                                        if (bienthe == null)
                                        {
                                            shop_bienthe newbt = new shop_bienthe();
                                            newbt.idsp = product.id;
                                            newbt.title = "default";
                                            newbt.gia = 100000;
                                            newbt.giasosanh = 0;
                                            newbt.isdelete = false;
                                            bienthe = _shopbientheRepository.Add(newbt);
                                            _unitOfWork.Commit();
                                        }
                                        productId = bienthe.id;
                                    }
                                    int donGia = 0;
                                    int donGiaKM = 0;
                                    if (!string.IsNullOrEmpty(item.variation_discounted_price))
                                    {
                                        int.TryParse(item.variation_discounted_price, out donGia);
                                        int.TryParse(item.variation_original_price, out donGiaKM);
                                    }

                                    else
                                        int.TryParse(item.variation_original_price, out donGia);
                                    var newOrderDetail = new donhang_ct();
                                    string description = "";
                                    description = JsonConvert.SerializeObject(item);
                                    newOrderDetail.UpdatedonhangctFromShopee(newOrder.id, productId, item.variation_quantity_purchased, donGia, donGiaKM, description);
                                    _donhangctRepository.Add(newOrderDetail);
                                    _unitOfWork.Commit();
                                    if (product != null)
                                    {
                                        var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == (int)BranchEnum.KHO_CHINH && x.ProductId == product.id);
                                        if (productInBranch == null)
                                        {
                                            var newStockCurrent = _softStockRepository.Init((int)BranchEnum.KHO_CHINH, product.id);
                                            productInBranch = _softStockRepository.Add(newStockCurrent);
                                            _unitOfWork.Commit();
                                        }
                                        productInBranch.StockTotal -= item.variation_quantity_purchased;
                                        productInBranch.UpdatedDate = DateTime.Now;
                                        _softStockRepository.Update(productInBranch);

                                        shop_sanphamLogs productLog = new shop_sanphamLogs();
                                        productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(product.id) - item.variation_quantity_purchased;
                                        productLog.UpdateShopSanPhamLog(product.id, "Đơn hàng Api Shopee,mã đơn Api Shopee: " + jsonContent.Data.Ordersn + ", mã đơn: " + newOrder.id.ToString(), item.variation_quantity_purchased, 0, (int)BranchEnum.KHO_CHINH, productInBranch.StockTotal.Value);
                                        _shopSanPhamLogRepository.Add(productLog);
                                        _unitOfWork.Commit();
                                    }

                                }
                            }
                        }
                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.COMPLETED)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.Done;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();
                        }

                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.SHIPPED)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.Shipping;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();
                        }

                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.TO_CONFIRM_RECEIVE)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.Shipped;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();
                        }

                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.READY_TO_SHIP)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.ReadyToShip;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();
                        }

                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.CANCELLED)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi.Trim() == jsonContent.Data.Ordersn.Trim());
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.Cancel;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();

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

                                shop_sanphamLogs productLog = new shop_sanphamLogs();
                                productLog.UpdateShopSanPhamLog(bienthe.idsp, "Cập nhật huỷ đơn hàng Api Shopee, mã đơn Api Shopee: " + order.OrderIdShopeeApi + ", mã đơn: " + order.id, item.Soluong, 0, (int)BranchEnum.KHO_CHINH, stockCurrent.StockTotal.Value);
                                productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value) + item.Soluong;
                                _shopSanPhamLogRepository.Add(productLog);

                            }
                            _unitOfWork.Commit();
                        }
                    }
                    else if (jsonContent.Code == 4)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.TrackingNo = jsonContent.Data.TrackingNo;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();
                        }

                    }

                    return request.CreateResponse(HttpStatusCode.OK);
                }
                return response;
            }
            catch (Exception ex)
            {
                var newLog = new shop_sanphamLogs();
                newLog.StockTotal = 0;
                newLog.StockTotalAll = 0;
                newLog.Description = ex.Message + "| " + ex.StackTrace + "|" + JsonConvert.SerializeObject(jsonContent);
                _shopSanPhamLogRepository.Add(newLog);
                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [Route("hooktest")]
        public HttpResponseMessage HookTest(HttpRequestMessage request, string content)
        {
            HttpResponseMessage response = null;

            var jsonContent = JsonConvert.DeserializeObject<ShopeeHookInpVM>(content);
            try
            {
                if (jsonContent != null)
                {
                    if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.UNPAID)
                    {
                        //thêm mới đơn hàng
                        var orderShopee = _shopeeRepository.getOrder(jsonContent.Data.Ordersn);
                        if (orderShopee != null)
                        {
                            var info = orderShopee.recipient_address;
                            var phone = info.phone;
                            if (phone.Substring(0, 2) == "84")
                            {
                                phone = "0" + phone.Substring(2);
                            }

                            var khachang = _khachhangRepository.GetSingleByCondition(x => x.dienthoai == phone);
                            if (khachang == null)
                            {
                                int? idtp = null;
                                int? idquan = null;
                                var tphoName = info.state.ToLower().Replace("tp. hồ chí minh", "TP.HCM");
                                var tpho = _donhangchuyenphattpRepository.GetSingleByCondition(x => x.tentp.Contains(tphoName));
                                if (tpho != null)
                                    idtp = tpho.id;
                                var quanName = info.city.ToLower().Replace("quận ", "").Replace("huyện ", "").Replace("thị xã ", "").Replace("tp. hồ chí minh", "TP.HCM");
                                var quan = _donhangchuyenphattinhRepository.GetSingleByCondition(x => x.tentinh.Contains(quanName));
                                if (quan != null)
                                    idquan = quan.id;

                                var newKH = new khachhang();
                                newKH.UpdatekhachhangV2(info.full_address, info.phone, info.name, idtp, idquan);
                                _khachhangRepository.Add(newKH);
                                _unitOfWork.Commit();
                                khachang = newKH;
                            }
                            long totalAmont = 0;
                            long.TryParse(orderShopee.total_amount, out totalAmont);
                            var newOrder = new donhang();
                            newOrder.UpdatedonhangFromShopee(khachang.MaKH, totalAmont, orderShopee.note, orderShopee.shipping_carrier, (int)StatusOrder.Process, jsonContent.Data.Ordersn);
                            _donhangRepository.Add(newOrder);
                            _unitOfWork.Commit();

                            foreach (var item in orderShopee.items)
                            {
                                var product = _shopSanPhamRepository.GetSingleByCondition(x => x.masp.Trim() == item.item_sku.Trim());
                                long? productId = null;
                                if (product != null)
                                {
                                    var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.idsp == product.id);
                                    if (bienthe == null)
                                    {
                                        shop_bienthe newbt = new shop_bienthe();
                                        newbt.idsp = product.id;
                                        newbt.title = "default";
                                        newbt.gia = 100000;
                                        newbt.giasosanh = 0;
                                        newbt.isdelete = false;
                                        bienthe = _shopbientheRepository.Add(newbt);
                                        _unitOfWork.Commit();
                                    }
                                    productId = bienthe.id;
                                }
                                int donGia = 0;
                                int donGiaKM = 0;
                                if (!string.IsNullOrEmpty(item.variation_discounted_price))
                                {
                                    int.TryParse(item.variation_discounted_price, out donGia);
                                    int.TryParse(item.variation_original_price, out donGiaKM);
                                }

                                else
                                    int.TryParse(item.variation_original_price, out donGia);
                                var newOrderDetail = new donhang_ct();
                                string description = "";
                                description = JsonConvert.SerializeObject(item);
                                newOrderDetail.UpdatedonhangctFromShopee(newOrder.id, productId, item.variation_quantity_purchased, donGia, donGiaKM, description);
                                _donhangctRepository.Add(newOrderDetail);
                                _unitOfWork.Commit();
                                if (product != null)
                                {
                                    var productInBranch = _softStockRepository.GetSingleByCondition(x => x.BranchId == (int)BranchEnum.KHO_CHINH && x.ProductId == product.id);
                                    if (productInBranch == null)
                                    {
                                        var newStockCurrent = _softStockRepository.Init((int)BranchEnum.KHO_CHINH, product.id);
                                        productInBranch = _softStockRepository.Add(newStockCurrent);
                                        _unitOfWork.Commit();
                                    }
                                    productInBranch.StockTotal -= item.variation_quantity_purchased;
                                    productInBranch.UpdatedDate = DateTime.Now;
                                    _softStockRepository.Update(productInBranch);

                                    shop_sanphamLogs productLog = new shop_sanphamLogs();
                                    productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(product.id) - item.variation_quantity_purchased;
                                    productLog.UpdateShopSanPhamLog(product.id, "Đơn hàng Api Shopee,mã đơn Api Shopee: " + jsonContent.Data.Ordersn + ", mã đơn: " + newOrder.id.ToString(), item.variation_quantity_purchased, 0, (int)BranchEnum.KHO_CHINH, productInBranch.StockTotal.Value);
                                    _shopSanPhamLogRepository.Add(productLog);
                                    _unitOfWork.Commit();
                                }

                            }
                        }
                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.COMPLETED)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.Done;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();
                        }

                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.SHIPPED)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == jsonContent.Data.Ordersn);
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.Shipped;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();
                        }

                    }
                    else if (jsonContent.Code == 3 && jsonContent.Data.Status == CommonClass.CANCELLED)
                    {
                        var order = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi.Trim() == jsonContent.Data.Ordersn.Trim());
                        if (order != null)
                        {
                            order.UpdatedBy = 0;
                            order.UpdatedDate = DateTime.Now;
                            order.Status = (int)StatusOrder.Cancel;
                            _donhangRepository.Update(order);
                            _unitOfWork.Commit();

                            foreach (var item in order.donhang_ct)
                            {
                                var bienthe = _shopbientheRepository.GetSingleByCondition(x => x.id == item.IdPro);
                                var stockCurrent = _softStockRepository.GetSingleByCondition(x => x.BranchId == (int)BranchEnum.KHO_CHINH && x.ProductId == bienthe.idsp);
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

                                shop_sanphamLogs productLog = new shop_sanphamLogs();
                                productLog.UpdateShopSanPhamLog(bienthe.idsp, "Cập nhật huỷ đơn hàng Api Shopee, mã đơn Api Shopee: " + order.OrderIdShopeeApi + ", mã đơn: " + order.id, item.Soluong, 0, (int)BranchEnum.KHO_CHINH, stockCurrent.StockTotal.Value);
                                productLog.StockTotalAll = _softStockRepository.GetStockTotalAll(bienthe.idsp.Value) + item.Soluong;
                                _shopSanPhamLogRepository.Add(productLog);

                            }
                            _unitOfWork.Commit();
                        }
                    }
                    return request.CreateResponse(HttpStatusCode.OK);
                }
                return response;
            }
            catch (Exception ex)
            {
                var newLog = new shop_sanphamLogs();
                newLog.StockTotal = 0;
                newLog.StockTotalAll = 0;
                newLog.Description = ex.Message + "| " + ex.StackTrace + "|" + JsonConvert.SerializeObject(jsonContent);
                _shopSanPhamLogRepository.Add(newLog);
                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [Route("getorderdetails")]
        public HttpResponseMessage GetOrderDetails(HttpRequestMessage request, string orderId, string username)
        {
            try
            {
                if (!string.IsNullOrEmpty(orderId))
                {
                    var order = _shopeeRepository.getOrder(orderId);
                    if (order != null)
                    {
                        var logistics = _shopeeRepository.GetOrderLogistics(orderId);
                        if (logistics != null)
                        {
                            var orderDB = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == orderId);
                            var datePrint = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                            orderDB.StatusPrint = "<li>" + username + " đã in (" + datePrint + ")</li>";
                            _donhangRepository.Update(orderDB);
                            _unitOfWork.Commit();

                            double maxWeight = 0;
                            double maxWeightLWH = 0;
                            double totalWeight = 0;
                            foreach (var item in order.items)
                            {
                                maxWeight = item.weight * item.variation_quantity_purchased;
                                var product = _shopeeRepository.GetItemDetail(item.item_id);
                                maxWeightLWH = TotalWeightByLWH(product.package_length, product.package_width, product.package_height) * item.variation_quantity_purchased;

                                if (maxWeight > maxWeightLWH)
                                    totalWeight += maxWeight;
                                else
                                    totalWeight += maxWeightLWH;
                            }

                            return request.CreateResponse(HttpStatusCode.OK, new
                            {
                                BuyerName = order.recipient_address.name,
                                AddressBuyer = order.recipient_address.full_address,
                                PhoneBuyer = order.recipient_address.phone,
                                RecipientSortCode = logistics.logistics.recipient_sort_code.first_recipient_sort_code,
                                SenderSortCode = logistics.logistics.sender_sort_code.first_sender_sort_code,
                                OrderDetails = order.items,
                                TotalAmount = order.cod ? order.total_amount : 0.ToString(),
                                CreateTime = UtilExtensions.UnixTimeStampToDateTime(order.create_time),
                                MaxWeight = Math.Floor(totalWeight * 1000),
                                AddressSeller = _shopeeRepository.GetAddress()
                            });
                        }
                        else
                            return request.CreateResponse(HttpStatusCode.BadRequest, "Chưa xác nhận đơn!");
                    }
                    else
                        return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng không tồn tại trên Shopee!");
                }
                else
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng không hợp lệ");
            }
            catch (Exception ex)
            {
                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "Error (GetOrderDetails to print ): " + JsonConvert.SerializeObject(ex);
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();

                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        //[HttpPost]
        //[Route("getordersdetailstoprint")]
        //[Authorize(Roles = "OrderUpdate")]
        //public HttpResponseMessage GetOrdersDetailsToPrint(HttpRequestMessage request, StatusOrdersViewModel statusOrdersViewModel)
        //{
        //    var result = new List<Object>();
        //    try
        //    {
        //        //foreach (var orderVM in statusOrdersViewModel.Orders)
        //        //{
        //        //    if (string.IsNullOrEmpty(orderVM.OrderIdShopeeApi))
        //        //        return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng " + orderVM.id + " không tồn tại!");
        //        //    if (string.IsNullOrEmpty(orderVM.TrackingNo))
        //        //        return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng " + orderVM.OrderIdShopeeApi + " chưa có mã vận đơn!");
        //        //}

        //        foreach (var orderVM in statusOrdersViewModel.Orders)
        //        {
        //            var orderDb = _donhangRepository.GetSingleById((int)orderVM.id);
        //            if(orderDb != null)
        //            {
        //                if (string.IsNullOrEmpty(orderDb.OrderIdShopeeApi))
        //                    return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng " + orderVM.id + " không tồn tại!");
        //                if (string.IsNullOrEmpty(orderDb.TrackingNo))
        //                    return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng " + orderVM.OrderIdShopeeApi + " chưa có mã vận đơn!");
        //            }

        //            var order = _shopeeRepository.getOrder(orderVM.OrderIdShopeeApi);
        //            if (order != null)
        //            {
        //                var logistics = _shopeeRepository.GetOrderLogistics(orderVM.OrderIdShopeeApi);
        //                if (logistics != null)
        //                {
        //                    double maxWeight = 0;
        //                    var totalQuantity = 0;
        //                    foreach (var item in order.items)
        //                    {
        //                        maxWeight += item.weight * item.variation_quantity_purchased;
        //                        totalQuantity += item.variation_quantity_purchased;
        //                    }

        //                    var orderDB = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == orderVM.OrderIdShopeeApi);
        //                    var datePrint = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        //                    orderDB.StatusPrint = "<li>" + statusOrdersViewModel.UserName + " đã in (" + datePrint + ")</li>";
        //                    _donhangRepository.Update(orderDB);

        //                    result.Add(new
        //                    {
        //                        TrackingNo = orderDB.TrackingNo,
        //                        OrderIdShopeeApi = orderDB.OrderIdShopeeApi,
        //                        ghichu = orderDB.ghichu,
        //                        MaxWeight = Math.Floor(maxWeight * 1000),
        //                        TotalQuantity = totalQuantity,
        //                        ShipperName = orderDB.ShipperNameShopeeApi,
        //                        BuyerName = order.recipient_address.name,
        //                        AddressBuyer = order.recipient_address.full_address,
        //                        PhoneBuyer = order.recipient_address.phone,
        //                        RecipientSortCode = logistics.logistics.recipient_sort_code.first_recipient_sort_code,
        //                        SenderSortCode = logistics.logistics.sender_sort_code.first_sender_sort_code,
        //                        OrderDetails = order.items,
        //                        TotalAmount = order.cod ? order.total_amount : 0.ToString(),
        //                        CreateTime = UtilExtensions.UnixTimeStampToDateTime(order.create_time)
        //                    });
        //                }
        //            }
        //        }
        //        _unitOfWork.Commit();
        //        return request.CreateResponse(HttpStatusCode.OK, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var contentLog = new SoftPointUpdateLog();
        //        contentLog.Description = "Error (GetOrdersDetailsToPrint to print all ): " + JsonConvert.SerializeObject(ex);
        //        contentLog.CreatedDate = DateTime.Now;
        //        _softPointUpdateLogRepository.Add(contentLog);
        //        _unitOfWork.Commit();
        //        return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("getordersdetailstoprint")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage GetOrdersDetailsToPrint(HttpRequestMessage request, string inputParams)
        {
            var result = new List<Object>();
            try
            {
                var addressSeller = _shopeeRepository.GetAddress();
                var statusOrdersViewModel = JsonConvert.DeserializeObject<StatusOrdersViewModelV2>(inputParams);

                foreach (var orderVM in statusOrdersViewModel.Orders)
                {
                    var orderDb = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == orderVM);
                    if (orderDb == null)
                        return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng " + orderVM + " không tồn tại!");
                    if (string.IsNullOrEmpty(orderDb.TrackingNo))
                        return request.CreateResponse(HttpStatusCode.BadRequest, "Đơn hàng " + orderVM + " chưa có mã vận đơn!");

                    var order = _shopeeRepository.getOrder(orderVM);
                    if (order != null)
                    {
                        var logistics = _shopeeRepository.GetOrderLogistics(orderVM);
                        if (logistics != null)
                        {
                            double maxWeight = 0;
                            double maxWeightLWH = 0;
                            double totalWeight = 0;
                            var totalQuantity = 0;
                            foreach (var item in order.items)
                            {
                                maxWeight = item.weight * item.variation_quantity_purchased;
                                var product = _shopeeRepository.GetItemDetail(item.item_id);
                                maxWeightLWH = TotalWeightByLWH(product.package_length, product.package_width, product.package_height) * item.variation_quantity_purchased;
                                totalQuantity += item.variation_quantity_purchased;

                                if (maxWeight > maxWeightLWH)
                                    totalWeight += maxWeight;
                                else
                                    totalWeight += maxWeightLWH;
                            }

                            var orderDB = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == orderVM);
                            var datePrint = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                            orderDB.StatusPrint = "<li>" + statusOrdersViewModel.UserName + " đã in (" + datePrint + ")</li>";
                            _donhangRepository.Update(orderDB);

                            result.Add(new
                            {
                                TrackingNo = orderDB.TrackingNo,
                                OrderIdShopeeApi = orderDB.OrderIdShopeeApi,
                                ghichu = orderDB.ghichu,
                                MaxWeight = Math.Floor(totalWeight * 1000),
                                TotalQuantity = totalQuantity,
                                ShipperName = orderDB.ShipperNameShopeeApi,
                                BuyerName = order.recipient_address.name,
                                AddressBuyer = order.recipient_address.full_address,
                                PhoneBuyer = order.recipient_address.phone,
                                RecipientSortCode = logistics.logistics.recipient_sort_code.first_recipient_sort_code,
                                SenderSortCode = logistics.logistics.sender_sort_code.first_sender_sort_code,
                                OrderDetails = order.items,
                                TotalAmount = order.cod ? order.total_amount : 0.ToString(),
                                CreateTime = UtilExtensions.UnixTimeStampToDateTime(order.create_time),
                                AddressSeller = addressSeller,
                            });
                        }
                    }
                }
                _unitOfWork.Commit();
                return request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "Error (GetOrdersDetailsToPrint to print all ): " + JsonConvert.SerializeObject(ex);
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();
                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [Route("getlackorders")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage GetLackOrders(HttpRequestMessage request)
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
                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var contentLog = new SoftPointUpdateLog();
                contentLog.Description = "Error (GetLackOrders): " + JsonConvert.SerializeObject(ex);
                contentLog.CreatedDate = DateTime.Now;
                _softPointUpdateLogRepository.Add(contentLog);
                _unitOfWork.Commit();
                return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("testapi")]
        [Authorize(Roles = "OrderUpdate")]
        public HttpResponseMessage TestAPI(HttpRequestMessage request, string orderParam, string productParam)
        {
            var result = new List<Object>();
            try
            {
                //var orderShopee = new ShopeeOrder();
                //if (!string.IsNullOrEmpty(orderParam))
                //    orderShopee = _shopeeRepository.getOrder(orderParam);
                //var productShopee = new ShopeeItem();
                //if (!string.IsNullOrEmpty(productParam))
                //    productShopee = _shopeeRepository.GetItemDetail(long.Parse(productParam));

                var test = _shopeeRepository.GetAddress();

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

        //[HttpGet]
        //[Route("updatestockofproductsnoskuinorders")]
        //[Authorize(Roles = "OrderUpdate")]
        //public HttpResponseMessage UpdateStockOfProductsNoSkuInOrders(HttpRequestMessage request)
        //{
        //    var result = new List<Object>();
        //    try
        //    {
        //        var orderShopee = _shopeeRepository.GetItemDetail(314091888);
        //        return request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        //var contentLog = new SoftPointUpdateLog();
        //        //contentLog.Description = "Error (GetLackOrders): " + JsonConvert.SerializeObject(ex);
        //        //contentLog.CreatedDate = DateTime.Now;
        //        //_softPointUpdateLogRepository.Add(contentLog);
        //        //_unitOfWork.Commit();
        //        return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        public float TotalWeightByLWH(float length = 0, float width = 0, float height = 0, bool fast = true)
        {
            float res = 0;
            if (fast)
                res = (length * width * height) / 6000;
            else
                res = (length * width * height) / 4000;
            return res;
        }
    }
}