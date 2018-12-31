using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IShopSanPhamRepository : IRepository<shop_sanpham>
    {
        IEnumerable<shop_sanpham> GetAllByBranchId(int branchId);
        IEnumerable<int> GetAllIds();
        IQueryable<shop_sanpham> GetAllPaging(int page, int pageSize, out int totalRow, int branchId, string searchString = "");
        IQueryable<shop_sanpham> GetAllPaging(int page, int pageSize, out int totalRow, int branchId, ShopSanPhamFilterBookViewModel model);
    }
    public class ShopSanPhamRepository : RepositoryBase<shop_sanpham>, IShopSanPhamRepository
    {
        public ShopSanPhamRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IEnumerable<shop_sanpham> GetAllByBranchId(int branchId)
        {
            var query = from s in DbContext.shop_sanpham
                        join b in DbContext.SoftBranchProductStocks
                        on s.id equals b.ProductId
                        where b.BranchId == branchId
                        select s;
            return query;
        }

        public IEnumerable<int> GetAllIds()
        {
            return DbContext.shop_sanpham.Select(x => x.id);
        }

        public IQueryable<shop_sanpham> GetAllPaging(int page, int pageSize, out int totalRow, int branchId, ShopSanPhamFilterBookViewModel model)
        {
            var query = from d in DbContext.shop_sanpham
                        select d;
            IQueryable<shop_sanpham> products = null;

            if (!string.IsNullOrEmpty(model.stringSearch))
            {
                products = query.Where(x => x.masp.Contains(model.stringSearch) || x.tensp.Contains(model.stringSearch));
            }
            else
                products = query;

            foreach (var item in model.FilterBookDetail)
            {
                switch (item.key)
                {
                    case 0:
                        products = products.Where(x => x.SupplierId == item.value);
                        break;
                    case 1:
                        var convert = item.value.ToString();
                        if (convert.Length == 1)
                        {
                            convert = '0' + convert;
                        }
                        products = products.Where(x => x.StatusId == convert);
                        break;
                }
            }

            totalRow = products.Count();
            products = products.OrderByDescending(x => x.id);
            return products.Skip(page * pageSize).Take(pageSize);
        }

        public IQueryable<shop_sanpham> GetAllPaging(int page, int pageSize, out int totalRow, int branchId, string searchString = "")
        {
            var query = from d in DbContext.shop_sanpham
                        select d;
            IQueryable<shop_sanpham> products = null;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = query.Where(x => x.masp.Contains(searchString) || x.tensp.Contains(searchString));
            }
            else
                products = query;
            totalRow = products.Count();
            products = products.OrderByDescending(x => x.id);
            return products.Skip(page * pageSize).Take(pageSize);
        }
    }
}