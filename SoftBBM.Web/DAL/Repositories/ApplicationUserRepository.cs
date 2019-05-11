using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        IEnumerable<ApplicationRole> GetListRoleByUserId(int userId);
        IQueryable<ApplicationUser> GetAllPaging(int page, int pageSize, out int totalRow, string filter);
        IEnumerable<SoftBranch> GetListBranchByUserName(string userName);
        void DeleteApplicationUser(int userId);
    }
    public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IApplicationUserRepository
    {
        IApplicationUserRoleRepository _applicationUserRoleRepository;
        public ApplicationUserRepository(IDbFactory dbFactory, IApplicationUserRoleRepository applicationUserRoleRepository) : base(dbFactory)
        {
            this._applicationUserRoleRepository = applicationUserRoleRepository;
        }

        public IQueryable<ApplicationUser> GetAllPaging(int page, int pageSize, out int totalRow, string filter)
        {
            var query = from g in DbContext.ApplicationUsers
                        where g.Id != 0
                        select g;
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.FullName.Contains(filter));

            totalRow = query.Count();
            return query.OrderByDescending(x => x.Id).Skip(page * pageSize).Take(pageSize);
        }

        public IEnumerable<ApplicationRole> GetListRoleByUserId(int userId)
        {
            var query = from ur in DbContext.ApplicationUserRoles
                        join r in DbContext.ApplicationRoles
                        on ur.RoleId equals r.Id
                        where ur.UserId == userId
                        select r;
            return query;
        }

        public IEnumerable<SoftBranch> GetListBranchByUserName(string userName)
        {
            var query = from u in DbContext.ApplicationUsers
                        join ub in DbContext.ApplicationUserSoftBranches
                        on u.Id equals ub.UserId
                        join b in DbContext.SoftBranches
                        on ub.BranchId equals b.Id
                        where u.UserName == userName
                        select b;
            return query;
        }

        public void DeleteApplicationUser(int userId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@userId",userId)
            };
            DbContext.Database.ExecuteSqlCommand("exec DeleteApplicationUser @userId", parameters);
        }
    }
}