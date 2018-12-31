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
    public interface ISoftAdjustmentStockRepository : IRepository<SoftAdjustmentStock>
    {
        IQueryable<SoftAdjustmentStock> GetAllPaging(int page, int pageSize, out int totalRow, AdjustmentStockFilterViewModel adjustmentStockFilter);
    }
    public class SoftAdjustmentStockRepository : RepositoryBase<SoftAdjustmentStock>, ISoftAdjustmentStockRepository
    {
        public SoftAdjustmentStockRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<SoftAdjustmentStock> GetAllPaging(int page, int pageSize, out int totalRow, AdjustmentStockFilterViewModel adjustmentStockFilter)

        {
            IQueryable<SoftAdjustmentStock> softAdjustmentStocks = null;
            var query = from d in DbContext.SoftAdjustmentStocks
                        select d;

            if (adjustmentStockFilter.branchId > 0)
                query = query.Where(x => x.BranchId == adjustmentStockFilter.branchId);
            if (!string.IsNullOrEmpty(adjustmentStockFilter.filter))
            {
                query = query.Where(c => c.Id.ToString().Contains(adjustmentStockFilter.filter) || c.ApplicationUser.FullName.ToLower().Contains(adjustmentStockFilter.filter) || c.ApplicationUser.UserName.ToLower().Contains(adjustmentStockFilter.filter));
            }
            DateTime init = new DateTime();
            if (adjustmentStockFilter.startDateFilter > init && adjustmentStockFilter.endDateFilter > init)
            {
                adjustmentStockFilter.startDateFilter = UtilExtensions.ConvertStartDate(adjustmentStockFilter.startDateFilter.ToLocalTime());
                adjustmentStockFilter.endDateFilter = UtilExtensions.ConvertEndDate(adjustmentStockFilter.endDateFilter.ToLocalTime());
                query = query.Where(x => x.CreatedDate >= adjustmentStockFilter.startDateFilter && x.CreatedDate <= adjustmentStockFilter.endDateFilter);
            }

            softAdjustmentStocks = query;
            totalRow = softAdjustmentStocks.Count();

            switch (adjustmentStockFilter.sortBy)
            {
                case "CreatedDate_des":
                    softAdjustmentStocks = softAdjustmentStocks.OrderByDescending(x => x.CreatedDate);
                    break;
                case "CreatedDate_asc":
                    softAdjustmentStocks = softAdjustmentStocks.OrderBy(x => x.CreatedDate);
                    break;
                default:
                    softAdjustmentStocks = softAdjustmentStocks.OrderByDescending(x => x.Id);
                    break;
            }

            return softAdjustmentStocks.Skip(page * pageSize).Take(pageSize);
        }
    }
}