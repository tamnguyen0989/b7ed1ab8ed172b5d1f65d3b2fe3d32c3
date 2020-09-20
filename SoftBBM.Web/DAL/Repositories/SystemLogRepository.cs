using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISystemLogRepository : IRepository<SystemLog>
    {

    }
    public class SystemLogRepository : RepositoryBase<SystemLog>, ISystemLogRepository
    {
        public SystemLogRepository(IDbFactory dbFactory) : base(dbFactory)
        {
            
        }
    }
}