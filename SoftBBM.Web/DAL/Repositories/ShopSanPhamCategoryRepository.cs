using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{

    public interface IShopSanPhamCategoryRepository : IRepository<shop_sanphamCategories>
    {
        void DeleteSanPhamCategory(int categoryId);
    }

    public class ShopSanPhamCategoryRepository : RepositoryBase<shop_sanphamCategories>, IShopSanPhamCategoryRepository
    {
        public ShopSanPhamCategoryRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public void DeleteSanPhamCategory(int categoryId)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@categoryId",categoryId)
            };
            DbContext.Database.ExecuteSqlCommand("exec DeleteShopSanPhamCategory @categoryId", parameters);
        }
    }

}