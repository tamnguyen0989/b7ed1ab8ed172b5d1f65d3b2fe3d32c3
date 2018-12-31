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

    [RoutePrefix("api/district")]
    public class donhangchuyenphattinhController : ApiController
    {
        IdonhangchuyenphattinhRepository _donhangchuyenphattinhRepository;
        IUnitOfWork _unitOfWork;
        public donhangchuyenphattinhController(IUnitOfWork unitOfWork, IdonhangchuyenphattinhRepository donhangchuyenphattinhRepository)
        {
            _unitOfWork = unitOfWork;
            _donhangchuyenphattinhRepository = donhangchuyenphattinhRepository;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;

            var districts = _donhangchuyenphattinhRepository.GetAll().OrderBy(x=>x.Priority).ThenBy(x=>x.tentinh);
            var responseData = Mapper.Map<IEnumerable<donhang_chuyenphat_tinh>, IEnumerable<donhangchuyenphattinhViewModel>>(districts);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        
    }
}