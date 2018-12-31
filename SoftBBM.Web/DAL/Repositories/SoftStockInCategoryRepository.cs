using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface ISoftStockInCategoryRepository : IRepository<SoftStockInCategory>
    {

    }
    public class SoftStockInCategoryRepository : RepositoryBase<SoftStockInCategory>, ISoftStockInCategoryRepository
    {
        public SoftStockInCategoryRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}