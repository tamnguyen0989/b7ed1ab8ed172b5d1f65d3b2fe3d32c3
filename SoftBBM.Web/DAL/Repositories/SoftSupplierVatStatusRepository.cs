using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface ISoftSupplierVatStatusRepository : IRepository<SoftSupplierVatStatu>
    {

    }
    public class SoftSupplierVatStatusRepository : RepositoryBase<SoftSupplierVatStatu>, ISoftSupplierVatStatusRepository
    {
        public SoftSupplierVatStatusRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}