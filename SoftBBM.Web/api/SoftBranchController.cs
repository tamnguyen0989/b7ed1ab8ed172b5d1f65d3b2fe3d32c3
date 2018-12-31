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
    [RoutePrefix("api/branch")]
    public class SoftBranchController : ApiController
    {
        ISoftBranchRepository _softBranchRepository;
        IUnitOfWork _unitOfWork;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftStockRepository _softStockRepository;

        public SoftBranchController(ISoftBranchRepository softBranchRepository, IUnitOfWork unitOfWork, IShopSanPhamRepository shopSanPhamRepository, ISoftStockRepository softStockRepository)
        {
            _softBranchRepository = softBranchRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _softStockRepository = softStockRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "BranchList")]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;
            HttpResponseMessage response = null;
            List<SoftBranch> branches = null;
            int totalBranches = new int();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.Trim().ToLower();

                branches = _softBranchRepository.GetMulti(c => c.Name.ToLower().Contains(filter) ||
                        c.Phone.ToLower().Contains(filter) || c.Address.ToLower().Contains(filter))
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalBranches = _softBranchRepository.GetMulti(c => c.Name.ToLower().Contains(filter) ||
                        c.Phone.ToLower().Contains(filter) || c.Address.ToLower().Contains(filter))
                    .Count();
            }
            else
            {
                branches = _softBranchRepository.GetAll()
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                .ToList();

                totalBranches = _softBranchRepository.GetAll().Count();
            }

            IEnumerable<SoftBranchViewModel> branchesVM = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<SoftBranchViewModel>>(branches);

            PaginationSet<SoftBranchViewModel> pagedSet = new PaginationSet<SoftBranchViewModel>()
            {
                Page = currentPage,
                TotalCount = totalBranches,
                TotalPages = (int)Math.Ceiling((decimal)totalBranches / currentPageSize),
                Items = branchesVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("add")]
        [Authorize(Roles = "BranchAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, SoftBranchViewModel SoftBranchViewModel)
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
                var softBranch = _softBranchRepository.GetSingleByCondition(x => x.Code == SoftBranchViewModel.Code);
                if (softBranch != null)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Code này đã có!");
                }
                else
                {
                    var newSoftBranch = new SoftBranch();
                    newSoftBranch.UpdateSoftBranch(SoftBranchViewModel);
                    _softBranchRepository.Add(newSoftBranch);
                    _unitOfWork.Commit();

                    //migration branch,stock and product
                    var products = _shopSanPhamRepository.GetAllIds();
                    foreach (var product in products.ToList())
                    {
                        var newstock = new SoftBranchProductStock();
                        newstock.ProductId = product;
                        newstock.BranchId = newSoftBranch.Id;
                        newstock.StockTotal = 0;
                        newstock.CreatedDate = DateTime.Now;
                        _softStockRepository.Add(newstock);
                    }
                    _unitOfWork.Commit();
                    var responseData = Mapper.Map<SoftBranch, SoftBranchViewModel>(newSoftBranch);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }
            }
            return response;
        }

        [Route("Update")]
        [Authorize(Roles = "BranchEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, SoftBranchViewModel softBranchViewModel)
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
                var softBranch = _softBranchRepository.GetSingleByCondition(x => x.Code == softBranchViewModel.Code && x.Id != softBranchViewModel.Id);
                if (softBranch != null)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Code đã được sử dụng!");
                }
                else
                {
                    var oldBranch = _softBranchRepository.GetSingleById(softBranchViewModel.Id);
                    oldBranch.UpdateSoftBranch(softBranchViewModel);
                    _softBranchRepository.Update(oldBranch);
                    _unitOfWork.Commit();
                    var responseData = Mapper.Map<SoftBranch, SoftBranchViewModel>(oldBranch);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

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
                var branches = _softBranchRepository.GetAll();
                if (order == "Name")
                    branches = branches.OrderBy(x => x.Name);
                else branches = branches.OrderBy(x => x.Id);
                var branchesVM = Mapper.Map<IEnumerable<SoftBranch>, IEnumerable<SoftBranchViewModel>>(branches);
                response = request.CreateResponse(HttpStatusCode.OK, branchesVM);
                return response;
            }
            catch (Exception ex)
            {

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }

        }

        [Route("authenadd")]
        [Authorize(Roles = "BranchAdd")]
        [HttpGet]
        public HttpResponseMessage Authen(HttpRequestMessage request)
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
        [Authorize(Roles = "BranchEdit")]
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
    }
}
