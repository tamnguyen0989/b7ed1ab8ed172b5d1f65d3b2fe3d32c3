using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface IdonhangctRepository : IRepository<donhang_ct>
    {

    }
    public class donhangctRepository : RepositoryBase<donhang_ct>, IdonhangctRepository
    {
        public donhangctRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}