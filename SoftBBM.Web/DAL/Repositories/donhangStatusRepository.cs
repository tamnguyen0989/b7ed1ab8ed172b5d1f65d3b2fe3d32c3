using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface IdonhangStatusRepository : IRepository<donhangStatu>
    {
    }

    public class donhangStatusRepository : RepositoryBase<donhangStatu>, IdonhangStatusRepository
    {
        public donhangStatusRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}