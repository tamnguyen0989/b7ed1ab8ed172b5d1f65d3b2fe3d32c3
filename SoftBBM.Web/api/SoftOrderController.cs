using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoftBBM.Web.api
{

    public class SoftOrderController : ApiController
    {
        ISoftOrderRepository _softOrderRepository;
        IUnitOfWork _unitOfWork;

        public SoftOrderController(ISoftOrderRepository softOrderRepository, IUnitOfWork unitOfWork)
        {
            _softOrderRepository = softOrderRepository;
            _unitOfWork = unitOfWork;
        }



    }
}