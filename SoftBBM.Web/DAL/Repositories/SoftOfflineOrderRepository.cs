using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftOfflineOrderWindowRepository : IRepository<SoftOfflineOrderWindow>
    {

    }

    public class SoftOfflineOrderWindowRepository : RepositoryBase<SoftOfflineOrderWindow>, ISoftOfflineOrderWindowRepository
    {
        public SoftOfflineOrderWindowRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}