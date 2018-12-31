using AutoMapper;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/city")]
    public class donhangchuyenphattpController : ApiController
    {
        IdonhangchuyenphattpRepository _donhangchuyenphattpRepository;
        IUnitOfWork _unitOfWork;
        public donhangchuyenphattpController(IUnitOfWork unitOfWork, IdonhangchuyenphattpRepository donhangchuyenphattpRepository)
        {
            _unitOfWork = unitOfWork;
            _donhangchuyenphattpRepository = donhangchuyenphattpRepository;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;

            var cities = _donhangchuyenphattpRepository.GetAll().OrderBy(x=>x.uutien);
            var responseData = Mapper.Map<IEnumerable<donhang_chuyenphat_tp>, IEnumerable<donhangchuyenphattpViewModel>>(cities);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

    }
}