using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftSupplierRepository : IRepository<SoftSupplier>
    {
        void DeleteSupplier(int supplierId);
    }
    public class SoftSupplierRepository : RepositoryBase<SoftSupplier>, ISoftSupplierRepository
    {
        public SoftSupplierRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public void DeleteSupplier(int supplierId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@supplierId",supplierId)
            };
            DbContext.Database.ExecuteSqlCommand("exec DeleteSupplier @supplierId", parameters);
        }
    }
}