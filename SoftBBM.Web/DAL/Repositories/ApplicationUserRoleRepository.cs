using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IApplicationUserRoleRepository : IRepository<ApplicationUserRole>
    {

    }
    public class ApplicationUserRoleRepository : RepositoryBase<ApplicationUserRole>,IApplicationUserRoleRepository
    {
        public ApplicationUserRoleRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}