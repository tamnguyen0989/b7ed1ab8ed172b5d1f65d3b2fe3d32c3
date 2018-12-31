using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IApplicationGroupRepository : IRepository<ApplicationGroup>
    {
        IEnumerable<ApplicationGroup> GetListGroupByUserId(int userId);
        IEnumerable<ApplicationUser> GetListUserByGroupId(int groupId);
        IEnumerable<ApplicationGroup> GetAllPaging(int page, int pageSize, out int totalRow, string filter);
        bool AddUserToGroups(IEnumerable<ApplicationUserGroup> userGroups, int userId);
    }
    public class ApplicationGroupRepository : RepositoryBase<ApplicationGroup>, IApplicationGroupRepository
    {
        IApplicationUserGroupRepository _appUserGroupRepository;
        IApplicationUserRoleRepository _appUserRoleRepository;

        public ApplicationGroupRepository(IDbFactory dbFactory, IApplicationUserGroupRepository appUserGroupRepository, IApplicationUserRoleRepository appUserRoleRepository) : base(dbFactory)
        {
            _appUserGroupRepository = appUserGroupRepository;
            _appUserRoleRepository = appUserRoleRepository;
        }

        public bool AddUserToGroups(IEnumerable<ApplicationUserGroup> userGroups, int userId)
        {
            _appUserGroupRepository.DeleteMulti(x => x.UserId == userId);
            foreach (var userGroup in userGroups)
            {
                _appUserGroupRepository.Add(userGroup);
            }
            return true;
        }

        public IEnumerable<ApplicationGroup> GetAllPaging(int page, int pageSize, out int totalRow, string filter)
        {
            var query = from g in DbContext.ApplicationGroups select g;
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Name.Contains(filter));

            totalRow = query.Count();
            return query.OrderByDescending(x => x.Id).Skip(page * pageSize).Take(pageSize);
        }

        public IEnumerable<ApplicationGroup> GetListGroupByUserId(int userId)
        {
            var query = from g in DbContext.ApplicationGroups
                        join ug in DbContext.ApplicationUserGroups
                        on g.Id equals ug.GroupId
                        where ug.UserId == userId
                        select g;
            return query;
        }

        public IEnumerable<ApplicationUser> GetListUserByGroupId(int groupId)
        {
            var query = from g in DbContext.ApplicationGroups
                        join ug in DbContext.ApplicationUserGroups
                        on g.Id equals ug.GroupId
                        join u in DbContext.ApplicationUsers
                        on ug.UserId equals u.Id
                        where ug.GroupId == groupId
                        select u;
            return query;
        }
    }
}