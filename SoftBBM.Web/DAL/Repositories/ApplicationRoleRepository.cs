using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IApplicationRoleRepository : IRepository<ApplicationRole>
    {
        IEnumerable<ApplicationRole> GetListRoleByGroupId(int groupId);
        IEnumerable<ApplicationRole> GetAllPaging(int page,int pageSize, out int totalRow,string filter);
        bool AddRolesToGroup(IEnumerable<ApplicationRoleGroup> roleGroups, int groupId);
        IEnumerable<ApplicationRole> GetListRoleByCategoryId(int categoryId);
    }
    public class ApplicationRoleRepository : RepositoryBase<ApplicationRole>, IApplicationRoleRepository
    {
        IApplicationRoleGroupRepository _appRoleGroupRepository;
        public ApplicationRoleRepository(IDbFactory dbFactory, IApplicationRoleGroupRepository appRoleGroupRepository) : base(dbFactory)
        {
            _appRoleGroupRepository = appRoleGroupRepository;
        }

        public bool AddRolesToGroup(IEnumerable<ApplicationRoleGroup> roleGroups, int groupId)
        {
            _appRoleGroupRepository.DeleteMulti(x => x.GroupId == groupId);
            foreach (var roleGroup in roleGroups)
            {
                _appRoleGroupRepository.Add(roleGroup);
            }
            return true;
        }

        public IEnumerable<ApplicationRole> GetAllPaging(int page, int pageSize, out int totalRow, string filter)
        {
            var query = from g in DbContext.ApplicationRoles select g;
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Description.Contains(filter) || x.Name.Contains(filter));
            totalRow = query.Count();
            return query.OrderByDescending(x => x.Id).Skip(page * pageSize).Take(pageSize);
        }

        public IEnumerable<ApplicationRole> GetListRoleByCategoryId(int categoryId)
        {

            return DbContext.ApplicationRoles.Where(x => x.CategoryId == categoryId);
        }

        public IEnumerable<ApplicationRole> GetListRoleByGroupId(int groupId)
        {
            var query = from g in DbContext.ApplicationRoles
                        join ug in DbContext.ApplicationRoleGroups
                        on g.Id equals ug.RoleId
                        where ug.GroupId == groupId
                        select g;
            return query;
        }
    }
}