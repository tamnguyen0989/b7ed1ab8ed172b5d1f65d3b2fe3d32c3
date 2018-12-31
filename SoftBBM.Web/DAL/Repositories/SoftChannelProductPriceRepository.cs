using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftChannelProductPriceRepository : IRepository<SoftChannelProductPrice>
    {

    }
    public class SoftChannelProductPriceRepository : RepositoryBase<SoftChannelProductPrice>, ISoftChannelProductPriceRepository
    {
        public SoftChannelProductPriceRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

    }
}