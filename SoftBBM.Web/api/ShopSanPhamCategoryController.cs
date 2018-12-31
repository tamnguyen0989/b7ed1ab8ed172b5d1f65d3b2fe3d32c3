using AutoMapper;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Infrastructure.Core;
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
    [RoutePrefix("api/productcategory")]
    public class ShopSanPhamCategoryController : ApiController
    {
        IShopSanPhamCategoryRepository _shopSanPhamCategoryRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        IUnitOfWork _unitOfWork;

        public ShopSanPhamCategoryController(IShopSanPhamCategoryRepository shopSanPhamCategoryRepository, IUnitOfWork unitOfWork, IShopSanPhamRepository shopSanPhamRepository)
        {
            _shopSanPhamCategoryRepository = shopSanPhamCategoryRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "ProductCategoryList")]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;
            HttpResponseMessage response = null;
            List<shop_sanphamCategories> productCategories = null;
            int totalProductCategories = new int();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.Trim().ToLower();

                productCategories = _shopSanPhamCategoryRepository.GetMulti(c => c.Name.ToLower().Contains(filter))
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalProductCategories = _shopSanPhamCategoryRepository.GetMulti(c => c.Name.ToLower().Contains(filter))
                    .Count();
            }
            else
            {
                productCategories = _shopSanPhamCategoryRepository.GetAll()
                    .OrderByDescending(c => c.Id)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                .ToList();

                totalProductCategories = _shopSanPhamCategoryRepository.GetAll().Count();
            }

            IEnumerable<ShopSanPhamCategoryViewModel> productCategoriesVM = Mapper.Map<IEnumerable<shop_sanphamCategories>, IEnumerable<ShopSanPhamCategoryViewModel>>(productCategories);

            PaginationSet<ShopSanPhamCategoryViewModel> pagedSet = new PaginationSet<ShopSanPhamCategoryViewModel>()
            {
                Page = currentPage,
                TotalCount = totalProductCategories,
                TotalPages = (int)Math.Ceiling((decimal)totalProductCategories / currentPageSize),
                Items = productCategoriesVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("add")]
        [Authorize(Roles = "ProductCategoryAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, ShopSanPhamCategoryViewModel shopSanPhamCategoryVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var shopSanPhamCategory = new shop_sanphamCategories();
                shopSanPhamCategory.Name = shopSanPhamCategoryVM.Name;
                shopSanPhamCategory.CreatedDate = DateTime.Now;
                shopSanPhamCategory.NotDiscountMember = shopSanPhamCategoryVM.NotDiscountMember;
                shopSanPhamCategory.CreatedBy = shopSanPhamCategoryVM.CreatedBy;
                _shopSanPhamCategoryRepository.Add(shopSanPhamCategory);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.Created, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("update")]
        [Authorize(Roles = "ProductCategoryEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, ShopSanPhamCategoryViewModel shopSanPhamCategoryVM)
        {
            HttpResponseMessage response = null;
            try
            {
                var oldProductCategory = _shopSanPhamCategoryRepository.GetSingleById(shopSanPhamCategoryVM.Id);

                if (oldProductCategory.NotDiscountMember == false && shopSanPhamCategoryVM.NotDiscountMember == true)
                {
                    var products = _shopSanPhamRepository.GetMulti(x => x.CategoryId == shopSanPhamCategoryVM.Id);
                    foreach (var item in products)
                    {
                        if (item.NotDiscountMember != true)
                            item.NotDiscountMember = true;
                        _shopSanPhamRepository.Update(item);
                    }
                }
                if (oldProductCategory.NotDiscountMember == true && shopSanPhamCategoryVM.NotDiscountMember == false)
                {
                    var products = _shopSanPhamRepository.GetMulti(x => x.CategoryId == shopSanPhamCategoryVM.Id);
                    foreach (var item in products)
                    {
                        if (item.NotDiscountMember != false)
                            item.NotDiscountMember = false;
                        _shopSanPhamRepository.Update(item);
                    }
                }

                oldProductCategory.Name = shopSanPhamCategoryVM.Name;
                oldProductCategory.Description = shopSanPhamCategoryVM.Description;
                oldProductCategory.UpdatedDate = DateTime.Now;
                oldProductCategory.UpdatedBy = shopSanPhamCategoryVM.UpdatedBy;
                oldProductCategory.NotDiscountMember = shopSanPhamCategoryVM.NotDiscountMember;
                _shopSanPhamCategoryRepository.Update(oldProductCategory);
                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.Created, true);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return response;
        }

        [Route("authenadd")]
        [Authorize(Roles = "ProductCategoryAdd")]
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
        [Authorize(Roles = "ProductCategoryEdit")]
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

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;

            var shopSanPhamCategories = _shopSanPhamCategoryRepository.GetAll().OrderBy(x => x.Name);
            var responseData = Mapper.Map<IEnumerable<shop_sanphamCategories>, IEnumerable<ShopSanPhamCategoryViewModel>>(shopSanPhamCategories);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        [Route("detail/{id}")]
        [Authorize(Roles = "ProductCategoryEdit")]
        [HttpGet]
        public HttpResponseMessage Detail(HttpRequestMessage request, int id)
        {
            HttpResponseMessage response = null;
            try
            {
                var productCategoryCn = _shopSanPhamCategoryRepository.GetSingleById(id);
                var productCategoryVm = Mapper.Map<shop_sanphamCategories, ShopSanPhamCategoryViewModel>(productCategoryCn);
                response = request.CreateResponse(HttpStatusCode.OK, productCategoryVm);
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
