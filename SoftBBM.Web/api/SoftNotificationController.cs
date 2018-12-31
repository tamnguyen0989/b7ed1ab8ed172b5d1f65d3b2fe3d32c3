using AutoMapper;
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
    [RoutePrefix("api/notification")]
    public class SoftNotificationController : ApiController
    {
        ISoftNotificationRepository _softNotificationRepository;
        IUnitOfWork _unitOfWork;

        public SoftNotificationController(IUnitOfWork unitOfWork, ISoftNotificationRepository softNotificationRepository)
        {
            _unitOfWork = unitOfWork;
            _softNotificationRepository = softNotificationRepository;
        }

        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search(HttpRequestMessage request, int page, int pageSize, int branchId)
        {
            HttpResponseMessage response = null;
            List<SoftNotification> softNotifications = null;
            int totalSoftNotifications = new int();
            int totalIsReadSoftNotifications = new int();
            softNotifications = _softNotificationRepository.GetAllPaging(page, pageSize, branchId, out totalSoftNotifications, out totalIsReadSoftNotifications).ToList();

            var softNotificationsVM = Mapper.Map<IEnumerable<SoftNotification>, IEnumerable<SoftNotificationViewModel>>(softNotifications);

            foreach (var item in softNotificationsVM)
            {
                if (item.CreatedDate != null)
                    item.CreatedDateConvert = UtilExtensions.ConvertDate(item.CreatedDate.Value);
            }

            var pagedSet = new PaginationSet<SoftNotificationViewModel>()
            {
                Page = page,
                TotalCount = totalSoftNotifications,
                TotalPages = (int)Math.Ceiling((decimal)totalSoftNotifications / pageSize),
                Items = softNotificationsVM,
                TotalIsRead = totalIsReadSoftNotifications
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("updateread")]
        [HttpPost]
        public HttpResponseMessage UpdateRead(HttpRequestMessage request, SoftNotificationViewModel modelVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var model = _softNotificationRepository.GetSingleById(modelVM.Id);
                model.IsRead = true;
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = modelVM.UpdatedBy;
                _softNotificationRepository.Update(model);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.Created, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }
        [Route("updatereadall")]
        [HttpGet]
        public HttpResponseMessage UpdateReadAll(HttpRequestMessage request, int branchId, int updateBy)
        {
            HttpResponseMessage response = null;
            try
            {
                var model = _softNotificationRepository.GetMulti(x => x.ToBranchId == branchId && x.IsRead == false);
                foreach (var item in model)
                {
                    item.IsRead = true;
                    item.UpdatedDate = DateTime.Now;
                    item.UpdatedBy = updateBy;
                    _softNotificationRepository.Update(item);
                }
                _unitOfWork.Commit();                
                response = request.CreateResponse(HttpStatusCode.Created, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }
    }
}