using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IdonhangchuyenphattinhRepository : IRepository<donhang_chuyenphat_tinh>
    {
    }

    public class donhangchuyenphattinhRepository : RepositoryBase<donhang_chuyenphat_tinh>, IdonhangchuyenphattinhRepository
    {
        public donhangchuyenphattinhRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}