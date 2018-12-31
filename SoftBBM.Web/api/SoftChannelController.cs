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
                _unitOfWork.Commit();
                var responseData = Mapper.Map<SoftChannel, SoftChannelViewModel>(oldChannel);
                response = request.CreateResponse(HttpStatusCode.Created, responseData);
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
    }
}