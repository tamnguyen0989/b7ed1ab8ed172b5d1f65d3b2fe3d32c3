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
    [RoutePrefix("api/supplier")]
    public class SoftSupplierController : ApiController
    {
        ISoftSupplierRepository _softSupplierRepository;
        ISoftSupplierVatStatusRepository _softSupplierVatStatusRepository;
        IUnitOfWork _unitOfWork;

        public SoftSupplierController(ISoftSupplierRepository softSupplierRepository, ISoftSupplierVatStatusRepository softSupplierVatStatusRepository, IUnitOfWork unitOfWork)
        {
            _softSupplierRepository = softSupplierRepository;
            _softSupplierVatStatusRepository = softSupplierVatStatusRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "SupplierList")]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftSupplier> suppliers = null;
            int totalSuppliers = new int();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.Trim().ToLower();

                suppliers = _softSupplierRepository.GetMulti(c => c.Name.ToLower().Contains(filter))
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalSuppliers = _softSupplierRepository.GetMulti(c => c.Name.ToLower().Contains(filter))
                    .Count();
            }
            else
            {
                suppliers = _softSupplierRepository.GetAll()
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                .ToList();

                totalSuppliers = _softSupplierRepository.GetAll().Count();
            }

            IEnumerable<SoftSupplierViewModel> suppliersVM = Mapper.Map<IEnumerable<SoftSupplier>, IEnumerable<SoftSupplierViewModel>>(suppliers);

            PaginationSet<SoftSupplierViewModel> pagedSet = new PaginationSet<SoftSupplierViewModel>()
            {
                Page = currentPage,
                TotalCount = totalSuppliers,
                TotalPages = (int)Math.Ceiling((decimal)totalSuppliers / currentPageSize),
                Items = suppliersVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("add")]
        [Authorize(Roles = "SupplierAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, SoftSupplierViewModel softSupplierViewModel)
        {
            HttpResponseMessage response = null;
            try
            {
                var newSoftSupplier = new SoftSupplier();
                newSoftSupplier.UpdateSoftSupplier(softSupplierViewModel);
                newSoftSupplier.CreatedDate = DateTime.Now;
                newSoftSupplier.CreatedBy = softSupplierViewModel.CreatedBy;
                _softSupplierRepository.Add(newSoftSupplier);
                _unitOfWork.Commit();
                var responseData = Mapper.Map<SoftSupplier, SoftSupplierViewModel>(newSoftSupplier);
                response = request.CreateResponse(HttpStatusCode.Created, responseData);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.Created, ex.Message);
            }
            return response;
        }

        [Route("delete")]
        [HttpDelete]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            HttpResponseMessage response = null;
            var oldModel = _softSupplierRepository.Delete(id);
            _unitOfWork.Commit();
            var responseData = Mapper.Map<SoftSupplier, SoftSupplierViewModel>(oldModel);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        [Route("Update")]
        [Authorize(Roles = "SupplierEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, SoftSupplierViewModel softSupplierViewModel)
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
                var oldSupplier = _softSupplierRepository.GetSingleById(softSupplierViewModel.Id);
                oldSupplier.UpdateSoftSupplier(softSupplierViewModel);
                oldSupplier.UpdatedBy = softSupplierViewModel.UpdatedBy;
                oldSupplier.UpdatedDate = DateTime.Now;
                _softSupplierRepository.Update(oldSupplier);
                _unitOfWork.Commit();
                var responseData = Mapper.Map<SoftSupplier, SoftSupplierViewModel>(oldSupplier);
                response = request.CreateResponse(HttpStatusCode.Created, responseData);
            }
            return response;
        }

        [Route("GetAll")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string order = "")
        {
            HttpResponseMessage response = null;
            try
            {
                var suppliers = _softSupplierRepository.GetAll();
                if (order == "Name")
                    suppliers = suppliers.OrderBy(x => x.Name);
                else suppliers = suppliers.OrderByDescending(x => x.Id);
                var suppliersVM = Mapper.Map<IEnumerable<SoftSupplier>, IEnumerable<SoftSupplierViewModel>>(suppliers);
                response = request.CreateResponse(HttpStatusCode.OK, suppliersVM);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("getallstockinadd")]
        [Authorize(Roles = "StockInAdd")]
        [HttpGet]
        public HttpResponseMessage GetAllStockInAdd(HttpRequestMessage request, string order = "")
        {
            HttpResponseMessage response = null;
            try
            {
                var suppliers = _softSupplierRepository.GetAll();
                if (order == "Name")
                    suppliers = suppliers.OrderBy(x => x.Name);
                else suppliers = suppliers.OrderByDescending(x => x.Id);
                var suppliersVM = Mapper.Map<IEnumerable<SoftSupplier>, IEnumerable<SoftSupplierViewModel>>(suppliers);
                response = request.CreateResponse(HttpStatusCode.OK, suppliersVM);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("getallstockoutadd")]
        [Authorize(Roles = "StockOutAdd")]
        [HttpGet]
        public HttpResponseMessage GetAllStockOutAdd(HttpRequestMessage request, string order = "")
        {
            HttpResponseMessage response = null;
            try
            {
                var suppliers = _softSupplierRepository.GetAll();
                if (order == "Name")
                    suppliers = suppliers.OrderBy(x => x.Name);
                else suppliers = suppliers.OrderByDescending(x => x.Id);
                var suppliersVM = Mapper.Map<IEnumerable<SoftSupplier>, IEnumerable<SoftSupplierViewModel>>(suppliers);
                response = request.CreateResponse(HttpStatusCode.OK, suppliersVM);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("getallbookadd")]
        [Authorize(Roles = "BookAdd")]
        [HttpGet]
        public HttpResponseMessage GetAllBookAdd(HttpRequestMessage request, string order = "")
        {
            HttpResponseMessage response = null;
            try
            {
                var suppliers = _softSupplierRepository.GetAll();
                if (order == "Name")
                    suppliers = suppliers.OrderBy(x => x.Name);
                else suppliers = suppliers.OrderByDescending(x => x.Id);
                var suppliersVM = Mapper.Map<IEnumerable<SoftSupplier>, IEnumerable<SoftSupplierViewModel>>(suppliers);
                response = request.CreateResponse(HttpStatusCode.OK, suppliersVM);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("getallbranchbookadd")]
        [Authorize(Roles = "BranchBookAdd")]
        [HttpGet]
        public HttpResponseMessage GetAllBookBranchAdd(HttpRequestMessage request, string order = "")
        {
            HttpResponseMessage response = null;
            try
            {
                var suppliers = _softSupplierRepository.GetAll();
                if (order == "Name")
                    suppliers = suppliers.OrderBy(x => x.Name);
                else suppliers = suppliers.OrderByDescending(x => x.Id);
                var suppliersVM = Mapper.Map<IEnumerable<SoftSupplier>, IEnumerable<SoftSupplierViewModel>>(suppliers);
                response = request.CreateResponse(HttpStatusCode.OK, suppliersVM);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("authenadd")]
        [Authorize(Roles = "SupplierAdd")]
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
        [Authorize(Roles = "SupplierEdit")]
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

        [Route("getallvatstatus")]
        [HttpGet]
        public HttpResponseMessage GetAllVatStatus(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                var vatStatuses = _softSupplierVatStatusRepository.GetAll().OrderBy(x => x.Id);
                var vatStatusesVM = Mapper.Map<IEnumerable<SoftSupplierVatStatu>, IEnumerable<SoftSupplierVatStatuViewModel>>(vatStatuses);
                response = request.CreateResponse(HttpStatusCode.OK, vatStatusesVM);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("delete")]
        [Authorize(Roles = "SupplierEdit")]
        [HttpGet]
        public HttpResponseMessage Detele(HttpRequestMessage request, int supplierId)
        {
            HttpResponseMessage response = null;
            try
            {
                if (supplierId > 0)
                {
                    _softSupplierRepository.DeleteSupplier(supplierId);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Không có nhà cung cấp này.");

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
