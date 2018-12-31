using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface IdonhangchuyenphatdiachifutaRepository : IRepository<donhang_chuyenphat_danhsachdiachifuta>
    {
    }

    public class donhangchuyenphatdiachifutaRepository : RepositoryBase<donhang_chuyenphat_danhsachdiachifuta>, IdonhangchuyenphatdiachifutaRepository
    {
        public donhangchuyenphatdiachifutaRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}