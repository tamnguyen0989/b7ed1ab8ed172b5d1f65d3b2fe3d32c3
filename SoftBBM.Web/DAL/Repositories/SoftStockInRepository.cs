using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftStockInRepository : IRepository<SoftStockIn>
    {
        IQueryable<SoftStockIn> GetAllPaging(int page, int pageSize, out int totalRow, out long totalMoney, BookFilterViewModel bookFilterVM);
    }
    public class SoftStockInRepository : RepositoryBase<SoftStockIn>, ISoftStockInRepository
    {
        public SoftStockInRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<SoftStockIn> GetAllPaging(int page, int pageSize, out int totalRow, out long totalMoney, BookFilterViewModel bookFilterVM)
        {
            var query = from d in DbContext.SoftStockIns
                        select d;

            if (bookFilterVM.branchId > 0)
                query = query.Where(x => x.BranchId == bookFilterVM.branchId);
            if (bookFilterVM.categoryId != null)
                query = query.Where(x => x.CategoryId == bookFilterVM.categoryId);
            if (bookFilterVM.fromBranchId > 0)
                query = query.Where(x => x.FromBranchId == bookFilterVM.fromBranchId);
            if (bookFilterVM.toBranchId > 0)
                query = query.Where(x => x.ToBranchId == bookFilterVM.toBranchId);
            if (bookFilterVM.isSupplierStockOut > 0)
            {
                query = query.Where(x => x.SupplierStatusId != "02" && x.FromBranchStatusId != "02");
            }
            if (bookFilterVM.isListStockout > 0)
            {
                query = query.Where(x => x.SupplierStatusId != "02");
            }
            if (bookFilterVM.isListStockin > 0)
            {
                query = query.Where(x => x.SupplierStatusId != "02" && x.FromBranchStatusId != "02" && x.FromBranchStatusId != "01" && x.FromBranchStatusId != "03");
            }
            IQueryable<SoftStockIn> SoftStockIns = null;
            //if (!string.IsNullOrEmpty(bookFilterVM.filter))
            //{
            //    SoftStockIns = query.Where(c => c.SoftSupplier.Name.ToLower().Contains(bookFilterVM.filter) || c.Id.ToString() == bookFilterVM.filter);
            //    totalRow = SoftStockIns.Count();
            //}
            //else
            {
                bool rootExist = false;
                DateTime init = new DateTime();
                IQueryable<SoftStockIn> SoftStockInsfilter = null;
                if (!string.IsNullOrEmpty(bookFilterVM.filter) || bookFilterVM.selectedBookStatusFilters.Count > 0 || bookFilterVM.selectedSupplierFilters.Count > 0 || bookFilterVM.selectedBranchFilters.Count > 0 || bookFilterVM.selectedPaymentStatusFilters.Count > 0 || (bookFilterVM.startDateFilter > init && bookFilterVM.endDateFilter > init) || bookFilterVM.selectedStockinCategoryFilters.Count > 0 || (bookFilterVM.startStockinDateFilter > init && bookFilterVM.endStockinDateFilter > init) || (bookFilterVM.startStockoutDateFilter > init && bookFilterVM.endStockoutDateFilter > init))
                {
                    if (!string.IsNullOrEmpty(bookFilterVM.filter))
                    {
                        if (rootExist == false)
                            SoftStockIns = query.Where(c => c.Id.ToString() == bookFilterVM.filter);
                        else
                            SoftStockIns = SoftStockIns.Where(c => c.Id.ToString() == bookFilterVM.filter);
                        if (rootExist == false) rootExist = true;
                    }
                    if (bookFilterVM.selectedSupplierFilters.Count > 0)
                    {
                        foreach (var item in bookFilterVM.selectedSupplierFilters)
                        {
                            IQueryable<SoftStockIn> SoftStockInstmp = null;
                            if (bookFilterVM.isListBook > 0 || bookFilterVM.isListStockin > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.FromSuppliers.Contains(item.Name));
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.FromSuppliers.Contains(item.Name));
                                if (SoftStockInsfilter == null)
                                    SoftStockInsfilter = SoftStockInstmp;
                                else
                                    SoftStockInsfilter = SoftStockInsfilter.Union(SoftStockInstmp);
                            }
                            else
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.SupplierId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.SupplierId == item.Id);
                                if (SoftStockInsfilter == null)
                                    SoftStockInsfilter = SoftStockInstmp;
                                else
                                    SoftStockInsfilter = SoftStockInsfilter.Union(SoftStockInstmp);
                            }
                        }
                        SoftStockIns = SoftStockInsfilter;
                        if (rootExist == false) rootExist = true;
                        SoftStockInsfilter = null;
                    }
                    if (bookFilterVM.selectedBookStatusFilters.Count > 0)
                    {
                        foreach (var item in bookFilterVM.selectedBookStatusFilters)
                        {
                            IQueryable<SoftStockIn> SoftStockInstmp = null;
                            if (bookFilterVM.isListStockin > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.ToBranchStatusId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.ToBranchStatusId == item.Id);
                            }
                            if (bookFilterVM.isListStockout > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.FromBranchStatusId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.FromBranchStatusId == item.Id);
                            }
                            if (bookFilterVM.isListBook > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.SupplierStatusId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.SupplierStatusId == item.Id);
                            }
                            if (bookFilterVM.isListBranchBook > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.SupplierStatusId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.SupplierStatusId == item.Id);
                            }
                            if (SoftStockInsfilter == null)
                                SoftStockInsfilter = SoftStockInstmp;
                            else
                                SoftStockInsfilter = SoftStockInsfilter.Union(SoftStockInstmp);
                        }
                        SoftStockIns = SoftStockInsfilter;
                        if (rootExist == false) rootExist = true;
                        SoftStockInsfilter = null;
                    }
                    if (bookFilterVM.selectedBranchFilters.Count > 0)
                    {
                        foreach (var item in bookFilterVM.selectedBranchFilters)
                        {
                            IQueryable<SoftStockIn> SoftStockInstmp = null;
                            if (bookFilterVM.isListStockin > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.FromBranchId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.FromBranchId == item.Id);
                            }
                            if (bookFilterVM.isListStockout > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.ToBranchId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.ToBranchId == item.Id);
                            }
                            if (bookFilterVM.isListBranchBook > 0)
                            {
                                if (rootExist == false)
                                    SoftStockInstmp = query.Where(x => x.FromBranchId == item.Id);
                                else
                                    SoftStockInstmp = SoftStockIns.Where(x => x.FromBranchId == item.Id);
                            }
                            if (SoftStockInsfilter == null)
                                SoftStockInsfilter = SoftStockInstmp;
                            else
                                SoftStockInsfilter = SoftStockInsfilter.Union(SoftStockInstmp);
                        }
                        SoftStockIns = SoftStockInsfilter;
                        if (rootExist == false) rootExist = true;
                        SoftStockInsfilter = null;
                    }
                    if (bookFilterVM.selectedStockinCategoryFilters.Count > 0)
                    {
                        foreach (var item in bookFilterVM.selectedStockinCategoryFilters)
                        {
                            IQueryable<SoftStockIn> SoftStockInstmp = null;
                            if (rootExist == false)
                                SoftStockInstmp = query.Where(x => x.CategoryId == item.Id);
                            else
                                SoftStockInstmp = SoftStockIns.Where(x => x.CategoryId == item.Id);
                            if (SoftStockInsfilter == null)
                                SoftStockInsfilter = SoftStockInstmp;
                            else
                                SoftStockInsfilter = SoftStockInsfilter.Union(SoftStockInstmp);
                        }
                        SoftStockIns = SoftStockInsfilter;
                        if (rootExist == false) rootExist = true;
                        SoftStockInsfilter = null;
                    }
                    if (bookFilterVM.selectedPaymentStatusFilters.Count > 0)
                    {
                        foreach (var item in bookFilterVM.selectedPaymentStatusFilters)
                        {
                            IQueryable<SoftStockIn> SoftStockInstmp = null;
                            if (rootExist == false)
                                SoftStockInstmp = query.Where(x => x.PaymentStatusId == item.Id);
                            else
                                SoftStockInstmp = SoftStockIns.Where(x => x.PaymentStatusId == item.Id);
                            if (SoftStockInsfilter == null)
                                SoftStockInsfilter = SoftStockInstmp;
                            else
                                SoftStockInsfilter = SoftStockInsfilter.Union(SoftStockInstmp);
                        }
                        SoftStockIns = SoftStockInsfilter;
                        if (rootExist == false) rootExist = true;
                        SoftStockInsfilter = null;
                    }
                    if (bookFilterVM.startDateFilter > init && bookFilterVM.endDateFilter > init)
                    {
                        bookFilterVM.startDateFilter = UtilExtensions.ConvertStartDate(bookFilterVM.startDateFilter.ToLocalTime());
                        bookFilterVM.endDateFilter = UtilExtensions.ConvertEndDate(bookFilterVM.endDateFilter.ToLocalTime());
                        IQueryable<SoftStockIn> SoftStockInstmp = null;
                        if (rootExist == false)
                            SoftStockInstmp = query.Where(x => x.CreatedDate >= bookFilterVM.startDateFilter && x.CreatedDate <= bookFilterVM.endDateFilter);
                        else
                            SoftStockInstmp = SoftStockIns.Where(x => x.CreatedDate >= bookFilterVM.startDateFilter && x.CreatedDate <= bookFilterVM.endDateFilter);
                        SoftStockIns = SoftStockInstmp;
                        if (rootExist == false) rootExist = true;
                    }
                    if (bookFilterVM.startStockinDateFilter > init && bookFilterVM.endStockinDateFilter > init)
                    {
                        bookFilterVM.startStockinDateFilter = UtilExtensions.ConvertStartDate(bookFilterVM.startStockinDateFilter.ToLocalTime());
                        bookFilterVM.endStockinDateFilter = UtilExtensions.ConvertEndDate(bookFilterVM.endStockinDateFilter.ToLocalTime());
                        IQueryable<SoftStockIn> SoftStockInstmp = null;
                        if (rootExist == false)
                            SoftStockInstmp = query.Where(x => x.StockInDate >= bookFilterVM.startStockinDateFilter && x.StockInDate <= bookFilterVM.endStockinDateFilter);
                        else
                            SoftStockInstmp = SoftStockIns.Where(x => x.StockInDate >= bookFilterVM.startStockinDateFilter && x.StockInDate <= bookFilterVM.endStockinDateFilter);
                        SoftStockIns = SoftStockInstmp;
                        if (rootExist == false) rootExist = true;
                    }
                    if (bookFilterVM.startStockoutDateFilter > init && bookFilterVM.endStockoutDateFilter > init)
                    {
                        bookFilterVM.startStockoutDateFilter = UtilExtensions.ConvertStartDate(bookFilterVM.startStockoutDateFilter.ToLocalTime());
                        bookFilterVM.endStockoutDateFilter = UtilExtensions.ConvertEndDate(bookFilterVM.endStockoutDateFilter.ToLocalTime());
                        IQueryable<SoftStockIn> SoftStockInstmp = null;
                        if (rootExist == false)
                            SoftStockInstmp = query.Where(x => x.StockOutDate >= bookFilterVM.startStockoutDateFilter && x.StockOutDate <= bookFilterVM.endStockoutDateFilter);
                        else
                            SoftStockInstmp = SoftStockIns.Where(x => x.StockOutDate >= bookFilterVM.startStockoutDateFilter && x.StockOutDate <= bookFilterVM.endStockoutDateFilter);
                        SoftStockIns = SoftStockInstmp;
                        if (rootExist == false) rootExist = true;
                    }
                }
                else
                {
                    SoftStockIns = query;
                }
                totalRow = SoftStockIns.Count();
                if (SoftStockIns.Sum(x => x.Total) == null)
                {
                    totalMoney = 0;
                }
                else
                    totalMoney = SoftStockIns.Sum(x => x.Total).Value;
            }

            switch (bookFilterVM.sortBy)
            {
                case "Total_des":
                    SoftStockIns = SoftStockIns.OrderByDescending(x => x.Total);
                    break;
                case "Total_asc":
                    SoftStockIns = SoftStockIns.OrderBy(x => x.Total);
                    break;
                case "CreatedDate_des":
                    SoftStockIns = SoftStockIns.OrderByDescending(x => x.CreatedDate);
                    break;
                case "CreatedDate_asc":
                    SoftStockIns = SoftStockIns.OrderBy(x => x.CreatedDate);
                    break;
                default:
                    SoftStockIns = SoftStockIns.OrderByDescending(x => x.Id);
                    break;
            }

            return SoftStockIns.Skip(page * pageSize).Take(pageSize);
        }
    }
}