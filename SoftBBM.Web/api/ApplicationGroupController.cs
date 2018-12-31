using AutoMapper;
using SoftBBM.Web.App_Start;
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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/applicationgroup")]
    public class ApplicationGroupController : ApiController
    {
        #region Initialize
        IApplicationGroupRepository _applicationGroupRepository;
        IApplicationRoleRepository _applicationRoleRepository;
        IApplicationUserRepository _applicationUserRepository;
        IApplicationUserRoleRepository _applicationUserRoleRepository;
        //ApplicationUserManager _userManager;
        IUnitOfWork _unitOfWork;
        public ApplicationGroupController(IApplicationGroupRepository applicationGroupRepository, IApplicationRoleRepository applicationRoleRepository, IUnitOfWork unitOfWork, IApplicationUserRepository applicationUserRepository, IApplicationUserRoleRepository applicationUserRoleRepository
            //,ApplicationUserManager userManager
            )
        {
            this._applicationGroupRepository = applicationGroupRepository;
            this._applicationRoleRepository = applicationRoleRepository;
            this._applicationUserRepository = applicationUserRepository;
            this._applicationUserRoleRepository = applicationUserRoleRepository;
            //this._userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        #endregion

        [Route("getlistall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {

            HttpResponseMessage response = null;
            var model = _applicationGroupRepository.GetAll();
            IEnumerable<ApplicationGroupViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(model);

            response = request.CreateResponse(HttpStatusCode.OK, modelVm);

            return response;

        }

        [Route("getlistpaging")]
        [Authorize(Roles = "GroupList")]
        [HttpGet]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {

            HttpResponseMessage response = null;
            int totalRow = 0;
            var model = _applicationGroupRepository.GetAllPaging(page, pageSize, out totalRow, filter);
            IEnumerable<ApplicationGroupViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(model);

            PaginationSet<ApplicationGroupViewModel> pagedSet = new PaginationSet<ApplicationGroupViewModel>()
            {
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                Items = modelVm
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);

            return response;
        }

        [Route("getbyid/{id:int}")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {

            var model = _applicationGroupRepository.GetSingleById(id);
            var responseData = Mapper.Map<ApplicationGroup, ApplicationGroupViewModel>(model);
            var roles = _applicationRoleRepository.GetListRoleByGroupId(id);
            responseData.Roles = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<ApplicationRoleViewModel>>(roles);

            responseData.OrderRoles = new List<ApplicationRoleViewModel>();
            responseData.StockInRoles = new List<ApplicationRoleViewModel>();
            responseData.StockOutRoles = new List<ApplicationRoleViewModel>();
            responseData.BookRoles = new List<ApplicationRoleViewModel>();
            responseData.BookBranchRoles = new List<ApplicationRoleViewModel>();
            responseData.ProductRoles = new List<ApplicationRoleViewModel>();
            responseData.AdjustmentStockRoles = new List<ApplicationRoleViewModel>();
            responseData.StampRoles = new List<ApplicationRoleViewModel>();
            responseData.BranchRoles = new List<ApplicationRoleViewModel>();
            responseData.SupplierRoles = new List<ApplicationRoleViewModel>();
            responseData.ChannelRoles = new List<ApplicationRoleViewModel>();
            responseData.UserRoles = new List<ApplicationRoleViewModel>();
            responseData.OwnRoles = new List<ApplicationRoleViewModel>();
            responseData.GroupRoles = new List<ApplicationRoleViewModel>();
            responseData.CustomerRoles = new List<ApplicationRoleViewModel>();
            responseData.ProductCategoryRoles = new List<ApplicationRoleViewModel>();
            responseData.ReturnSupplierRoles = new List<ApplicationRoleViewModel>();

            foreach (var item in responseData.Roles)
            {
                switch (item.CategoryId)
                {
                    case 1:
                        responseData.OrderRoles.Add(item); 
                        break;
                    case 2:
                        responseData.StockInRoles.Add(item);
                        break;
                    case 3:
                        responseData.StockOutRoles.Add(item);
                        break;
                    case 4:
                        responseData.BookRoles.Add(item);
                        break;
                    case 5:
                        responseData.BookBranchRoles.Add(item);
                        break;
                    case 6:
                        responseData.ProductRoles.Add(item);
                        break;
                    case 7:
                        responseData.AdjustmentStockRoles.Add(item);
                        break;
                    case 8:
                        responseData.StampRoles.Add(item);
                        break;
                    case 9:
                        responseData.BranchRoles.Add(item);
                        break;
                    case 10:
                        responseData.SupplierRoles.Add(item);
                        break;
                    case 11:
                        responseData.ChannelRoles.Add(item);
                        break;
                    case 12:
                        responseData.UserRoles.Add(item);
                        break;
                    case 13:
                        responseData.OwnRoles.Add(item);
                        break;
                    case 14:
                        responseData.GroupRoles.Add(item);
                        break;
                    case 15:
                        responseData.CustomerRoles.Add(item);
                        break;
                    case 16:
                        responseData.ProductCategoryRoles.Add(item);
                        break;
                    case 17:
                        responseData.ReturnSupplierRoles.Add(item);
                        break;
                }
            }

            var response = request.CreateResponse(HttpStatusCode.OK, responseData);
            return response;

        }

        [Route("update")]
        [Authorize(Roles = "GroupEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, ApplicationGroupViewModel applicationGroupVM)
        {
            HttpResponseMessage response = null;

            if (ModelState.IsValid)
            {
                var model = _applicationGroupRepository.GetSingleById(applicationGroupVM.Id);
                model.UpdateApplicationGroup(applicationGroupVM);
                _applicationGroupRepository.Update(model);
                _unitOfWork.Commit();

                //save Group
                List<ApplicationRoleGroup> applicationRoleGroups = new List<ApplicationRoleGroup>();
                foreach (var item in applicationGroupVM.Roles)
                {
                    applicationRoleGroups.Add(new ApplicationRoleGroup()
                    {
                        GroupId = model.Id,
                        RoleId = item.Id
                    });
                }
                _applicationRoleRepository.AddRolesToGroup(applicationRoleGroups, model.Id);
                var oldUsersByGroupID = _applicationGroupRepository.GetListUserByGroupId(model.Id);
                var oldrolesByGroudID = _applicationRoleRepository.GetListRoleByGroupId(model.Id);
                foreach (var oldUser in oldUsersByGroupID)
                {
                    foreach (var oldRole in oldrolesByGroudID)
                    {
                        _applicationUserRoleRepository.DeleteMulti(x => x.RoleId == oldRole.Id && x.UserId==oldUser.Id);
                    }
                }
                _unitOfWork.Commit();

                //add roles to users from group

                var rolesByGroudID = _applicationRoleRepository.GetListRoleByGroupId(applicationGroupVM.Id);
                var usersByGroupID = _applicationGroupRepository.GetListUserByGroupId(applicationGroupVM.Id);
                foreach (var user in usersByGroupID.ToList())
                {      
                    foreach (var role in rolesByGroudID.ToList())
                    {
                        _applicationUserRoleRepository.DeleteMulti(x => x.UserId == user.Id && x.RoleId==role.Id);
                        _applicationUserRoleRepository.Add(new ApplicationUserRole {
                            UserId=user.Id,
                            RoleId=role.Id
                        });
                    }
                    _unitOfWork.Commit();
                }
                var responseData = Mapper.Map<ApplicationGroup, ApplicationGroupViewModel>(model);
                response = request.CreateResponse(HttpStatusCode.Created, responseData);
            }
            else
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            return response;
        }

        [Route("create")]
        [Authorize(Roles = "GroupAdd")]
        public HttpResponseMessage Post(HttpRequestMessage request, ApplicationGroupViewModel applicationGroupVM)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                ApplicationGroup newApplicationGroup = new ApplicationGroup();
                newApplicationGroup.UpdateApplicationGroup(applicationGroupVM);
                var model = _applicationGroupRepository.Add(newApplicationGroup);
                _unitOfWork.Commit();
                List<ApplicationRoleGroup> applicationRoleGroups = new List<ApplicationRoleGroup>();
                foreach (var item in applicationGroupVM.Roles)
                {
                    applicationRoleGroups.Add(new ApplicationRoleGroup()
                    {
                        GroupId = model.Id,
                        RoleId = item.Id
                    });
                }
                _applicationRoleRepository.AddRolesToGroup(applicationRoleGroups, model.Id);
                _unitOfWork.Commit();
                var responseModel = Mapper.Map<ApplicationGroup, ApplicationGroupViewModel>(model);
                response = request.CreateResponse(HttpStatusCode.Created, responseModel);
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
                var oldModel = _applicationGroupRepository.Delete(id);
                _unitOfWork.Commit();

                var responseData = Mapper.Map<ApplicationGroup, ApplicationGroupViewModel>(oldModel);
                response = request.CreateResponse(HttpStatusCode.OK, responseData);
            }

            return response;
        }

        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMutli(HttpRequestMessage request, string checkedApplicationGroups)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                var ids = new JavaScriptSerializer().Deserialize<List<int>>(checkedApplicationGroups);
                foreach (var item in ids)
                {
                    _applicationGroupRepository.Delete(item);
                }
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, ids.Count);
            }

            return response;
        }
    }
}