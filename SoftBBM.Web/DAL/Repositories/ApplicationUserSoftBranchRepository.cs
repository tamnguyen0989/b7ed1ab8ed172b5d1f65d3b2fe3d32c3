using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IApplicationUserSoftBranchRepository : IRepository<ApplicationUserSoftBranch>
    {

    }
    public class ApplicationUserSoftBranchRepository : RepositoryBase<ApplicationUserSoftBranch>, IApplicationUserSoftBranchRepository
    {
        public ApplicationUserSoftBranchRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}