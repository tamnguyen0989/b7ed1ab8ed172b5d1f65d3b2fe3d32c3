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
    [RoutePrefix("api/productlog")]
    public class ShopSanPhamLogController : ApiController
    {
        IShopSanPhamLogRepository _shopSanPhamLogRepository;
        IUnitOfWork _unitOfWork;

        public ShopSanPhamLogController(IShopSanPhamLogRepository shopSanPhamLogRepository, IUnitOfWork unitOfWork)
        {
            _shopSanPhamLogRepository = shopSanPhamLogRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize(Roles = "ProductLogList")]
        [Route("search")]
        public HttpResponseMessage Search(HttpRequestMessage request, ProductLogFilterViewModel productLogFilterVM)
        {
            int currentPage = productLogFilterVM.page.Value;
            int currentPageSize = productLogFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            List<shop_sanphamLogs> shopSanPhamLogs = null;
            int totalShopSanPhamLogs = 0;

            shopSanPhamLogs = _shopSanPhamLogRepository.GetAllPaging(currentPage, currentPageSize, out totalShopSanPhamLogs, productLogFilterVM).ToList();

            IEnumerable<ShopSanPhamLogViewModel> shopSanPhamLogsVM = Mapper.Map<IEnumerable<shop_sanphamLogs>, IEnumerable<ShopSanPhamLogViewModel>>(shopSanPhamLogs);

            PaginationSet<ShopSanPhamLogViewModel> pagedSet = new PaginationSet<ShopSanPhamLogViewModel>()
            {
                Page = currentPage,
                TotalCount = totalShopSanPhamLogs,
                TotalPages = (int)Math.Ceiling((decimal)totalShopSanPhamLogs / currentPageSize),
                Items = shopSanPhamLogsVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [HttpPost]
        [Authorize(Roles = "ProductLogDelete")]
        [Route("deleteall")]
        public HttpResponseMessage DeleteAll(HttpRequestMessage request, ProductLogFilterViewModel productLogFilterVM)
        {
            HttpResponseMessage response = null;
            List<shop_sanphamLogs> shopSanPhamLogs = null;
            DateTime init = new DateTime();
            if (productLogFilterVM.startDateDeleteFilter > init && productLogFilterVM.endDateDeleteFilter > init)
            {
                productLogFilterVM.startDateDeleteFilter = UtilExtensions.ConvertStartDate(productLogFilterVM.startDateDeleteFilter.ToLocalTime());
                productLogFilterVM.endDateDeleteFilter = UtilExtensions.ConvertEndDate(productLogFilterVM.endDateDeleteFilter.ToLocalTime());
                shopSanPhamLogs = _shopSanPhamLogRepository.GetMulti(x => x.CreatedDate >= productLogFilterVM.startDateDeleteFilter && x.CreatedDate <= productLogFilterVM.endDateDeleteFilter).ToList();
                foreach (var item in shopSanPhamLogs)
                {
                    _shopSanPhamLogRepository.Delete(item);
                }
                _unitOfWork.Commit();
            }
            response = request.CreateResponse(HttpStatusCode.OK, true);
            return response;
        }

        [Route("authendelete")]
        [Authorize(Roles = "ProductLogDelete")]
        [HttpGet]
        public HttpResponseMessage AuthenDelete(HttpRequestMessage request)
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