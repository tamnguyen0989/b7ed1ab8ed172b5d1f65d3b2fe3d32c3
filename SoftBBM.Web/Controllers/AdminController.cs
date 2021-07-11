using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftBBM.Web.Controllers
{
    public class AdminController : Controller
    {
        ISoftChannelRepository _softChannelRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftBranchRepository _softBranchRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        ISoftStockRepository _softStockRepository;
        IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork, ISoftChannelRepository softChannelRepository, IShopSanPhamRepository shopSanPhamRepository, ISoftChannelProductPriceRepository softChannelProductPriceRepository, ISoftBranchRepository softBranchRepository, ISoftStockRepository softStockRepository)
        {
            _unitOfWork = unitOfWork;
            _softChannelRepository = softChannelRepository;
            _shopSanPhamRepository = shopSanPhamRepository;
            _softBranchRepository = softBranchRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _softStockRepository = softStockRepository;
        }
        // GET: Admin
        public ActionResult Index()
        {
            //migration channelprice and product
            //var products = _shopSanPhamRepository.GetAllIds();
            //var channels = _softChannelRepository.GetAllIds();
            //foreach (var product in products.ToList())
            //{
            //    foreach (var channel in channels.ToList())
            //    {
            //        var price = _softChannelProductPriceRepository.GetSingleByCondition(x => x.ProductId == product && x.ChannelId == channel);
            //        if (price == null)
            //        {
            //            var newPrice = new SoftChannelProductPrice();
            //            newPrice.ProductId = product;
            //            newPrice.ChannelId = channel;
            //            newPrice.Price = 0;
            //            newPrice.CreatedDate = DateTime.Now;
            //            _softChannelProductPriceRepository.Add(newPrice);
            //            _unitOfWork.Commit();
            //        }
            //    }
            //}

            //migration branch,stock and product
            //var products = _shopSanPhamRepository.GetAllIds();
            //var branches = _softBranchRepository.GetAllIds();
            //foreach (var product in products.ToList())
            //{
            //    foreach (var branch in branches.ToList())
            //    {
            //        var price = _softStockRepository.GetSingleByCondition(x => x.ProductId == product && x.BranchId == branch);
            //        if (price == null)
            //        {
            //            var newstock = new SoftBranchProductStock();
            //            newstock.ProductId = product;
            //            newstock.BranchId = branch;
            //            newstock.StockTotal = 0;
            //            newstock.CreatedDate = DateTime.Now;
            //            _softStockRepository.Add(newstock);
            //            _unitOfWork.Commit();
            //        }
            //    }
            //}
            long version = 0;
            long.TryParse(ConfigurationSettings.AppSettings.Get("version").ToString(), out version);

            return View(version);
        }
    }
}
