using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftAdjustmentStockDetailRepository : IRepository<SoftAdjustmentStockDetail>
    {

    }

    public class SoftAdjustmentStockDetailRepository : RepositoryBase<SoftAdjustmentStockDetail>, ISoftAdjustmentStockDetailRepository
    {
        public SoftAdjustmentStockDetailRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}