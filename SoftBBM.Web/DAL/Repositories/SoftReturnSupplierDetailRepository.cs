using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftReturnSupplierDetailRepository : IRepository<SoftReturnSupplierDetail>
    {

    }

    public class SoftReturnSupplierDetailRepository : RepositoryBase<SoftReturnSupplierDetail>, ISoftReturnSupplierDetailRepository
    {
        public SoftReturnSupplierDetailRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}