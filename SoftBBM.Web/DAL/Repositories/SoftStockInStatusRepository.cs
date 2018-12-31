using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface ISoftStockInStatusRepository : IRepository<SoftStockInStatu>
    {

    }
    public class SoftStockInStatusRepository : RepositoryBase<SoftStockInStatu>, ISoftStockInStatusRepository
    {
        public SoftStockInStatusRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}