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
using System.Web.Script.Serialization;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/applicationrole")]
    public class ApplicationRoleController : ApiController
    {
        #region Initialize
        IApplicationRoleRepository _applicationRoleRepository;
        IApplicationRoleCategoryRepository _applicationRoleCategoryRepository;
        IUnitOfWork _unitOfWork;

        public ApplicationRoleController(IApplicationRoleRepository applicationRoleRepository, IUnitOfWork unitOfWork, IApplicationRoleCategoryRepository applicationRoleCategoryRepository)
        {
            _applicationRoleRepository = applicationRoleRepository;
            _applicationRoleCategoryRepository = applicationRoleCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        [Route("getlistall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            var model = _applicationRoleRepository.GetAll();
            IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(model);
            response = request.CreateResponse(HttpStatusCode.OK, modelVm);
            return response;

        }


        [Route("getlistpaging")]
        [Authorize(Roles = "RoleList")]
        [HttpGet]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {
            HttpResponseMessage response = null;
            int totalRow = 0;
            var model = _applicationRoleRepository.GetAllPaging(page, pageSize, out totalRow, filter);
            IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(model);
            PaginationSet<ApplicationRoleViewModel> pagedSet = new PaginationSet<ApplicationRoleViewModel>()
            {
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                Items = modelVm
            };
            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }




        [Route("detail/{id}")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {

            var model = _applicationRoleRepository.GetSingleByCondition(x => x.Id == id);
            var responseData = Mapper.Map<ApplicationRole, ApplicationRoleViewModel>(model);
            var response = request.CreateResponse(HttpStatusCode.OK, responseData);
            return response;

        }


        [Route("update")]
        [Authorize(Roles = "RoleEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, ApplicationRoleViewModel ApplicationRoleVM)
        {
            HttpResponseMessage response = null;
            if (ModelState.IsValid)
            {
                var model = _applicationRoleRepository.GetSingleByCondition(x => x.Id == ApplicationRoleVM.Id);
                model.UpdateApplicationRole(ApplicationRoleVM, "update");
                _applicationRoleRepository.Update(model);
                _unitOfWork.Commit();
                var responseData = Mapper.Map<ApplicationRole, ApplicationRoleViewModel>(model);
                response = request.CreateResponse(HttpStatusCode.Created, responseData);
            }
            else
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            return response;
        }


        [Route("create")]
        [Authorize(Roles = "RoleAdd")]
        public HttpResponseMessage Post(HttpRequestMessage request, ApplicationRoleViewModel ApplicationRoleVM)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                ApplicationRole newApplicationrole = new ApplicationRole();
                newApplicationrole.UpdateApplicationRole(ApplicationRoleVM);

                var model = _applicationRoleRepository.Add(newApplicationrole);
                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.Created, model);

            }
            return response;
        }


        [Route("delete")]
        [HttpDelete]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                var model = _applicationRoleRepository.GetSingleByCondition(x => x.Id == id);
                _applicationRoleRepository.Delete(model);
                _unitOfWork.Commit();

                var responseData = Mapper.Map<ApplicationRole, ApplicationRoleViewModel>(model);
                response = request.CreateResponse(HttpStatusCode.OK, responseData);
            }

            return response;
        }


        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMutli(HttpRequestMessage request, string checkedApplicationRoles)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                var ids = new JavaScriptSerializer().Deserialize<List<string>>(checkedApplicationRoles);
                foreach (var item in ids)
                {
                    var model = _applicationRoleRepository.GetSingleByCondition(x => x.Name == item);
                    _applicationRoleRepository.Delete(model);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, ids.Count);
            }

            return response;
        }

        [Route("getlistallcategory")]
        [HttpGet]
        public HttpResponseMessage GetAllCategory(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            var model = _applicationRoleCategoryRepository.GetAll();
            IEnumerable<ApplicationRoleCategoryViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationRoleCategory>, IEnumerable<ApplicationRoleCategoryViewModel>>(model);
            response = request.CreateResponse(HttpStatusCode.OK, modelVm);
            return response;

        }

        [Route("getlistallcategoryroleadd")]
        [Authorize(Roles = "RoleAdd")]
        [HttpGet]
        public HttpResponseMessage GetAllCategoryRoleAdd(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            var model = _applicationRoleCategoryRepository.GetAll();
            IEnumerable<ApplicationRoleCategoryViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationRoleCategory>, IEnumerable<ApplicationRoleCategoryViewModel>>(model);
            response = request.CreateResponse(HttpStatusCode.OK, modelVm);
            return response;

        }

        [Route("getlistallcategoryroleedit")]
        [Authorize(Roles = "RoleEdit")]
        [HttpGet]
        public HttpResponseMessage GetAllCategoryRoleEdit(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            var model = _applicationRoleCategoryRepository.GetAll();
            IEnumerable<ApplicationRoleCategoryViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationRoleCategory>, IEnumerable<ApplicationRoleCategoryViewModel>>(model);
            response = request.CreateResponse(HttpStatusCode.OK, modelVm);
            return response;

        }

        [Route("getlistallbycategory")]
        [HttpGet]
        public HttpResponseMessage GetAllByCategory(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            RoleCategory result = new RoleCategory();

            var orderRoles = _applicationRoleRepository.GetListRoleByCategoryId(1);
            var orderRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(orderRoles);
            result.OrderRoles = orderRolesVM;

            var stockinRoles = _applicationRoleRepository.GetListRoleByCategoryId(2);
            var stockinRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stockinRoles);
            result.StockInRoles = stockinRolesVM;

            var stockoutRoles = _applicationRoleRepository.GetListRoleByCategoryId(3);
            var stockoutRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stockoutRoles);
            result.StockOutRoles = stockoutRolesVM;

            var bookRoles = _applicationRoleRepository.GetListRoleByCategoryId(4);
            var bookRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(bookRoles);
            result.BookRoles = bookRolesVM;

            var bookBranchRoles = _applicationRoleRepository.GetListRoleByCategoryId(5);
            var bookBranchRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(bookBranchRoles);
            result.BookBranchRoles = bookBranchRolesVM;

            var productRoles = _applicationRoleRepository.GetListRoleByCategoryId(6);
            var productRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(productRoles);
            result.ProductRoles = productRolesVM;

            var adjustmentStockRoles = _applicationRoleRepository.GetListRoleByCategoryId(7);
            var adjustmentStockRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(adjustmentStockRoles);
            result.AdjustmentStockRoles = adjustmentStockRolesVM;

            var stampRoles = _applicationRoleRepository.GetListRoleByCategoryId(8);
            var stampRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stampRoles);
            result.StampRoles = stampRolesVM;

            var branchRoles = _applicationRoleRepository.GetListRoleByCategoryId(9);
            var branchRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(branchRoles);
            result.BranchRoles = branchRolesVM;

            var supplierRoles = _applicationRoleRepository.GetListRoleByCategoryId(10);
            var supplierRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(supplierRoles);
            result.SupplierRoles = supplierRolesVM;

            var channelRoles = _applicationRoleRepository.GetListRoleByCategoryId(11);
            var channelRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(channelRoles);
            result.ChannelRoles = channelRolesVM;

            var userRoles = _applicationRoleRepository.GetListRoleByCategoryId(12);
            var userRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(userRoles);
            result.UserRoles = userRolesVM;

            var ownRoles = _applicationRoleRepository.GetListRoleByCategoryId(13);
            var ownRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(ownRoles);
            result.OwnRoles = ownRolesVM;

            var groupRoles = _applicationRoleRepository.GetListRoleByCategoryId(14);
            var groupRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(groupRoles);
            result.GroupRoles = groupRolesVM;

            var customerRoles = _applicationRoleRepository.GetListRoleByCategoryId(15);
            var customerRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(customerRoles);
            result.GroupRoles = customerRolesVM;

            response = request.CreateResponse(HttpStatusCode.OK, result);
            return response;

        }


        [Route("getlistallbycategorygroupadd")]
        [Authorize(Roles = "GroupAdd")]
        [HttpGet]
        public HttpResponseMessage GetAllByCategoryGroupAdd(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            RoleCategory result = new RoleCategory();

            var orderRoles = _applicationRoleRepository.GetListRoleByCategoryId(1);
            var orderRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(orderRoles);
            result.OrderRoles = orderRolesVM;

            var stockinRoles = _applicationRoleRepository.GetListRoleByCategoryId(2);
            var stockinRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stockinRoles);
            result.StockInRoles = stockinRolesVM;

            var stockoutRoles = _applicationRoleRepository.GetListRoleByCategoryId(3);
            var stockoutRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stockoutRoles);
            result.StockOutRoles = stockoutRolesVM;

            var bookRoles = _applicationRoleRepository.GetListRoleByCategoryId(4);
            var bookRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(bookRoles);
            result.BookRoles = bookRolesVM;

            var bookBranchRoles = _applicationRoleRepository.GetListRoleByCategoryId(5);
            var bookBranchRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(bookBranchRoles);
            result.BookBranchRoles = bookBranchRolesVM;

            var productRoles = _applicationRoleRepository.GetListRoleByCategoryId(6);
            var productRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(productRoles);
            result.ProductRoles = productRolesVM;

            var adjustmentStockRoles = _applicationRoleRepository.GetListRoleByCategoryId(7);
            var adjustmentStockRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(adjustmentStockRoles);
            result.AdjustmentStockRoles = adjustmentStockRolesVM;

            var stampRoles = _applicationRoleRepository.GetListRoleByCategoryId(8);
            var stampRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stampRoles);
            result.StampRoles = stampRolesVM;

            var branchRoles = _applicationRoleRepository.GetListRoleByCategoryId(9);
            var branchRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(branchRoles);
            result.BranchRoles = branchRolesVM;

            var supplierRoles = _applicationRoleRepository.GetListRoleByCategoryId(10);
            var supplierRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(supplierRoles);
            result.SupplierRoles = supplierRolesVM;

            var channelRoles = _applicationRoleRepository.GetListRoleByCategoryId(11);
            var channelRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(channelRoles);
            result.ChannelRoles = channelRolesVM;

            var userRoles = _applicationRoleRepository.GetListRoleByCategoryId(12);
            var userRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(userRoles);
            result.UserRoles = userRolesVM;

            var ownRoles = _applicationRoleRepository.GetListRoleByCategoryId(13);
            var ownRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(ownRoles);
            result.OwnRoles = ownRolesVM;

            var groupRoles = _applicationRoleRepository.GetListRoleByCategoryId(14);
            var groupRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(groupRoles);
            result.GroupRoles = groupRolesVM;

            var customerRoles = _applicationRoleRepository.GetListRoleByCategoryId(15);
            var customerRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(customerRoles);
            result.CustomerRoles = customerRolesVM;

            var productCategoryRoles = _applicationRoleRepository.GetListRoleByCategoryId(16);
            var productCategoryRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(productCategoryRoles);
            result.ProductCategoryRoles = productCategoryRolesVM;

            var returnSupplierRoles = _applicationRoleRepository.GetListRoleByCategoryId(17);
            var returnSupplierRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(returnSupplierRoles);
            result.ReturnSupplierRoles = returnSupplierRolesVM;

            var reportRoles = _applicationRoleRepository.GetListRoleByCategoryId(18);
            var reportRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(reportRoles);
            result.ReportRoles = reportRolesVM;

            response = request.CreateResponse(HttpStatusCode.OK, result);
            return response;

        }

        [Route("getlistallbycategorygroupedit")]
        [Authorize(Roles = "GroupEdit")]
        [HttpGet]
        public HttpResponseMessage GetAllByCategoryGroupEdit(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            RoleCategory result = new RoleCategory();

            var orderRoles = _applicationRoleRepository.GetListRoleByCategoryId(1);
            var orderRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(orderRoles);
            result.OrderRoles = orderRolesVM;

            var stockinRoles = _applicationRoleRepository.GetListRoleByCategoryId(2);
            var stockinRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stockinRoles);
            result.StockInRoles = stockinRolesVM;

            var stockoutRoles = _applicationRoleRepository.GetListRoleByCategoryId(3);
            var stockoutRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stockoutRoles);
            result.StockOutRoles = stockoutRolesVM;

            var bookRoles = _applicationRoleRepository.GetListRoleByCategoryId(4);
            var bookRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(bookRoles);
            result.BookRoles = bookRolesVM;

            var bookBranchRoles = _applicationRoleRepository.GetListRoleByCategoryId(5);
            var bookBranchRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(bookBranchRoles);
            result.BookBranchRoles = bookBranchRolesVM;

            var productRoles = _applicationRoleRepository.GetListRoleByCategoryId(6);
            var productRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(productRoles);
            result.ProductRoles = productRolesVM;

            var adjustmentStockRoles = _applicationRoleRepository.GetListRoleByCategoryId(7);
            var adjustmentStockRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(adjustmentStockRoles);
            result.AdjustmentStockRoles = adjustmentStockRolesVM;

            var stampRoles = _applicationRoleRepository.GetListRoleByCategoryId(8);
            var stampRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(stampRoles);
            result.StampRoles = stampRolesVM;

            var branchRoles = _applicationRoleRepository.GetListRoleByCategoryId(9);
            var branchRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(branchRoles);
            result.BranchRoles = branchRolesVM;

            var supplierRoles = _applicationRoleRepository.GetListRoleByCategoryId(10);
            var supplierRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(supplierRoles);
            result.SupplierRoles = supplierRolesVM;

            var channelRoles = _applicationRoleRepository.GetListRoleByCategoryId(11);
            var channelRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(channelRoles);
            result.ChannelRoles = channelRolesVM;

            var userRoles = _applicationRoleRepository.GetListRoleByCategoryId(12);
            var userRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(userRoles);
            result.UserRoles = userRolesVM;

            var ownRoles = _applicationRoleRepository.GetListRoleByCategoryId(13);
            var ownRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(ownRoles);
            result.OwnRoles = ownRolesVM;

            var groupRoles = _applicationRoleRepository.GetListRoleByCategoryId(14);
            var groupRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(groupRoles);
            result.GroupRoles = groupRolesVM;

            var customerRoles = _applicationRoleRepository.GetListRoleByCategoryId(15);
            var customerRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(customerRoles);
            result.CustomerRoles = customerRolesVM;

            var productCategoryRoles = _applicationRoleRepository.GetListRoleByCategoryId(16);
            var productCategoryRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(productCategoryRoles);
            result.ProductCategoryRoles = productCategoryRolesVM;

            var returnSupplierRoles = _applicationRoleRepository.GetListRoleByCategoryId(17);
            var returnSupplierRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(returnSupplierRoles);
            result.ReturnSupplierRoles = returnSupplierRolesVM;

            var reportRoles = _applicationRoleRepository.GetListRoleByCategoryId(18);
            var reportRolesVM = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(reportRoles);
            result.ReportRoles = reportRolesVM;

            response = request.CreateResponse(HttpStatusCode.OK, result);
            return response;

        }

    }
}