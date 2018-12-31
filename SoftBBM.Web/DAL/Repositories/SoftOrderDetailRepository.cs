using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftOrderDetailRepository : IRepository<SoftOrderDetail>
    {

    }
    public class SoftOrderDetailRepository : RepositoryBase<SoftOrderDetail>, ISoftOrderDetailRepository
    {
        public SoftOrderDetailRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}