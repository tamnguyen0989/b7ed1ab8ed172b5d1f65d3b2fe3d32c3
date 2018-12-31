using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IApplicationUserGroupRepository : IRepository<ApplicationUserGroup>
    {
        IEnumerable<ApplicationRole> GetListRolesByUserId(int id);

    }
    public class ApplicationUserGroupRepository : RepositoryBase<ApplicationUserGroup>, IApplicationUserGroupRepository
    {
        public ApplicationUserGroupRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IEnumerable<ApplicationRole> GetListRolesByUserId(int id)
        {
            var query = from r in DbContext.ApplicationRoles
                        join ur in DbContext.ApplicationUserRoles
                        on r.Id equals ur.RoleId
                        where ur.UserId == id
                        select r;
            return query;
        }
    }
}