using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftStockRepository : IRepository<SoftBranchProductStock>
    {
        IQueryable<SoftBranchProductStock> GetAllPaging(int page, int pageSize, out int totalRow, string filter, int branchId);
        double GetStockTotalAll(int productId);
        double GetStockTotal(int productId, int branchId);
        IQueryable<SoftBranchProductStock> GetAllFilter(int branchId, string filter);
        IQueryable<SoftBranchProductStock> GetAllPagingFilter(out int totalRow, SoftStockSearchFilterViewModel softStockSearchFilterVM);
        SoftBranchProductStock Init(int branchId, int productId);
        IQueryable<SoftBranchProductStock> GetAllPagingFilter(SoftStockSearchFilterViewModel softStockSearchFilterVM);
    }
    public class SoftStockRepository : RepositoryBase<SoftBranchProductStock>, ISoftStockRepository
    {
        public SoftStockRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<SoftBranchProductStock> GetAllFilter(int branchId, string filter)
        {
            var query = from d in DbContext.SoftBranchProductStocks
                        where d.BranchId == branchId
                        select d;
            IQueryable<SoftBranchProductStock> stocks = null;

            if (!string.IsNullOrEmpty(filter))
            {
                stocks = query.Where(c => c.shop_sanpham.masp.ToLower().Contains(filter) || c.shop_sanpham.tensp.ToLower().Contains(filter) || c.shop_sanpham.id.ToString().Contains(filter));
            }
            else
            {
                stocks = query;
            }
            return stocks;
        }

        public IQueryable<SoftBranchProductStock> GetAllPaging(int page, int pageSize, out int totalRow, string filter, int branchId)
        {
            var query = from d in DbContext.SoftBranchProductStocks
                        where d.BranchId == branchId
                        select d;
            IQueryable<SoftBranchProductStock> stocks = null;

            if (!string.IsNullOrEmpty(filter))
            {
                stocks = query.Where(c => c.shop_sanpham.masp.ToLower().Contains(filter) || c.shop_sanpham.tensp.ToLower().Contains(filter) || c.shop_sanpham.id.ToString().Contains(filter));
            }
            else
            {
                stocks = query;
            }
            totalRow = stocks.Count();
            return stocks.OrderByDescending(x => x.ProductId).Skip(page * pageSize).Take(pageSize);
        }

        public IQueryable<SoftBranchProductStock> GetAllPagingFilter(SoftStockSearchFilterViewModel softStockSearchFilterVM)

        {
            var query = from d in DbContext.SoftBranchProductStocks
                        where d.BranchId == softStockSearchFilterVM.branchId
                        select d;
            IQueryable<SoftBranchProductStock> stocks = null;
            {
                bool rootExist = false;
                IQueryable<SoftBranchProductStock> stocksFilter = null;
                if (!string.IsNullOrEmpty(softStockSearchFilterVM.stringFilter) || softStockSearchFilterVM.selectedProductCategoryFilters.Count > 0 || softStockSearchFilterVM.selectedSupplierFilters.Count > 0 ||
                    softStockSearchFilterVM.selectedProductStatusFilters.Count > 0 || softStockSearchFilterVM.selectedVatStatusFilters.Count > 0 || (softStockSearchFilterVM.selectedStockFilter > 0 && softStockSearchFilterVM.selectedStockFilterValue != null) || (softStockSearchFilterVM.selectedStockTotalFilter > 0 && softStockSearchFilterVM.selectedStockTotalFilterValue != null))
                {
                    if (!string.IsNullOrEmpty(softStockSearchFilterVM.stringFilter))
                    {
                        if (rootExist == false)
                            stocks = query.Where(c => c.shop_sanpham.tensp.ToLower().Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.id.ToString() == softStockSearchFilterVM.stringFilter || c.shop_sanpham.masp.Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.SoftSupplier.Name.Contains(softStockSearchFilterVM.stringFilter));
                        else
                            stocks = stocks.Where(c => c.shop_sanpham.tensp.ToLower().Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.id.ToString() == softStockSearchFilterVM.stringFilter || c.shop_sanpham.masp.Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.SoftSupplier.Name.Contains(softStockSearchFilterVM.stringFilter));
                        if (rootExist == false) rootExist = true;
                    }
                    if (softStockSearchFilterVM.selectedProductCategoryFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedProductCategoryFilters)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.shop_sanpham.CategoryId == item);
                            else
                                donhangstmp = stocks.Where(x => x.shop_sanpham.CategoryId == item);
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedSupplierFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedSupplierFilters)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.shop_sanpham.SupplierId == item);
                            else
                                donhangstmp = stocks.Where(x => x.shop_sanpham.SupplierId == item);
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedProductStatusFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedProductStatusFilters)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.shop_sanpham.StatusId == item);
                            else
                                donhangstmp = stocks.Where(x => x.shop_sanpham.StatusId == item);
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedVatStatusFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedVatStatusFilters)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.shop_sanpham.SoftSupplier.VatId == item);
                            else
                                donhangstmp = stocks.Where(x => x.shop_sanpham.SoftSupplier.VatId == item);
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedStockFilter > 0 && softStockSearchFilterVM.selectedStockFilterValue != null)
                    {
                        if (rootExist == false)
                        {
                            switch (softStockSearchFilterVM.selectedStockFilter)
                            {
                                case 1:
                                    stocks = query.Where(x => x.StockTotal > softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 2:
                                    stocks = query.Where(x => x.StockTotal < softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 3:
                                    stocks = query.Where(x => x.StockTotal == softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                            }
                        }
                        else
                        {
                            switch (softStockSearchFilterVM.selectedStockFilter)
                            {
                                case 1:
                                    stocks = stocks.Where(x => x.StockTotal > softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 2:
                                    stocks = stocks.Where(x => x.StockTotal < softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 3:
                                    stocks = stocks.Where(x => x.StockTotal == softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                            }
                        }
                        if (rootExist == false) rootExist = true;
                    }
                    if (softStockSearchFilterVM.selectedStockTotalFilter > 0 && softStockSearchFilterVM.selectedStockTotalFilterValue != null)
                    {
                        if (rootExist == false)
                        {
                            switch (softStockSearchFilterVM.selectedStockTotalFilter)
                            {
                                case 1:
                                    stocks = query.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) > softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 2:
                                    stocks = query.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) < softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 3:
                                    stocks = query.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) == softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                            }
                        }
                        else
                        {
                            switch (softStockSearchFilterVM.selectedStockTotalFilter)
                            {
                                case 1:
                                    stocks = stocks.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) > softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 2:
                                    stocks = stocks.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) < softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 3:
                                    stocks = stocks.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) == softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                            }
                        }
                        if (rootExist == false) rootExist = true;
                    }
                }
                else
                {
                    stocks = query;
                }
            }

            switch (softStockSearchFilterVM.sortBy)
            {
                case "productCode_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.masp);
                    break;
                case "productCode_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.masp);
                    break;
                case "productName_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.tensp);
                    break;
                case "productName_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.tensp);
                    break;
                case "stock_Des":
                    stocks = stocks.OrderByDescending(x => x.StockTotal);
                    break;
                case "stock_Asc":
                    stocks = stocks.OrderBy(x => x.StockTotal);
                    break;
                case "stockTotal_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal));
                    break;
                case "stockTotal_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal));
                    break;
                case "priceBase_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.PriceBase);
                    break;
                case "priceBase_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.PriceBase);
                    break;
                default:
                    stocks = stocks.OrderBy(x => x.shop_sanpham.tensp);
                    break;
            }
            return stocks;
        }

        public IQueryable<SoftBranchProductStock> GetAllPagingFilter(out int totalRow, SoftStockSearchFilterViewModel softStockSearchFilterVM)
        {
            var query = from d in DbContext.SoftBranchProductStocks
                        where d.BranchId == softStockSearchFilterVM.branchId
                        select d;
            IQueryable<SoftBranchProductStock> stocks = null;
            {
                bool rootExist = false;
                IQueryable<SoftBranchProductStock> stocksFilter = null;
                if (!string.IsNullOrEmpty(softStockSearchFilterVM.stringFilter) || softStockSearchFilterVM.selectedProductCategoryFilters.Count > 0 || softStockSearchFilterVM.selectedSupplierFilters.Count > 0 ||
                    softStockSearchFilterVM.selectedProductStatusFilters.Count > 0 || softStockSearchFilterVM.selectedVatStatusFilters.Count > 0 || (softStockSearchFilterVM.selectedStockFilter > 0 && softStockSearchFilterVM.selectedStockFilterValue != null) || (softStockSearchFilterVM.selectedStockTotalFilter > 0 && softStockSearchFilterVM.selectedStockTotalFilterValue != null) || softStockSearchFilterVM.selectedHideStatusFilter.Count > 0)
                {
                    if (!string.IsNullOrEmpty(softStockSearchFilterVM.stringFilter))
                    {
                        if (rootExist == false)
                            stocks = query.Where(c => c.shop_sanpham.tensp.ToLower().Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.id.ToString() == softStockSearchFilterVM.stringFilter || c.shop_sanpham.masp.Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.SoftSupplier.Name.Contains(softStockSearchFilterVM.stringFilter));
                        else
                            stocks = stocks.Where(c => c.shop_sanpham.tensp.ToLower().Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.id.ToString() == softStockSearchFilterVM.stringFilter || c.shop_sanpham.masp.Contains(softStockSearchFilterVM.stringFilter) || c.shop_sanpham.SoftSupplier.Name.Contains(softStockSearchFilterVM.stringFilter));
                        if (rootExist == false) rootExist = true;
                    }
                    if (softStockSearchFilterVM.selectedProductCategoryFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedProductCategoryFilters)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.shop_sanpham.CategoryId == item);
                            else
                                donhangstmp = stocks.Where(x => x.shop_sanpham.CategoryId == item);
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedSupplierFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedSupplierFilters)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.shop_sanpham.SupplierId == item);
                            else
                                donhangstmp = stocks.Where(x => x.shop_sanpham.SupplierId == item);
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedProductStatusFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedProductStatusFilters)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                                donhangstmp = query.Where(x => x.shop_sanpham.StatusId == item);
                            else
                                donhangstmp = stocks.Where(x => x.shop_sanpham.StatusId == item);
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedVatStatusFilters.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedVatStatusFilters)
                        {
                            if (item < 0)
                            {
                                IQueryable<SoftBranchProductStock> donhangstmp = null;
                                if (rootExist == false)
                                {
                                    switch (item)
                                    {
                                        case -1:
                                            donhangstmp = query.Where(x => x.shop_sanpham.hide == true || x.shop_sanpham.hide == null);
                                            break;
                                        case -3:
                                            donhangstmp = query.Where(x => x.shop_sanpham.hide == false && x.shop_sanpham.shop_collection.Count > 0);
                                            break;
                                        default:
                                            donhangstmp = query.Where(x => x.shop_sanpham.hide == false);
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (item)
                                    {
                                        case -1:
                                            donhangstmp = stocks.Where(x => x.shop_sanpham.hide == true || x.shop_sanpham.hide == null);
                                            break;
                                        case -3:
                                            donhangstmp = stocks.Where(x => x.shop_sanpham.hide == false && x.shop_sanpham.shop_collection.Count > 0);
                                            break;
                                        default:
                                            donhangstmp = stocks.Where(x => x.shop_sanpham.hide == false);
                                            break;
                                    }
                                }
                                if (stocksFilter == null)
                                    stocksFilter = donhangstmp;
                                else
                                    stocksFilter = stocksFilter.Union(donhangstmp);
                            }
                            else
                            {
                                IQueryable<SoftBranchProductStock> donhangstmp = null;
                                if (rootExist == false)
                                    donhangstmp = query.Where(x => x.shop_sanpham.SoftSupplier.VatId == item);
                                else
                                    donhangstmp = stocks.Where(x => x.shop_sanpham.SoftSupplier.VatId == item);
                                if (stocksFilter == null)
                                    stocksFilter = donhangstmp;
                                else
                                    stocksFilter = stocksFilter.Union(donhangstmp);
                            }
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                    if (softStockSearchFilterVM.selectedStockFilter > 0 && softStockSearchFilterVM.selectedStockFilterValue != null)
                    {
                        if (rootExist == false)
                        {
                            switch (softStockSearchFilterVM.selectedStockFilter)
                            {
                                case 1:
                                    stocks = query.Where(x => x.StockTotal > softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 2:
                                    stocks = query.Where(x => x.StockTotal < softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 3:
                                    stocks = query.Where(x => x.StockTotal == softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                            }
                        }
                        else
                        {
                            switch (softStockSearchFilterVM.selectedStockFilter)
                            {
                                case 1:
                                    stocks = stocks.Where(x => x.StockTotal > softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 2:
                                    stocks = stocks.Where(x => x.StockTotal < softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                                case 3:
                                    stocks = stocks.Where(x => x.StockTotal == softStockSearchFilterVM.selectedStockFilterValue);
                                    break;
                            }
                        }
                        if (rootExist == false) rootExist = true;
                    }
                    if (softStockSearchFilterVM.selectedStockTotalFilter > 0 && softStockSearchFilterVM.selectedStockTotalFilterValue != null)
                    {
                        if (rootExist == false)
                        {
                            switch (softStockSearchFilterVM.selectedStockTotalFilter)
                            {
                                case 1:
                                    stocks = query.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) > softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 2:
                                    stocks = query.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) < softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 3:
                                    stocks = query.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) == softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                            }
                        }
                        else
                        {
                            switch (softStockSearchFilterVM.selectedStockTotalFilter)
                            {
                                case 1:
                                    stocks = stocks.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) > softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 2:
                                    stocks = stocks.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) < softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                                case 3:
                                    stocks = stocks.Where(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal) == softStockSearchFilterVM.selectedStockTotalFilterValue);
                                    break;
                            }
                        }
                        if (rootExist == false) rootExist = true;
                    }
                    if (softStockSearchFilterVM.selectedHideStatusFilter.Count > 0)
                    {
                        foreach (var item in softStockSearchFilterVM.selectedHideStatusFilter)
                        {
                            IQueryable<SoftBranchProductStock> donhangstmp = null;
                            if (rootExist == false)
                            {
                                switch (item)
                                {
                                    case 1:
                                        donhangstmp = query.Where(x => x.shop_sanpham.hide == true || x.shop_sanpham.hide == null);
                                        break;
                                    default:
                                        donhangstmp = query.Where(x => x.shop_sanpham.hide == false);
                                        break;
                                }
                            }
                            else
                            {
                                switch (item)
                                {
                                    case 1:
                                        donhangstmp = stocks.Where(x => x.shop_sanpham.hide == true || x.shop_sanpham.hide == null);
                                        break;
                                    default:
                                        donhangstmp = stocks.Where(x => x.shop_sanpham.hide == false);
                                        break;
                                }
                            }
                            if (stocksFilter == null)
                                stocksFilter = donhangstmp;
                            else
                                stocksFilter = stocksFilter.Union(donhangstmp);
                        }
                        stocks = stocksFilter;
                        if (rootExist == false) rootExist = true;
                        stocksFilter = null;
                    }
                }
                else
                {
                    stocks = query;
                }
                totalRow = stocks.Count();
            }

            switch (softStockSearchFilterVM.sortBy)
            {
                case "productCode_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.masp);
                    break;
                case "productCode_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.masp);
                    break;
                case "productName_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.tensp);
                    break;
                case "productName_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.tensp);
                    break;
                case "stock_Des":
                    stocks = stocks.OrderByDescending(x => x.StockTotal);
                    break;
                case "stock_Asc":
                    stocks = stocks.OrderBy(x => x.StockTotal);
                    break;
                case "stockTotal_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal));
                    break;
                case "stockTotal_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.SoftBranchProductStocks.Sum(y => y.StockTotal));
                    break;
                case "priceBase_Des":
                    stocks = stocks.OrderByDescending(x => x.shop_sanpham.PriceBase);
                    break;
                case "priceBase_Asc":
                    stocks = stocks.OrderBy(x => x.shop_sanpham.PriceBase);
                    break;
                default:
                    stocks = stocks.OrderBy(x => x.shop_sanpham.tensp);
                    break;
            }
            return stocks.Skip(softStockSearchFilterVM.page * softStockSearchFilterVM.pageSize).Take(softStockSearchFilterVM.pageSize);
        }

        public double GetStockTotal(int productId, int branchId)
        {
            double result = 0;
            var model = DbContext.SoftBranchProductStocks.FirstOrDefault(x => x.ProductId == productId && x.BranchId == branchId);
            if (model != null)
                result = model.StockTotal.Value;
            return result;
        }

        public double GetStockTotalAll(int productId)
        {
            double result = 0;
            var models = DbContext.SoftBranchProductStocks.Where(x => x.ProductId == productId).ToList();
            if (models.Count > 0)
            {
                foreach (var item in models)
                {
                    if (item.StockTotal != null)
                        result += item.StockTotal.Value;
                }
            }
            return result;
        }

        public SoftBranchProductStock Init(int branchId, int productId)
        {
            var newStock = new SoftBranchProductStock();
            newStock.BranchId = branchId;
            newStock.ProductId = productId;
            newStock.StockTotal = 0;
            newStock.CreatedDate = DateTime.Now;
            newStock.CreatedBy = 0;
            return newStock;
        }
    }
}