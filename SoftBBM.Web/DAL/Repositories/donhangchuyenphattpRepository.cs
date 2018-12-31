using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface IdonhangchuyenphattpRepository : IRepository<donhang_chuyenphat_tp>
    {
    }

    public class donhangchuyenphattpRepository : RepositoryBase<donhang_chuyenphat_tp>, IdonhangchuyenphattpRepository
    {
        public donhangchuyenphattpRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}