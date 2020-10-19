using AutoMapper;
using Newtonsoft.Json;
using SoftBBM.Web.Common;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Enum;
using SoftBBM.Web.Infrastructure.Core;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/channel")]
    public class SoftChannelController : ApiController
    {
        ISoftChannelRepository _softChannelRepository;
        IUnitOfWork _unitOfWork;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;

        public SoftChannelController(ISoftChannelRepository softChannelRepository, IUnitOfWork unitOfWork, IShopSanPhamRepository shopSanPhamRepository, ISoftChannelProductPriceRepository softChannelProductPriceRepository)
        {
            _softChannelRepository = softChannelRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "ChannelList")]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftChannel> channels = null;
            int totalChannels = new int();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.Trim().ToLower();

                channels = _softChannelRepository.GetMulti(c => c.Name.ToLower().Contains(filter))
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalChannels = _softChannelRepository.GetMulti(c => c.Name.ToLower().Contains(filter))
                    .Count();
            }
            else
            {
                channels = _softChannelRepository.GetAll()
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                .ToList();

                totalChannels = _softChannelRepository.GetAll().Count();
            }

            IEnumerable<SoftChannelViewModel> channelsVM = Mapper.Map<IEnumerable<SoftChannel>, IEnumerable<SoftChannelViewModel>>(channels);

            PaginationSet<SoftChannelViewModel> pagedSet = new PaginationSet<SoftChannelViewModel>()
            {
                Page = currentPage,
                TotalCount = totalChannels,
                TotalPages = (int)Math.Ceiling((decimal)totalChannels / currentPageSize),
                Items = channelsVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("add")]
        [Authorize(Roles = "ChannelAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, SoftChannelViewModel SoftChannelViewModel)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray();
                string outputError = "";
                foreach (var item in errors)
                {
                    outputError += item + " | ";
                }
                response = request.CreateResponse(HttpStatusCode.BadRequest, outputError);
            }
            else
            {
                var newSoftChannel = new SoftChannel();
                newSoftChannel.UpdateSoftChannel(SoftChannelViewModel);
                newSoftChannel.Status = true;

                _softChannelRepository.Add(newSoftChannel);
                _unitOfWork.Commit();

                //migration channelprice and product
                //var products = _shopSanPhamRepository.GetAllIds();
                //var channels = _softChannelRepository.GetAllIds();
                //foreach (var product in products.ToList())
                //{
                //    var newPrice = new SoftChannelProductPrice();
                //    newPrice.ProductId = product;
                //    newPrice.ChannelId = newSoftChannel.Id;
                //    newPrice.Price = 0;
                //    newPrice.CreatedDate = DateTime.Now;
                //    _softChannelProductPriceRepository.Add(newPrice);   
                //}
                _unitOfWork.Commit();

                var responseData = Mapper.Map<SoftChannel, SoftChannelViewModel>(newSoftChannel);
                response = request.CreateResponse(HttpStatusCode.Created, responseData);
            }
            return response;
        }

        [Route("update")]
        [Authorize(Roles = "ChannelEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, SoftChannelViewModel softChannelViewModel)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray();
                string outputError = "";
                foreach (var item in errors)
                {
                    outputError += item + " | ";
                }
                response = request.CreateResponse(HttpStatusCode.BadRequest, outputError);
            }
            else
            {
                var oldChannel = _softChannelRepository.GetSingleById(softChannelViewModel.Id);
                oldChannel.UpdateSoftChannel(softChannelViewModel);
                _softChannelRepository.Update(oldChannel);
                switch (softChannelViewModel.Id)
                {
                    case (int)ChannelEnum.SPE:
                        var result = "";
                        if (!string.IsNullOrEmpty(softChannelViewModel.ApiId) && !string.IsNullOrEmpty(softChannelViewModel.ApiPartnerId) && !string.IsNullOrEmpty(softChannelViewModel.ApiPassword))
                        {
                            var apiPartnerId = 0;
                            var apiId = 0;
                            int.TryParse(softChannelViewModel.ApiId, out apiId);
                            int.TryParse(softChannelViewModel.ApiPartnerId, out apiPartnerId);
                            result = authenShopee(apiId, softChannelViewModel.ApiPassword, apiPartnerId);
                            var resultJson = JsonConvert.DeserializeObject<ShopInfo>(result);
                            if (resultJson.status == "NORMAL")
                            {
                                oldChannel.ApiId = softChannelViewModel.ApiId;
                                oldChannel.ApiPassword = softChannelViewModel.ApiPassword;
                                oldChannel.ApiPartnerId = softChannelViewModel.ApiPartnerId;
                                response = request.CreateResponse(HttpStatusCode.OK, "Cập nhật và kết nối API thành công!");
                            }
                            else
                            {
                                response = request.CreateResponse(HttpStatusCode.BadRequest, "Kết nối API thất bại");
                            }
                        }
                        else
                            response = request.CreateResponse(HttpStatusCode.OK, "Cập nhật thành công!");
                        break;
                    default:
                        response = request.CreateResponse(HttpStatusCode.OK, "Cập nhật thành công!");
                        break;
                }
                _unitOfWork.Commit();                
            }
            return response;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;

            var channels = _softChannelRepository.GetAll();
            var responseData = Mapper.Map<IEnumerable<SoftChannel>, IEnumerable<SoftChannelViewModel>>(channels);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        [Route("authenadd")]
        [Authorize(Roles = "ChannelAdd")]
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

        [Route("authenedit")]
        [Authorize(Roles = "ChannelEdit")]
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

        [Route("detail/{id}")]
        [HttpGet]
        public HttpResponseMessage Detail(HttpRequestMessage request, int id)
        {
            HttpResponseMessage response = null;
            var channel = _softChannelRepository.GetSingleById(id);
            var responseData = Mapper.Map<SoftChannel, SoftChannelViewModel>(channel);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);
            return response;
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

        //key=password, data = dataJson
        public string createSignature(string key, string data)
        {
            var result = "";

            //ASCIIEncoding encoding = new ASCIIEncoding();
            //byte[] keyByte = encoding.GetBytes(key);
            //HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
            //byte[] messageBytes = encoding.GetBytes(data);
            //byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            //result = Encoding.ASCII.GetString(hashmessage);

            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(data);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return result;
        }

    }
}