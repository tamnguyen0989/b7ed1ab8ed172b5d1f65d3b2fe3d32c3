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
    public interface ISoftReturnSupplierRepository : IRepository<SoftReturnSupplier>
    {
        IQueryable<SoftReturnSupplier> GetAllPaging(int page, int pageSize, out int totalRow, SoftReturnSupplierFilterViewModel FilterVm);
    }
    public class SoftReturnSupplierRepository : RepositoryBase<SoftReturnSupplier>, ISoftReturnSupplierRepository
    {
        public SoftReturnSupplierRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<SoftReturnSupplier> GetAllPaging(int page, int pageSize, out int totalRow, SoftReturnSupplierFilterViewModel FilterVm)

        {
            var query = from d in DbContext.SoftReturnSuppliers
                        select d;
            //if (FilterVm.branchId > 0)
            //    query = query.Where(x => x.BranchId == FilterVm.branchId);
            if (FilterVm.supplierId > 0)
                query = query.Where(x => x.SupplierId == FilterVm.supplierId);
            //if (!string.IsNullOrEmpty(FilterVm.filter))
            //{
            //    query = query.Where(c => c.Id.ToString().Contains(FilterVm.filter) || c.ApplicationUser.FullName.ToLower().Contains(FilterVm.filter) || c.ApplicationUser.UserName.ToLower().Contains(FilterVm.filter) || c.SoftSupplier.Name.ToLower().Contains(FilterVm.filter));
            //}
            bool rootExist = false;
            DateTime init = new DateTime();
            IQueryable<SoftReturnSupplier> softReturnSuppliers = null;
            IQueryable<SoftReturnSupplier> softReturnSuppliersfilter = null;
            if (!string.IsNullOrEmpty(FilterVm.filter) || FilterVm.selectedSupplierFilters.Count > 0 || FilterVm.startDateFilter > init && FilterVm.endDateFilter > init)
            {
                if (!string.IsNullOrEmpty(FilterVm.filter))
                {
                    if (rootExist == false)
                        softReturnSuppliers = query.Where(c => c.Id.ToString() == FilterVm.filter);
                    else
                        softReturnSuppliers = softReturnSuppliers.Where(c => c.Id.ToString() == FilterVm.filter);
                    if (rootExist == false) rootExist = true;
                }

                if (FilterVm.selectedSupplierFilters.Count > 0)
                {
                    foreach (var item in FilterVm.selectedSupplierFilters)
                    {
                        IQueryable<SoftReturnSupplier> softReturnSupplierTmp = null;
                        if (rootExist == false)
                            softReturnSupplierTmp = query.Where(x => x.SupplierId == item.Id);
                        else
                            softReturnSupplierTmp = softReturnSuppliers.Where(x => x.SupplierId == item.Id);
                        if (softReturnSuppliersfilter == null)
                            softReturnSuppliersfilter = softReturnSupplierTmp;
                        else
                            softReturnSuppliersfilter = softReturnSuppliersfilter.Union(softReturnSupplierTmp);
                    }
                    softReturnSuppliers = softReturnSuppliersfilter;
                    if (rootExist == false) rootExist = true;
                    softReturnSuppliersfilter = null;
                }

                if (FilterVm.startDateFilter > init && FilterVm.endDateFilter > init)
                {
                    //if (FilterVm.startDateFilter > init && FilterVm.endDateFilter > init)
                    //{
                    //    FilterVm.startDateFilter = UtilExtensions.ConvertStartDate(FilterVm.startDateFilter.ToLocalTime());
                    //    FilterVm.endDateFilter = UtilExtensions.ConvertEndDate(FilterVm.endDateFilter.ToLocalTime());
                    //    query = query.Where(x => x.CreatedDate >= FilterVm.startDateFilter && x.CreatedDate <= FilterVm.endDateFilter);
                    //}

                    FilterVm.startDateFilter = UtilExtensions.ConvertStartDate(FilterVm.startDateFilter.ToLocalTime());
                    FilterVm.endDateFilter = UtilExtensions.ConvertEndDate(FilterVm.endDateFilter.ToLocalTime());
                    IQueryable<SoftReturnSupplier> softReturnSupplierTmp = null;
                    if (rootExist == false)
                        softReturnSupplierTmp = query.Where(x => x.CreatedDate >= FilterVm.startDateFilter && x.CreatedDate <= FilterVm.endDateFilter);
                    else
                        softReturnSupplierTmp = softReturnSuppliers.Where(x => x.CreatedDate >= FilterVm.startDateFilter && x.CreatedDate <= FilterVm.endDateFilter);
                    softReturnSuppliers = softReturnSupplierTmp;
                    if (rootExist == false) rootExist = true;
                }
            }
            else
                softReturnSuppliers = query;
            totalRow = softReturnSuppliers.Count();

            switch (FilterVm.sortBy)
            {
                case "CreatedDate_des":
                    softReturnSuppliers = softReturnSuppliers.OrderByDescending(x => x.CreatedDate);
                    break;
                case "CreatedDate_asc":
                    softReturnSuppliers = softReturnSuppliers.OrderBy(x => x.CreatedDate);
                    break;
                default:
                    softReturnSuppliers = softReturnSuppliers.OrderByDescending(x => x.Id);
                    break;
            }

            return softReturnSuppliers.Skip(page * pageSize).Take(pageSize);
        }
    }
}