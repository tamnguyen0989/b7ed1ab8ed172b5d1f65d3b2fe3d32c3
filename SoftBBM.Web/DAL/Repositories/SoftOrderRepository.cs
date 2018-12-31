using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftOrderRepository : IRepository<SoftOrder>
    {

    }
    public class SoftOrderRepository : RepositoryBase<SoftOrder>, ISoftOrderRepository
    {
        public SoftOrderRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}