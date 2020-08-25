using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IShopSanPhamLogRepository : IRepository<shop_sanphamLogs>
    {
        IQueryable<shop_sanphamLogs> GetAllPaging(int page, int pageSize, out int totalRow, ProductLogFilterViewModel productLogFilterVM);
    }
    public class ShopSanPhamLogRepository : RepositoryBase<shop_sanphamLogs>, IShopSanPhamLogRepository
    {
        public ShopSanPhamLogRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<shop_sanphamLogs> GetAllPaging(int page, int pageSize, out int totalRow, ProductLogFilterViewModel productLogFilterVM)
        {
            IQueryable<shop_sanphamLogs> query = null;

            if (productLogFilterVM.branchId > 0)
                query = from d in DbContext.shop_sanphamLogs
                        where d.BranchId == productLogFilterVM.branchId
                        select d;
            else
                query = from d in DbContext.shop_sanphamLogs
                        select d;

            IQueryable<shop_sanphamLogs> shop_sanphamLogss = null;
            //if (!string.IsNullOrEmpty(productLogFilterVM.filter))
            //{
            //    shop_sanphamLogss = query.Where(c => c.SoftSupplier.Name.ToLower().Contains(productLogFilterVM.filter) || c.Id.ToString() == productLogFilterVM.filter);
            //    totalRow = shop_sanphamLogss.Count();
            //}
            //else
            {
                bool rootExist = false;
                DateTime init = new DateTime();
                IQueryable<shop_sanphamLogs> shop_sanphamLogssfilter = null;
                if (!string.IsNullOrEmpty(productLogFilterVM.filter) || (productLogFilterVM.startDateFilter > init && productLogFilterVM.endDateFilter > init))
                {
                    if (!string.IsNullOrEmpty(productLogFilterVM.filter))
                    {
                        if (rootExist == false)
                            shop_sanphamLogss = query.Where(c => c.Id.ToString() == productLogFilterVM.filter || c.shop_sanpham.masp.Contains(productLogFilterVM.filter));
                        else
                            shop_sanphamLogss = shop_sanphamLogss.Where(c => c.Id.ToString() == productLogFilterVM.filter || c.shop_sanpham.masp.Contains(productLogFilterVM.filter));
                        if (rootExist == false) rootExist = true;
                    }
                    if (productLogFilterVM.startDateFilter > init && productLogFilterVM.endDateFilter > init)
                    {
                        productLogFilterVM.startDateFilter = UtilExtensions.ConvertStartDate(productLogFilterVM.startDateFilter.ToLocalTime());
                        productLogFilterVM.endDateFilter = UtilExtensions.ConvertEndDate(productLogFilterVM.endDateFilter.ToLocalTime());
                        IQueryable<shop_sanphamLogs> shop_sanphamLogsstmp = null;
                        if (rootExist == false)
                            shop_sanphamLogsstmp = query.Where(x => x.CreatedDate >= productLogFilterVM.startDateFilter && x.CreatedDate <= productLogFilterVM.endDateFilter);
                        else
                            shop_sanphamLogsstmp = shop_sanphamLogss.Where(x => x.CreatedDate >= productLogFilterVM.startDateFilter && x.CreatedDate <= productLogFilterVM.endDateFilter);
                        shop_sanphamLogss = shop_sanphamLogsstmp;
                        if (rootExist == false) rootExist = true;
                    }
                }
                else
                {
                    shop_sanphamLogss = query;
                }
                totalRow = shop_sanphamLogss.Count();
            }

            //switch (productLogFilterVM.sortBy)
            //{
            //    case "Total_des":
            //        shop_sanphamLogss = shop_sanphamLogss.OrderByDescending(x => x.Total);
            //        break;
            //    case "Total_asc":
            //        shop_sanphamLogss = shop_sanphamLogss.OrderBy(x => x.Total);
            //        break;
            //    case "CreatedDate_des":
            //        shop_sanphamLogss = shop_sanphamLogss.OrderByDescending(x => x.CreatedDate);
            //        break;
            //    case "CreatedDate_asc":
            //        shop_sanphamLogss = shop_sanphamLogss.OrderBy(x => x.CreatedDate);
            //        break;
            //    default:
            //        shop_sanphamLogss = shop_sanphamLogss.OrderByDescending(x => x.Id);
            //        break;
            //}
            shop_sanphamLogss = shop_sanphamLogss.OrderByDescending(x => x.Id);
            return shop_sanphamLogss.Skip(page * pageSize).Take(pageSize);
        }
    }
}