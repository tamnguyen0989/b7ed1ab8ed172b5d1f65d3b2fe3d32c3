using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftStockInPaymentStatusRepository : IRepository<SoftStockInPaymentStatus>
    {

    }
    public class SoftStockInPaymentStatusRepository : RepositoryBase<SoftStockInPaymentStatus>, ISoftStockInPaymentStatusRepository
    {
        public SoftStockInPaymentStatusRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}