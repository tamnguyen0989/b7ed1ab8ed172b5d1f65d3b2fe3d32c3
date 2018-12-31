using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftStockInPaymentTypeRepository : IRepository<SoftStockInPaymentType>
    {

    }
    public class SoftStockInPaymentTypeRepository : RepositoryBase<SoftStockInPaymentType>, ISoftStockInPaymentTypeRepository
    {
        public SoftStockInPaymentTypeRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}