using AutoMapper;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
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
    [RoutePrefix("api/channelproductprice")]
    public class SoftChannelProductPriceController : ApiController
    {
        IUnitOfWork _unitOfWork;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        ISoftChannelRepository _softChannelRepository;
        IShopSanPhamRepository _shopSanPhamRepository;

        public SoftChannelProductPriceController(IUnitOfWork unitOfWork, ISoftChannelProductPriceRepository softChannelProductPriceRepository, IShopSanPhamRepository shopSanPhamRepository, ISoftChannelRepository softChannelRepository)
        {
            _unitOfWork = unitOfWork;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _softChannelRepository = softChannelRepository;
        }

        [HttpGet]
        [Route("getallbyproductid")]
        [Authorize(Roles = "ProductChannelPrice")]
        public HttpResponseMessage GetAllByProductId(HttpRequestMessage request, int productId)
        {
            HttpResponseMessage respone = null;
            try
            {
                var channelProductPrices = _softChannelProductPriceRepository.GetMulti(x => x.ProductId == productId);
                var channelProductPricesVM = Mapper.Map<IEnumerable<SoftChannelProductPrice>, IEnumerable<SoftChannelProductPriceViewModel>>(channelProductPrices);
                respone = request.CreateResponse(HttpStatusCode.OK, channelProductPricesVM);
                return respone;
            }
            catch (Exception ex)
            {
                respone = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return respone;
            }

        }

        [HttpPost]
        [Route("updatechannelprice")]
        [Authorize(Roles = "ProductChannelPriceUpdate")]
        public HttpResponseMessage UpdateChannelPrice(HttpRequestMessage request, GetAllPriceChannelProductOutputViewModel SoftChannelProductPriceVm)
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
                foreach (var item in SoftChannelProductPriceVm.PriceChannels)
                {
                    if (item.StartDateDiscount != null)
                        item.StartDateDiscount = UtilExtensions.ConvertStartDate(item.StartDateDiscount.Value.ToLocalTime());
                    if (item.EndDateDiscount != null)
                        item.EndDateDiscount = UtilExtensions.ConvertEndDate(item.EndDateDiscount.Value.ToLocalTime());
                    var model = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == SoftChannelProductPriceVm.ProductId && x.ChannelId == item.ChannelId);
                    if (model == null)
                    {
                        if (item.Price > 0 || item.PriceDiscount > 0)
                        {
                            var nwPrice = new SoftChannelProductPrice();
                            nwPrice.ProductId = SoftChannelProductPriceVm.ProductId;
                            nwPrice.ChannelId = item.ChannelId;
                            nwPrice.Price = item.Price > 0 ? item.Price : 0;
                            nwPrice.PriceDiscount = item.PriceDiscount > 0 ? item.PriceDiscount : 0;
                            nwPrice.StartDateDiscount = item.StartDateDiscount;
                            nwPrice.EndDateDiscount = item.EndDateDiscount;
                            nwPrice.Description = item.Description;
                            nwPrice.CreatedBy = SoftChannelProductPriceVm.UpdateBy;
                            nwPrice.CreatedDate = DateTime.Now;
                            _softChannelProductPriceRepository.Add(nwPrice);
                            _unitOfWork.Commit();
                        }
                    }
                    else
                    {
                        model.ProductId = SoftChannelProductPriceVm.ProductId;
                        model.ChannelId = item.ChannelId;
                        model.Price = item.Price > 0 ? item.Price : 0;
                        model.PriceDiscount = item.PriceDiscount > 0 ? item.PriceDiscount : 0;
                        model.StartDateDiscount = item.StartDateDiscount;
                        model.EndDateDiscount = item.EndDateDiscount;
                        model.Description = item.Description;
                        model.UpdatedBy = SoftChannelProductPriceVm.UpdateBy;
                        model.UpdatedDate = DateTime.Now;
                        _softChannelProductPriceRepository.Update(model);
                    }
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK);
            }
            return response;
        }

        [HttpGet]
        [Route("getallpricechannelproduct")]
        [Authorize(Roles = "ProductChannelPrice")]
        public HttpResponseMessage GetAllPriceChannelProduct(HttpRequestMessage request, int productId)
        {
            HttpResponseMessage respone = null;
            try
            {
                var result = new GetAllPriceChannelProductOutputViewModel();
                var product = _shopSanPhamRepository.GetSingleById(productId);
                result.ProductId = productId;
                result.PriceBase = product.PriceBase == null ? 0 : product.PriceBase.Value;
                result.PriceBaseOld = product.PriceBaseOld == null ? 0 : product.PriceBaseOld.Value;
                result.PriceRef = product.PriceRef == null ? 0 : product.PriceRef.Value;
                List<SoftChannelProductPrice> channelProductPrices = new List<SoftChannelProductPrice>();
                channelProductPrices = _softChannelProductPriceRepository.GetMulti(x => x.ProductId == productId).OrderBy(x => x.ChannelId).ToList();
                var channelProductPricesVm = Mapper.Map<List<SoftChannelProductPrice>, List<PriceChannelViewModel>>(channelProductPrices);
                var channels = _softChannelRepository.GetMulti(x => x.Id != 7);
                foreach (var item in channels)
                {
                    var exist = channelProductPrices.SingleOrDefault(x => x.ChannelId == item.Id);
                    if (exist == null)
                    {
                        var nwPrice = new PriceChannelViewModel();
                        nwPrice.ChannelId = item.Id;
                        nwPrice.Code = item.Code;
                        nwPrice.Name = item.Name;
                        nwPrice.Price = 0;
                        nwPrice.PriceDiscount = 0;
                        channelProductPricesVm.Add(nwPrice);
                    }
                }
                channelProductPricesVm = channelProductPricesVm.OrderBy(x => x.ChannelId).ToList();
                result.PriceChannels = channelProductPricesVm;
                foreach (var item in result.PriceChannels)
                {
                    if (item.Price == null)
                        item.Price = 0;
                    if (item.PriceDiscount == null)
                        item.PriceDiscount = 0;
                }
                respone = request.CreateResponse(HttpStatusCode.OK, result);
                return respone;
            }
            catch (Exception ex)
            {
                respone = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return respone;
            }

        }
    }
}