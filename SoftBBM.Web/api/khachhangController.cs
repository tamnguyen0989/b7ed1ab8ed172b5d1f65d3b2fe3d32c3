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
    [RoutePrefix("api/customer")]
    public class khachhangController : ApiController
    {
        IkhachhangRepository _khachhangRepository;
        IdonhangchuyenphattpRepository _donhangchuyenphattpRepository;
        IdonhangchuyenphattinhRepository _donhangchuyenphattinhRepository;
        IUnitOfWork _unitOfWork;
        public khachhangController(IUnitOfWork unitOfWork, IkhachhangRepository khachhangRepository, IdonhangchuyenphattpRepository donhangchuyenphattpRepository, IdonhangchuyenphattinhRepository donhangchuyenphattinhRepository)
        {
            _unitOfWork = unitOfWork;
            _khachhangRepository = khachhangRepository;
            _donhangchuyenphattpRepository = donhangchuyenphattpRepository;
            _donhangchuyenphattinhRepository = donhangchuyenphattinhRepository;
        }

        [Route("getbyphone")]
        [HttpGet]
        public HttpResponseMessage GetByPhone(HttpRequestMessage request, string phone)
        {
            HttpResponseMessage response = null;

            var customer = _khachhangRepository.GetSingleByCondition(x => x.dienthoai == phone);
            var responseData = Mapper.Map<khachhang, khachhangViewModel>(customer);
            response = request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        [HttpPost]
        [Authorize(Roles = "CustomerList")]
        [Route("search")]
        public HttpResponseMessage Search(HttpRequestMessage request, CustomerFilterViewModel customerFilterVM)
        {
            int currentPage = customerFilterVM.page.Value;
            int currentPageSize = customerFilterVM.pageSize.Value;
            HttpResponseMessage response = null;
            IEnumerable<khachhang> khachhangs = null;
            int totalkhachhangs = 0;

            khachhangs = _khachhangRepository.GetAllPaging(currentPage, currentPageSize, out totalkhachhangs, customerFilterVM).ToList();

            IEnumerable<khachhangViewModel> khachhangsVM = Mapper.Map<IEnumerable<khachhang>, IEnumerable<khachhangViewModel>>(khachhangs);

            foreach (var item in khachhangsVM)
            {
                if (item.idtp > 0)
                {
                    var city = _donhangchuyenphattpRepository.GetSingleById(item.idtp.Value);
                    item.City = Mapper.Map<donhang_chuyenphat_tp, donhangchuyenphattpViewModel>(city);
                }
                if (item.idquan > 0)
                {
                    var district = _donhangchuyenphattinhRepository.GetSingleById(item.idquan.Value);
                    item.District = Mapper.Map<donhang_chuyenphat_tinh, donhangchuyenphattinhViewModel>(district);
                }
            }

            PaginationSet<khachhangViewModel> pagedSet = new PaginationSet<khachhangViewModel>()
            {
                Page = currentPage,
                TotalCount = totalkhachhangs,
                TotalPages = (int)Math.Ceiling((decimal)totalkhachhangs / currentPageSize),
                Items = khachhangsVM
            };

            response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
            return response;
        }

        [Route("update")]
        [Authorize(Roles = "CustomerEdit")]
        [HttpPost]
        public HttpResponseMessage Update(HttpRequestMessage request, khachhangViewModel khachhangVM)
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
                var oldCustomer = _khachhangRepository.GetSingleById(khachhangVM.MaKH);
                oldCustomer.dienthoai = khachhangVM.dienthoai;
                oldCustomer.duong = khachhangVM.duong;
                oldCustomer.email = khachhangVM.email;
                oldCustomer.hoten = khachhangVM.hoten;
                oldCustomer.idquan = khachhangVM.District.id;
                oldCustomer.idtp = khachhangVM.City.id;
                oldCustomer.UpdatedBy = khachhangVM.userId;
                oldCustomer.UpdatedDate = DateTime.Now;
                _khachhangRepository.Update(oldCustomer);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, true);
            }
            return response;
        }

        [Route("add")]
        [Authorize(Roles = "CustomerAdd")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, khachhangViewModel khachhangVM)
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
                var existCus = _khachhangRepository.GetSingleByCondition(x => x.tendn == khachhangVM.tendn);
                if (existCus != null)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Lỗi! Tên đăng nhập đã tồn tại!");
                    return response;
                }
                var cus = new khachhang();
                cus.diem = "0";
                cus.dienthoai = khachhangVM.dienthoai;
                cus.duong = khachhangVM.duong;
                cus.email = khachhangVM.email;
                cus.hoten = khachhangVM.hoten;
                cus.idquan = khachhangVM.District.id;
                cus.idtp = khachhangVM.City.id;
                cus.matkhau = "YmFieW1hcnQudm4=";
                cus.tendn = khachhangVM.tendn;
                cus.CreatedBy = khachhangVM.userId;
                cus.CreatedDate = DateTime.Now;
                _khachhangRepository.Add(cus);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.Created, true);
            }
            return response;
        }

        [Route("authenadd")]
        [Authorize(Roles = "CustomerAdd")]
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
        [Authorize(Roles = "CustomerEdit")]
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