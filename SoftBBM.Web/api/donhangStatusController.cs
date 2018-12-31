using AutoMapper;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
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
    [RoutePrefix("api/orderstatus")]
    public class donhangStatusController : ApiController
    {
        IdonhangStatusRepository _donhangStatusRepository;
        IUnitOfWork _unitOfWork;

        public donhangStatusController(IUnitOfWork unitOfWork, IdonhangStatusRepository donhangStatusRepository)
        {
            this._donhangStatusRepository = donhangStatusRepository;
            _unitOfWork = unitOfWork;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            var donhangStatuses = _donhangStatusRepository.GetAll();
            var responseData = Mapper.Map<IEnumerable<donhangStatu>, IEnumerable<donhangStatusViewModel>>(donhangStatuses);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);
            return response;
        }
    }
}