using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IshopbientheRepository : IRepository<shop_bienthe>
    {
    }

    public class shopbientheRepository : RepositoryBase<shop_bienthe>, IshopbientheRepository
    {
        public shopbientheRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}