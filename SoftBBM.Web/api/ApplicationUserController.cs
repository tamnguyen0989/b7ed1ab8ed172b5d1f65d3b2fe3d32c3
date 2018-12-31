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
    [RoutePrefix("api/applicationuser")]
    public class ApplicationUserController : ApiController
    {
        private IApplicationGroupRepository _applicationGroupRepository;
        private IApplicationRoleRepository _applicationRoleRepository;
        private IApplicationUserRepository _applicationUserRepository;
        private IApplicationUserRoleRepository _applicationUserRoleRepository;
        private IApplicationUserGroupRepository _applicationUserGroupRepository;
        private ISoftBranchRepository _softBranchRepository;
        IUnitOfWork _unitOfWork;
        public ApplicationUserController(IApplicationUserRepository applicationUserRepository, IApplicationGroupRepository applicationGroupRepository, IApplicationRoleRepository applicationRoleRepository, IApplicationUserRoleRepository applicationUserRoleRepository, IApplicationUserGroupRepository applicationUserGroupRepository, IUnitOfWork unitOfWork, ISoftBranchRepository softBranchRepository)
        {
            this._applicationUserRepository = applicationUserRepository;
            this._applicationGroupRepository = applicationGroupRepository;
            this._applicationRoleRepository = applicationRoleRepository;
            this._applicationUserRoleRepository = applicationUserRoleRepository;
            this._applicationUserGroupRepository = applicationUserGroupRepository;
            this._softBranchRepository = softBranchRepository;
            _unitOfWork = unitOfWork;
        }

        //[Authorize]
        [Route("getlistpaging")]
        [Authorize(Roles = "UserList")]
        [HttpGet]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {
            HttpResponseMessage response = null;
            int totalRow = 0;
            var model = _applicationUserRepository.GetAllPaging(page, pageSize, out totalRow, filter);
            IEnumerable<ApplicationUserViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(model);
            PaginationSet<ApplicationUserViewModel> pagedSet = new PaginationSet<ApplicationUserViewModel>()
            {
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                Items = modelVm
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);

            return response;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            var system = _applicationUserRepository.GetSingleById(0);
            var model = _applicationUserRepository.GetAll().ToList();
            model.Remove(system);
            IEnumerable<ApplicationUserViewModel> modelVm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(model);
            response = request.CreateResponse(HttpStatusCode.OK, modelVm);
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
                var oldModel = _applicationUserRepository.Delete(id);
                _unitOfWork.Commit();

                var responseData = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(oldModel);
                response = request.CreateResponse(HttpStatusCode.OK, responseData);
            }

            return response;
        }

        //[Authorize]
        [Route("create")]
        [Authorize(Roles = "UserAdd")]
        [HttpPost]
        public HttpResponseMessage Post(HttpRequestMessage request, ApplicationUserViewModel ApplicationUserVM)
        {
            if (!ModelState.IsValid)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                try
                {
                    var userByEmail = _applicationUserRepository.GetSingleByCondition(x => x.Email == ApplicationUserVM.Email);
                    if (userByEmail != null)
                    {
                        ModelState.AddModelError("email", "Email đã tồn tại!");
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                    var userByUserName = _applicationUserRepository.GetSingleByCondition(x => x.UserName == ApplicationUserVM.UserName);
                    if (userByUserName != null)
                    {
                        ModelState.AddModelError("userName", "Tên đăng nhập đã tồn tại!");
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }

                    var user = new ApplicationUser();
                    user.UpdateUser(ApplicationUserVM);
                    _applicationUserRepository.Add(user);
                    _unitOfWork.Commit();

                    List<ApplicationUserSoftBranch> applicationUserSoftBranch = new List<ApplicationUserSoftBranch>();
                    foreach (var item in ApplicationUserVM.Branches)
                    {
                        applicationUserSoftBranch.Add(new ApplicationUserSoftBranch()
                        {
                            BranchId = item.Id,
                            UserId = user.Id
                        });
                    }
                    _softBranchRepository.AddUserToBranches(applicationUserSoftBranch, user.Id);
                    _unitOfWork.Commit();

                    if (user.Id > 0)
                    {
                        List<ApplicationUserGroup> applicationUserGroups = new List<ApplicationUserGroup>();
                        foreach (var item in ApplicationUserVM.Groups)
                        {
                            applicationUserGroups.Add(new ApplicationUserGroup()
                            {
                                UserId = user.Id,
                                GroupId = item.Id
                            });

                            //Add role to user from group
                            IEnumerable<ApplicationRole> roles = _applicationRoleRepository.GetListRoleByGroupId(item.Id);
                            foreach (var role in roles)
                            {
                                _applicationUserRoleRepository.DeleteMulti(x => x.UserId == user.Id && x.RoleId==role.Id);
                                _applicationUserRoleRepository.Add(new ApplicationUserRole
                                {
                                    UserId = user.Id,
                                    RoleId = role.Id
                                });
                            }
                            _unitOfWork.Commit();
                        }
                        _applicationGroupRepository.AddUserToGroups(applicationUserGroups, user.Id);
                        _unitOfWork.Commit();

                        return request.CreateResponse(HttpStatusCode.OK, user.FullName);
                    }
                    else
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant create new User!");
                    }
                }
                catch (Exception ex)
                {

                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }

            }
        }

        //[Authorize]
        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMutli(HttpRequestMessage request, string checkedapplicationUsers)
        {

            var ids = new JavaScriptSerializer().Deserialize<List<int>>(checkedapplicationUsers);
            foreach (var item in ids)
            {
                var oldModel = _applicationUserRepository.GetSingleById(item);
                var result = _applicationUserRepository.Delete(oldModel);
            }
            return request.CreateResponse(HttpStatusCode.OK);
        }

        //[Authorize]
        [Route("detail/{id}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            var user = _applicationUserRepository.GetSingleById(id);
            if (user == null)
            {
                return request.CreateErrorResponse(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            var applicationUserVM = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(user);
            applicationUserVM.Password = applicationUserVM.Password.Base64Decode();
            var groups = _applicationGroupRepository.GetListGroupByUserId(applicationUserVM.Id);
            var branches = _applicationUserRepository.GetListBranchByUserName(applicationUserVM.UserName);
            applicationUserVM.Groups = Mapper.Map<IEnumerable<ApplicationGroup>, IEnumerable<ApplicationGroupViewModel>>(groups);
            applicationUserVM.Branches = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<SoftBranchViewModel>>(branches);
            return request.CreateResponse(HttpStatusCode.OK, applicationUserVM);
        }

        //[Authorize]
        [Route("detailuser/{id}")]
        [HttpGet]
        public HttpResponseMessage GetByIdUser(HttpRequestMessage request, int id)
        {
            var user = _applicationUserRepository.GetSingleById(id);
            if (user == null)
            {
                return request.CreateErrorResponse(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            var applicationUserVM = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(user);
            applicationUserVM.Password = applicationUserVM.Password.Base64Decode();
            return request.CreateResponse(HttpStatusCode.OK, applicationUserVM);
        }

        //[Authorize]
        [Route("update")]
        [Authorize(Roles = "UserEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, ApplicationUserViewModel ApplicationUserVM)
        {
            if (ModelState.IsValid)
            {
                var user = _applicationUserRepository.GetSingleById(ApplicationUserVM.Id);
                user.UpdateUser(ApplicationUserVM);
                _applicationUserRepository.Update(user);
                _unitOfWork.Commit();
                List<ApplicationUserGroup> applicationUserGroups = new List<ApplicationUserGroup>();
                foreach (var item in ApplicationUserVM.Groups)
                {
                    applicationUserGroups.Add(new ApplicationUserGroup()
                    {
                        UserId = user.Id,
                        GroupId = item.Id
                    });
                }
                _applicationUserRoleRepository.DeleteMulti(x => x.UserId == user.Id);
                _applicationGroupRepository.AddUserToGroups(applicationUserGroups, user.Id);
                _unitOfWork.Commit();

                List<ApplicationUserSoftBranch> applicationUserSoftBranch = new List<ApplicationUserSoftBranch>();
                foreach (var item in ApplicationUserVM.Branches)
                {
                    applicationUserSoftBranch.Add(new ApplicationUserSoftBranch()
                    {
                        BranchId = item.Id,
                        UserId = user.Id
                    });
                }
                _softBranchRepository.AddUserToBranches(applicationUserSoftBranch, user.Id);
                _unitOfWork.Commit();

                foreach (var item in ApplicationUserVM.Groups)
                {
                    //Add role to user from group
                    IEnumerable < ApplicationRole > roles = _applicationRoleRepository.GetListRoleByGroupId(item.Id);
                    foreach (var role in roles)
                    {
                        _applicationUserRoleRepository.DeleteMulti(x => x.UserId == user.Id && x.RoleId == role.Id);
                        _applicationUserRoleRepository.Add(new ApplicationUserRole
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    _unitOfWork.Commit();
                }
                return request.CreateResponse(HttpStatusCode.OK, user.FullName);
            }
            else
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        //[Authorize]
        [Route("updateinfo")]
        [HttpPost]
        public HttpResponseMessage UpdateInfo(HttpRequestMessage request, ApplicationUserViewModel ApplicationUserVM)
        {
            if (ModelState.IsValid)
            {
                if(ApplicationUserVM.Id== ApplicationUserVM.userId)
                {
                    var user = _applicationUserRepository.GetSingleById(ApplicationUserVM.Id);
                    user.UpdateUser(ApplicationUserVM);
                    _applicationUserRepository.Update(user);
                    _unitOfWork.Commit();
                    return request.CreateResponse(HttpStatusCode.OK, user.FullName);
                }
                else
                    return request.CreateResponse(HttpStatusCode.BadRequest, "404 not found!");
            }
            else
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        //[Route("getlistbranch")]
        //public HttpResponseMessage GetListBranch(HttpRequestMessage request, string userName)
        //{
        //    if (!string.IsNullOrEmpty(userName))
        //    {
        //        var branches = _applicationUserRepository.GetListBranchByUserName(userName);
        //        var branchesVM = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<SoftBranchViewModel>>(branches);
        //        return request.CreateResponse(HttpStatusCode.OK, branchesVM);
        //    }
        //    else
        //    {
        //        return request.CreateResponse(HttpStatusCode.BadRequest, "Login is required");
        //    }
        //}

        [Route("getlistbranch")]
        public HttpResponseMessage GetListBranch(HttpRequestMessage request, string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var branches = _applicationUserRepository.GetListBranchByUserName(userName).OrderBy(x=>x.Priority);
                var user = _applicationUserRepository.GetSingleByCondition(x => x.UserName == userName);
                var userVM = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(user);
                var branchesVM = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<SoftBranchViewModel>>(branches);
                SoftBranchLoginViewModel softBranchLoginVM = new SoftBranchLoginViewModel();
                softBranchLoginVM.ApplicationUser = userVM;
                softBranchLoginVM.SoftBranchs = branchesVM;
                return request.CreateResponse(HttpStatusCode.OK, softBranchLoginVM);
            }
            else
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, "Login is required");
            }
        }

        [Route("authenadd")]
        [Authorize(Roles = "UserAdd")]
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
        [Authorize(Roles = "UserEdit")]
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

        [Route("autheneditinfo")]
        [Authorize]
        [HttpGet]
        public HttpResponseMessage AuthenEditInfo(HttpRequestMessage request)
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
