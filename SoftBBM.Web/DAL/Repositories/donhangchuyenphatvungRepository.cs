using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IdonhangchuyenphatvungRepository : IRepository<donhang_chuyenphat_vung>
    {
    }

    public class donhangchuyenphatvungRepository : RepositoryBase<donhang_chuyenphat_vung>, IdonhangchuyenphatvungRepository
    {
        public donhangchuyenphatvungRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}