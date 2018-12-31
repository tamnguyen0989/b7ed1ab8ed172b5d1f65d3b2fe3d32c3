using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftChannelRepository : IRepository<SoftChannel>
    {
        IEnumerable<int> GetAllIds();
    }
    public class SoftChannelRepository : RepositoryBase<SoftChannel>, ISoftChannelRepository
    {
        public SoftChannelRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IEnumerable<int> GetAllIds()
        {
            return DbContext.SoftChannels.Select(x => x.Id);
        }
    }
}