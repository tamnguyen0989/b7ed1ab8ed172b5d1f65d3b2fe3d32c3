using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftStockInDetailRepository : IRepository<SoftStockInDetail>
    {
        IEnumerable<SoftStockInDetail> GetAllFilter(string filter);
    }
    public class SoftStockInDetailRepository : RepositoryBase<SoftStockInDetail>, ISoftStockInDetailRepository
    {
        public SoftStockInDetailRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IEnumerable<SoftStockInDetail> GetAllFilter(string filter)
        {
            throw new NotImplementedException();
        }
    }
}