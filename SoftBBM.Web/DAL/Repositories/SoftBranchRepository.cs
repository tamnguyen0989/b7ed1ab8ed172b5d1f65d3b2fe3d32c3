using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{


    public interface ISoftBranchRepository : IRepository<SoftBranch>
    {
        IEnumerable<int> GetAllIds();
        bool AddUserToBranches(IEnumerable<ApplicationUserSoftBranch> userBranches, int userId);
    }
    public class SoftBranchRepository : RepositoryBase<SoftBranch>, ISoftBranchRepository
    {
        IApplicationUserSoftBranchRepository _applicationUserSoftBranchRepository;


        public SoftBranchRepository(IDbFactory dbFactory, IApplicationUserSoftBranchRepository applicationUserSoftBranchRepository) : base(dbFactory)
        {
            _applicationUserSoftBranchRepository = applicationUserSoftBranchRepository;
        }

        public bool AddUserToBranches(IEnumerable<ApplicationUserSoftBranch> userBranches, int userId)
        {
            _applicationUserSoftBranchRepository.DeleteMulti(x => x.UserId == userId);
            foreach (var item in userBranches)
            {
                _applicationUserSoftBranchRepository.Add(item);
            }
            return true;
        }

        public IEnumerable<int> GetAllIds()
        {
            return DbContext.SoftBranches.Select(x => x.Id);
        }

    }
}