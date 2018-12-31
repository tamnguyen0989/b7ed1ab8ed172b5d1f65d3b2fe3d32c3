using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftSupplierRepository : IRepository<SoftSupplier>
    {

    }
    public class SoftSupplierRepository : RepositoryBase<SoftSupplier>, ISoftSupplierRepository
    {
        public SoftSupplierRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}