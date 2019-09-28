using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftPointUpdateLogRepository : IRepository<SoftPointUpdateLog>
    {

    }
    public class SoftPointUpdateLogRepository : RepositoryBase<SoftPointUpdateLog>, ISoftPointUpdateLogRepository
    {
        public SoftPointUpdateLogRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}