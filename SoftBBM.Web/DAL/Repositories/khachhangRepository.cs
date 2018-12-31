using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface IkhachhangRepository : IRepository<khachhang>
    {
        IQueryable<khachhang> GetAllPaging(int page, int pageSize, out int totalRow, CustomerFilterViewModel customerFilterVM);
    }

    public class khachhangRepository : RepositoryBase<khachhang>, IkhachhangRepository
    {
        public khachhangRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IQueryable<khachhang> GetAllPaging(int page, int pageSize, out int totalRow, CustomerFilterViewModel customerFilterVM)
        {
            var query = from k in DbContext.khachhangs
                        select k;
            IQueryable<khachhang> khachhangs = null;
            var filter = customerFilterVM.filter;
            {
                if (filter != null)
                    khachhangs = query.Where(x => x.MaKH.ToString() == filter || x.tendn.Contains(filter) || x.hoten.Contains(filter) || x.email.Contains(filter) || x.duong.Contains(filter) || x.dienthoai.Contains(filter));
                else
                    khachhangs = query;
                totalRow = khachhangs.Count();
            }
            khachhangs = khachhangs.OrderByDescending(x => x.MaKH);

            return khachhangs.Skip(page * pageSize).Take(pageSize);
        }
    }
}