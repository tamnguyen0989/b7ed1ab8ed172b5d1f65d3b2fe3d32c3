﻿using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System.Linq;
using System;
using SoftBBM.Web.ViewModels;
using System.Collections.Generic;
using SoftBBM.Web.Infrastructure.Extensions;
using System.Data.SqlClient;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IdonhangRepository : IRepository<donhang>
    {
        IQueryable<donhang> GetAllPaging(int page, int pageSize, out int totalRow, out long totalMoney, OrderFilterViewModel orderFilterVM);
        IEnumerable<ChannelSalesRevenuesReportViewModel> GetChannelSalesReport(string fromDate, string toDate,int branchId);
        IEnumerable<ChannelSalesRevenuesReportViewModel> GetChannelRevenuesReport(string fromDate, string toDate, int branchId);
        IEnumerable<ChannelSalesRevenuesReportViewModel> GetSalesReport(string fromDate, string toDate, int branchId, int channelId);
        IEnumerable<ChannelSalesRevenuesReportViewModel> GetRevenuesReport(string fromDate, string toDate, int branchId, int channelId);
        IEnumerable<ChannelSalesRevenuesReportViewModel> GetSalesReportMonth(string fromDate, string toDate, int branchId, int channelId);
        IEnumerable<ChannelSalesRevenuesReportViewModel> GetSalesReportYear(string fromDate, string toDate, int branchId, int channelId);
        IQueryable<donhang> GetAllPagingFilter(OrderFilterViewModel orderFilterVM);
    }

    public class donhangRepository : RepositoryBase<donhang>, IdonhangRepository
    {
        public donhangRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<donhang> GetAllPaging(int page, int pageSize, out int totalRow, out long totalMoney, OrderFilterViewModel orderFilterVM)
        {
            var query = from d in DbContext.donhangs
                        where d.BranchId == orderFilterVM.branchId
                        select d;
            if (orderFilterVM.channelId != null)
                query = query.Where(x => x.ChannelId == orderFilterVM.channelId);
            IQueryable<donhang> donhangs = null;
            {
                bool rootExist = false;
                DateTime init = new DateTime();
                IQueryable<donhang> donhangsfilter = null;
                if (!string.IsNullOrEmpty(orderFilterVM.filter) || orderFilterVM.selectedOrderStatusFilters.Count > 0 || orderFilterVM.selectedSellerFilters.Count > 0 || orderFilterVM.selectedShipperFilters.Count > 0 || (orderFilterVM.startDateFilter > init && orderFilterVM.endDateFilter > init) || orderFilterVM.selectedEcommerceShipperFilters.Count > 0 || orderFilterVM.selectedPaymentFilters.Count > 0 || orderFilterVM.selectedUltilFilters.Count > 0)
                {
                    if (!string.IsNullOrEmpty(orderFilterVM.filter))
                    {
                        if (rootExist == false)
                            donhangs = query.Where(c => c.khachhang.hoten.ToLower().Contains(orderFilterVM.filter) || c.id.ToString() == orderFilterVM.filter || c.khachhang.dienthoai.Contains(orderFilterVM.filter) || c.khachhang.hoten.Contains(orderFilterVM.filter) || c.khachhang.duong.Contains(orderFilterVM.filter) || c.ghichu.Contains(orderFilterVM.filter) || c.OrderIdShopeeApi.Contains(orderFilterVM.filter) || c.TrackingNo.Contains(orderFilterVM.filter) || c.ShipperNameShopeeApi.Contains(orderFilterVM.filter));
                        else
                            donhangs = donhangs.Where(c => c.khachhang.hoten.ToLower().Contains(orderFilterVM.filter) || c.id.ToString() == orderFilterVM.filter || c.khachhang.dienthoai.Contains(orderFilterVM.filter) || c.khachhang.hoten.Contains(orderFilterVM.filter) || c.khachhang.duong.Contains(orderFilterVM.filter) || c.ghichu.Contains(orderFilterVM.filter) || c.OrderIdShopeeApi.Contains(orderFilterVM.filter) || c.TrackingNo.Contains(orderFilterVM.filter) || c.ShipperNameShopeeApi.Contains(orderFilterVM.filter));
                        if (rootExist == false) rootExist = true;

                    }
                    if (orderFilterVM.selectedOrderStatusFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedOrderStatusFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.Status == item);
                            else
                                donhangstmp = donhangs.Where(x => x.Status == item);
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.selectedSellerFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedSellerFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.CreatedBy == item);
                            else
                                donhangstmp = donhangs.Where(x => x.CreatedBy == item);
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.selectedShipperFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedShipperFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.ShipperId == item);
                            else
                                donhangstmp = donhangs.Where(x => x.ShipperId == item);
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.startDateFilter > init && orderFilterVM.endDateFilter > init)
                    {
                        orderFilterVM.startDateFilter = UtilExtensions.ConvertStartDate(orderFilterVM.startDateFilter.ToLocalTime());
                        orderFilterVM.endDateFilter = UtilExtensions.ConvertEndDate(orderFilterVM.endDateFilter.ToLocalTime());
                        IQueryable<donhang> donhangstmp = null;
                        if (rootExist == false)
                            donhangstmp = query.Where(x => x.CreatedDate >= orderFilterVM.startDateFilter && x.CreatedDate <= orderFilterVM.endDateFilter);
                        else
                            donhangstmp = donhangs.Where(x => x.CreatedDate >= orderFilterVM.startDateFilter && x.CreatedDate <= orderFilterVM.endDateFilter);
                        donhangs = donhangstmp;
                        if (rootExist == false) rootExist = true;
                    }
                    if (orderFilterVM.selectedEcommerceShipperFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedEcommerceShipperFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.ShipperNameShopeeApi.ToLower() == item.ToLower());
                            else
                                donhangstmp = donhangs.Where(x => x.ShipperNameShopeeApi.ToLower() == item.ToLower());
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.selectedPaymentFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedPaymentFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.pttt == item);
                            else
                                donhangstmp = donhangs.Where(x => x.pttt == item);
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.selectedUltilFilters.Count > 0)
                    {
                        foreach (var selectedUltilFilter in orderFilterVM.selectedUltilFilters)
                        {
                            switch (selectedUltilFilter)
                            {
                                case "NoTrackingNumber":
                                    if (rootExist == false)
                                        donhangs = query.Where(x => string.IsNullOrEmpty(x.TrackingNo));
                                    else
                                        donhangs = donhangs.Where(x => string.IsNullOrEmpty(x.TrackingNo));
                                    if (rootExist == false) rootExist = true;
                                    break;
                                case "NoPrinting":
                                    if (rootExist == false)
                                        donhangs = query.Where(x => string.IsNullOrEmpty(x.StatusPrint));
                                    else
                                        donhangs = donhangs.Where(x => string.IsNullOrEmpty(x.StatusPrint));
                                    if (rootExist == false) rootExist = true;
                                    break;
                                case "HaveTrackingNumber":
                                    if (rootExist == false)
                                        donhangs = query.Where(x => !string.IsNullOrEmpty(x.TrackingNo));
                                    else
                                        donhangs = donhangs.Where(x => !string.IsNullOrEmpty(x.TrackingNo));
                                    if (rootExist == false) rootExist = true;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    donhangs = query;
                }
                totalRow = donhangs.Count();
                if (donhangs.Sum(x => x.tongtien) == null)
                {
                    totalMoney = 0;
                }
                else
                    totalMoney = donhangs.Sum(x => x.tongtien).Value;
            }

            switch (orderFilterVM.sortBy)
            {
                case "tongtien_des":
                    donhangs = donhangs.OrderByDescending(x => x.tongtien);
                    break;
                case "tongtien_asc":
                    donhangs = donhangs.OrderBy(x => x.tongtien);
                    break;
                case "CreatedDate_des":
                    donhangs = donhangs.OrderByDescending(x => x.CreatedDate);
                    break;
                case "CreatedDate_asc":
                    donhangs = donhangs.OrderBy(x => x.CreatedDate);
                    break;
                default:
                    donhangs = donhangs.OrderByDescending(x => x.CreatedDate);
                    break;
            }

            return donhangs.Skip(page * pageSize).Take(pageSize);
        }

        public IQueryable<donhang> GetAllPagingFilter(OrderFilterViewModel orderFilterVM)
        {
            var query = from d in DbContext.donhangs
                        where d.BranchId == orderFilterVM.branchId
                        select d;
            if (orderFilterVM.channelId != null)
                query = query.Where(x => x.ChannelId == orderFilterVM.channelId);
            IQueryable<donhang> donhangs = null;
            {
                bool rootExist = false;
                DateTime init = new DateTime();
                IQueryable<donhang> donhangsfilter = null;
                if (!string.IsNullOrEmpty(orderFilterVM.filter) || orderFilterVM.selectedOrderStatusFilters.Count > 0 || orderFilterVM.selectedSellerFilters.Count > 0 || orderFilterVM.selectedShipperFilters.Count > 0 || (orderFilterVM.startDateFilter > init && orderFilterVM.endDateFilter > init))
                {
                    if (!string.IsNullOrEmpty(orderFilterVM.filter))
                    {
                        if (rootExist == false)
                            donhangs = query.Where(c => c.khachhang.hoten.ToLower().Contains(orderFilterVM.filter) || c.id.ToString() == orderFilterVM.filter || c.khachhang.dienthoai.Contains(orderFilterVM.filter) || c.khachhang.hoten.Contains(orderFilterVM.filter) || c.khachhang.duong.Contains(orderFilterVM.filter) || c.ghichu.Contains(orderFilterVM.filter));
                        else
                            donhangs = donhangs.Where(c => c.khachhang.hoten.ToLower().Contains(orderFilterVM.filter) || c.id.ToString() == orderFilterVM.filter || c.khachhang.dienthoai.Contains(orderFilterVM.filter) || c.khachhang.hoten.Contains(orderFilterVM.filter) || c.khachhang.duong.Contains(orderFilterVM.filter) || c.ghichu.Contains(orderFilterVM.filter));
                    }
                    if (orderFilterVM.selectedOrderStatusFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedOrderStatusFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.Status == item);
                            else
                                donhangstmp = donhangs.Where(x => x.Status == item);
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.selectedSellerFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedSellerFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.CreatedBy == item);
                            else
                                donhangstmp = donhangs.Where(x => x.CreatedBy == item);
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.selectedShipperFilters.Count > 0)
                    {
                        foreach (var item in orderFilterVM.selectedShipperFilters)
                        {
                            IQueryable<donhang> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.ShipperId == item);
                            else
                                donhangstmp = donhangs.Where(x => x.ShipperId == item);
                            if (donhangsfilter == null)
                                donhangsfilter = donhangstmp;
                            else
                                donhangsfilter = donhangsfilter.Union(donhangstmp);
                        }
                        donhangs = donhangsfilter;
                        if (rootExist == false) rootExist = true;
                        donhangsfilter = null;
                    }
                    if (orderFilterVM.startDateFilter > init && orderFilterVM.endDateFilter > init)
                    {
                        orderFilterVM.startDateFilter = UtilExtensions.ConvertStartDate(orderFilterVM.startDateFilter.ToLocalTime());
                        orderFilterVM.endDateFilter = UtilExtensions.ConvertEndDate(orderFilterVM.endDateFilter.ToLocalTime());
                        IQueryable<donhang> donhangstmp = null;
                        if (rootExist == false)
                            donhangstmp = query.Where(x => x.CreatedDate >= orderFilterVM.startDateFilter && x.CreatedDate <= orderFilterVM.endDateFilter);
                        else
                            donhangstmp = donhangs.Where(x => x.CreatedDate >= orderFilterVM.startDateFilter && x.CreatedDate <= orderFilterVM.endDateFilter);
                        donhangs = donhangstmp;
                        if (rootExist == false) rootExist = true;
                    }
                }
                else
                {
                    donhangs = query;
                }
            }

            switch (orderFilterVM.sortBy)
            {
                case "tongtien_des":
                    donhangs = donhangs.OrderByDescending(x => x.tongtien);
                    break;
                case "tongtien_asc":
                    donhangs = donhangs.OrderBy(x => x.tongtien);
                    break;
                case "CreatedDate_des":
                    donhangs = donhangs.OrderByDescending(x => x.CreatedDate);
                    break;
                case "CreatedDate_asc":
                    donhangs = donhangs.OrderBy(x => x.CreatedDate);
                    break;
                default:
                    donhangs = donhangs.OrderByDescending(x => x.id);
                    break;
            }

            return donhangs;
        }

        public IEnumerable<ChannelSalesRevenuesReportViewModel> GetChannelSalesReport(string fromDate, string toDate, int branchId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromDate),
                new SqlParameter("@toDate",toDate),
                new SqlParameter("@branchId",branchId)
            };
            return DbContext.Database.SqlQuery<ChannelSalesRevenuesReportViewModel>("GetChannelSalesReport @fromDate,@toDate,@branchId", parameters);
        }
        public IEnumerable<ChannelSalesRevenuesReportViewModel> GetChannelRevenuesReport(string fromDate, string toDate, int branchId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromDate),
                new SqlParameter("@toDate",toDate),
                new SqlParameter("@branchId",branchId)
            };
            return DbContext.Database.SqlQuery<ChannelSalesRevenuesReportViewModel>("GetChannelRevenuesReport @fromDate,@toDate,@branchId", parameters);
        }
        public IEnumerable<ChannelSalesRevenuesReportViewModel> GetSalesReport(string fromDate, string toDate, int branchId,int channelId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromDate),
                new SqlParameter("@toDate",toDate),
                new SqlParameter("@branchId",branchId),
                new SqlParameter("@channelId",channelId)
            };
            return DbContext.Database.SqlQuery<ChannelSalesRevenuesReportViewModel>("GetSalesReport @fromDate,@toDate,@branchId,@channelId", parameters);
        }
        public IEnumerable<ChannelSalesRevenuesReportViewModel> GetRevenuesReport(string fromDate, string toDate, int branchId, int channelId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromDate),
                new SqlParameter("@toDate",toDate),
                new SqlParameter("@branchId",branchId),
                new SqlParameter("@channelId",channelId)
            };
            return DbContext.Database.SqlQuery<ChannelSalesRevenuesReportViewModel>("GetRevenuesReport @fromDate,@toDate,@branchId,@channelId", parameters);
        }
        public IEnumerable<ChannelSalesRevenuesReportViewModel> GetSalesReportMonth(string fromDate, string toDate, int branchId, int channelId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromDate),
                new SqlParameter("@toDate",toDate),
                new SqlParameter("@branchId",branchId),
                new SqlParameter("@channelId",channelId)
            };
            return DbContext.Database.SqlQuery<ChannelSalesRevenuesReportViewModel>("GetSalesReportMonth @fromDate,@toDate,@branchId,@channelId", parameters);
        }
        public IEnumerable<ChannelSalesRevenuesReportViewModel> GetSalesReportYear(string fromDate, string toDate, int branchId, int channelId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromDate),
                new SqlParameter("@toDate",toDate),
                new SqlParameter("@branchId",branchId),
                new SqlParameter("@channelId",channelId)
            };
            return DbContext.Database.SqlQuery<ChannelSalesRevenuesReportViewModel>("GetSalesReportYear @fromDate,@toDate,@branchId,@channelId", parameters);
        }
    }
}