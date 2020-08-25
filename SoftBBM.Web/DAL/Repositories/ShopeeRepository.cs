using Newtonsoft.Json;
using SoftBBM.Web.Common;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Enum;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IShopeeRepository : IRepository<donhang>
    {
        string update_stock(int item_id, int stock, int partner_id, int shopid);
        string createSignature(string key, string data);
        string createSignatureNoPass(string data);
        ShopeeOrder getOrder(string ordersn);
        string getTimeSlot(string ordersn);
        string confirmOrder(string ordersn, string pickup_time_id);
        void confirmOrderNoDelay(string ordersn, string pickup_time_id);
        ShopeeOrderLogistics GetOrderLogistics(string ordersn);
        ShopeeGetOrdersList GetOrdersListLastDay();
        List<donhangIdShopeeId> GetOrdersListLastDayDB();
        void AddOrderLack(string ordersn, string statusOrder, DateTime? updatedDate);

    }
    public class ShopeeRepository : RepositoryBase<donhang>, IShopeeRepository
    {
        IUnitOfWork _unitOfWork;
        ISoftChannelRepository _softChannelRepository;
        ISoftPointUpdateLogRepository _softPointUpdateLogRepository;
        IkhachhangRepository _khachhangRepository;
        IdonhangchuyenphattpRepository _donhangchuyenphattpRepository;
        IdonhangchuyenphattinhRepository _donhangchuyenphattinhRepository;
        IdonhangRepository _donhangRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        IshopbientheRepository _shopbientheRepository;
        IdonhangctRepository _donhangctRepository;
        ISoftStockRepository _softStockRepository;
        IShopSanPhamLogRepository _shopSanPhamLogRepository;

        int _apiId;
        public string _apiPassowrd;
        int _apiPartnerId;
        bool _validApi;
        long _addressId = 0;


        public ShopeeRepository(IUnitOfWork unitOfWork, ISoftChannelRepository softChannelRepository, ISoftPointUpdateLogRepository softPointUpdateLogRepository, IDbFactory dbFactory, IkhachhangRepository khachhangRepository, IdonhangchuyenphattpRepository donhangchuyenphattpRepository, IdonhangchuyenphattinhRepository donhangchuyenphattinhRepository, IdonhangRepository donhangRepository, IShopSanPhamRepository shopSanPhamRepository, IshopbientheRepository shopbientheRepository, IdonhangctRepository donhangctRepository, ISoftStockRepository softStockRepository, IShopSanPhamLogRepository shopSanPhamLogRepository) : base(dbFactory)
        {
            _softChannelRepository = softChannelRepository;
            _softPointUpdateLogRepository = softPointUpdateLogRepository;
            _khachhangRepository = khachhangRepository;
            _donhangchuyenphattpRepository = donhangchuyenphattpRepository;
            _donhangchuyenphattinhRepository = donhangchuyenphattinhRepository;
            _donhangRepository = donhangRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _shopbientheRepository = shopbientheRepository;
            _donhangctRepository = donhangctRepository;
            _softStockRepository = softStockRepository;
            _shopSanPhamLogRepository = shopSanPhamLogRepository;

            _unitOfWork = unitOfWork;

            _apiId = 1928354;
            _apiPassowrd = "64e256a952a7d79e9c0d09cd2075b8249b6cbba27586a4238e2733af28e26266";
            _apiPartnerId = 842214;
            long.TryParse(ConfigurationSettings.AppSettings.Get("shopeeAddressId").ToString(), out _addressId);

            //_validApi = false;
            //var channel = _softChannelRepository.GetSingleById((int)ChannelEnum.SPE);
            //var apiPartnerId = 0;
            //var apiId = 0;
            //int.TryParse(channel.ApiId, out apiId);
            //int.TryParse(channel.ApiPartnerId, out apiPartnerId);

            //var result = authenShopee(apiId, channel.ApiPassword, apiPartnerId);
            //if(!string.IsNullOrEmpty(result))
            //{
            //    var resultJson = JsonConvert.DeserializeObject<ShopInfo>(result);
            //    if (resultJson.status == "NORMAL" && channel.Enabled == true)
            //    {
            //        _apiId = apiId;
            //        _apiPassowrd = channel.ApiPassword;
            //        _apiPartnerId = apiPartnerId;
            //        _validApi = true;
            //    }
            //}
        }

        public string authenShopee(int apiId, string apiPassword, int apiPartnerId)
        {
            var jsonStrResult = "";
            //var channel = _softChannelRepository.GetSingleById((int)ChannelEnum.SPE);
            var timeStamp = getTimestamp();
            string url = "https://partner.shopeemobile.com/api/v1/shop/get";
            string dataJson = "{'partner_id':" + apiPartnerId + "," +
                              "'shopid':" + apiId + "," +
                              "'timestamp':" + timeStamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string signatureBaseString = url + '|' + dataJson;
            string signatureAuth = createSignature(apiPassword, signatureBaseString);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", signatureAuth);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(dataJson);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.Default, true))
            {
                jsonStrResult = streamReader.ReadToEnd();
            }

            return jsonStrResult;
        }

        public int getTimestamp()
        {
            var unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public int getTimestampLastDay()
        {
            var unixTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        //key=password, data = dataJson
        public string createSignature(string key, string data)
        {
            var result = "";
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(data);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return result;
        }

        public string postRequest(string url, string dataJson)
        {
            var jsonStrResult = "";
            var timeStamp = getTimestamp();
            string signatureBaseString = url + '|' + dataJson;
            _apiPassowrd = "64e256a952a7d79e9c0d09cd2075b8249b6cbba27586a4238e2733af28e26266";

            string signatureAuth = createSignature(_apiPassowrd, signatureBaseString);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", signatureAuth);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(dataJson);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.Default, true))
            {
                jsonStrResult = streamReader.ReadToEnd();
            }

            return jsonStrResult;
        }

        public string update_stock(int item_id, int stock, int partner_id, int shopid)
        {
            var timestamp = getTimestamp();
            var result = "";
            var url = "https://partner.shopeemobile.com/api/v1/items/update_stock";
            string dataJson = "{'item_id':" + item_id + "," +
                              "'stock':" + stock + "," +
                              "'partner_id':" + partner_id + "," +
                              "'shopid':" + shopid + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            result = postRequest(url, dataJson);
            return result;
        }

        public string createSignatureNoPass(string data)
        {
            var result = "";
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(data);
            Byte[] keyBytes = encoding.GetBytes(_apiPassowrd);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return result;
        }

        public ShopeeOrder getOrder(string ordersn)
        {
            var timestamp = getTimestamp();
            var result = "";
            var url = "https://partner.shopeemobile.com/api/v1/orders/detail";
            string dataJson = "{'ordersn_list':['" + ordersn + "']," +
                              "'partner_id':" + _apiPartnerId + "," +
                              "'shopid':" + _apiId + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string data = postRequest(url, dataJson);

            if (!string.IsNullOrEmpty(data))
            {
                var getOrderDetailsRes = JsonConvert.DeserializeObject<GetOrderDetailsRes>(data);
                if (getOrderDetailsRes.orders.Count > 0)
                {
                    return getOrderDetailsRes.orders.FirstOrDefault();
                }
            }
            return null;
        }

        public string getTimeSlot(string ordersn)
        {
            var timestamp = getTimestamp();
            var result = "";
            var url = "https://partner.shopeemobile.com/api/v1/logistics/timeslot/get";
            string dataJson = "{'ordersn':'" + ordersn + "'," +
                              "'address_id':" + _addressId + "," +
                              "'partner_id':" + _apiPartnerId + "," +
                              "'shopid':" + _apiId + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string data = postRequest(url, dataJson);

            if (!string.IsNullOrEmpty(data))
            {
                var timeSlot = JsonConvert.DeserializeObject<ShopeeTimeSlot>(data);
                if (timeSlot.pickup_time.Count > 0)
                {
                    return timeSlot.pickup_time.FirstOrDefault().date.ToString();
                }
            }
            return null;
        }

        public string confirmOrder(string ordersn, string pickup_time_id)
        {
            var timestamp = getTimestamp();
            var result = "";
            var url = "https://partner.shopeemobile.com/api/v1/logistics/init";
            string dataJson = "{'ordersn':'" + ordersn + "'," +
                              "'pickup':{'address_id': " + _addressId + "," +
                                        "'pickup_time_id': '" + pickup_time_id + "'" + "}," +
                              "'partner_id':" + _apiPartnerId + "," +
                              "'shopid':" + _apiId + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string data = postRequest(url, dataJson);

            if (!string.IsNullOrEmpty(data))
            {
                var init = JsonConvert.DeserializeObject<ShopeeInit>(data);

                //var dataConfirm = new SoftPointUpdateLog();
                //dataConfirm.Description = "data "+ ordersn + " confirmOrder: " + data;
                //dataConfirm.CreatedDate = DateTime.Now;
                //_softPointUpdateLogRepository.Add(dataConfirm);
                //_unitOfWork.Commit();


                if (!string.IsNullOrEmpty(init.tracking_number))
                {
                    return init.tracking_number;
                }
                else
                {
                    //Shopee delay
                    Thread.Sleep(2000);
                    var orderDetail = this.getOrder(ordersn);

                    //var dataOrdersn = new SoftPointUpdateLog();
                    //dataOrdersn.Description = "data " + ordersn + ": " + JsonConvert.SerializeObject(orderDetail);
                    //dataOrdersn.CreatedDate = DateTime.Now;
                    //_softPointUpdateLogRepository.Add(dataOrdersn);
                    //_unitOfWork.Commit();

                    if (orderDetail != null)
                        if (!string.IsNullOrEmpty(orderDetail.tracking_no))
                        {
                            return orderDetail.tracking_no;
                        }
                }
            }
            return null;
        }
        public void confirmOrderNoDelay(string ordersn, string pickup_time_id)
        {
            var timestamp = getTimestamp();
            var url = "https://partner.shopeemobile.com/api/v1/logistics/init";
            string dataJson = "{'ordersn':'" + ordersn + "'," +
                              "'pickup':{'address_id': " + _addressId + "," +
                                        "'pickup_time_id': '" + pickup_time_id + "'" + "}," +
                              "'partner_id':" + _apiPartnerId + "," +
                              "'shopid':" + _apiId + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string data = postRequest(url, dataJson);
        }

        public ShopeeOrderLogistics GetOrderLogistics(string ordersn)
        {
            var timestamp = getTimestamp();
            var result = "";
            var url = "https://partner.shopeemobile.com/api/v1/logistics/order/get";
            string dataJson = "{'ordersn':'" + ordersn + "'," +
                              "'partner_id':" + _apiPartnerId + "," +
                              "'shopid':" + _apiId + "," +
                              "'forder_id':''" + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string data = postRequest(url, dataJson);

            if (!string.IsNullOrEmpty(data))
            {
                var orderLogistics = JsonConvert.DeserializeObject<ShopeeOrderLogistics>(data);
                if (orderLogistics != null)
                {
                    return orderLogistics;
                }
            }
            return null;
        }

        public ShopeeGetOrdersList GetOrdersListLastDay()
        {
            var timestamp = getTimestamp();
            var fromDate = getTimestampLastDay();
            var result = new ShopeeGetOrdersList();
            var url = "https://partner.shopeemobile.com/api/v1/orders/basics";
            string dataJson = "{'create_time_from':" + fromDate + "," +
                              "'create_time_to':" + timestamp + "," +
                              "'partner_id':" + _apiPartnerId + "," +
                              "'shopid':" + _apiId + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string data = postRequest(url, dataJson);

            if (!string.IsNullOrEmpty(data))
            {
                var ordersList = JsonConvert.DeserializeObject<ShopeeGetOrdersList>(data);
                if (ordersList == null)
                    return result;
                if (ordersList.orders.Count > 0)
                {
                    result = ordersList;
                }
            }
            return result;
        }

        public List<donhangIdShopeeId> GetOrdersListLastDayDB()
        {
            var lastDay = DateTime.Now.AddDays(-2);
            return DbContext.donhangs.Where(x => x.CreatedDate >= lastDay && x.CreatedDate <= DateTime.Now && x.ChannelId == 4).Select(x => new donhangIdShopeeId()
            {
                id = x.id,
                OrderIdShopeeApi = x.OrderIdShopeeApi
            }).ToList();
        }

        public void AddOrderLack(string ordersn, string statusOrder, DateTime? updatedDate)
        {
            //thêm mới đơn hàng
            var orderShopee = this.getOrder(ordersn);
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
                int statusOrderConvert = 0;
                switch (statusOrder)
                {
                    case CommonClass.COMPLETED:
                        statusOrderConvert = (int)StatusOrder.Done;
                        break;
                    case CommonClass.SHIPPED:
                        statusOrderConvert = (int)StatusOrder.Shipping;
                        break;
                    case CommonClass.TO_CONFIRM_RECEIVE:
                        statusOrderConvert = (int)StatusOrder.Shipped;
                        break;
                    case CommonClass.READY_TO_SHIP:
                        statusOrderConvert = (int)StatusOrder.ReadyToShip;
                        break;
                    case CommonClass.CANCELLED:
                        statusOrderConvert = (int)StatusOrder.Cancel;
                        break;
                    default:
                        statusOrderConvert = (int)StatusOrder.Process;
                        break;
                }

                var createdDate = UtilExtensions.UnixTimeStampToDateTime(orderShopee.create_time);

                newOrder.UpdatedonhangLackFromShopee(khachang.MaKH, totalAmont, orderShopee.message_to_seller, orderShopee.shipping_carrier, statusOrderConvert, ordersn, createdDate, updatedDate);
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
                        productLog.UpdateShopSanPhamLog(product.id, "Đơn hàng Api Shopee,mã đơn Api Shopee: " + ordersn + ", mã đơn: " + newOrder.id.ToString(), item.variation_quantity_purchased, 0, (int)BranchEnum.KHO_CHINH, productInBranch.StockTotal.Value);
                        _shopSanPhamLogRepository.Add(productLog);
                        _unitOfWork.Commit();
                    }

                }
            }
        }
    }
}