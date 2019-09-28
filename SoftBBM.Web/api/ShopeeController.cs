using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoftBBM.Web.api
{
    [RoutePrefix("api/shopee")]
    public class ShopeeController : ApiController
    {
        ISoftStockRepository _softStockRepository;
        IShopSanPhamRepository _shopSanPhamRepository;
        ISoftChannelProductPriceRepository _softChannelProductPriceRepository;
        IShopSanPhamStatusRepository _shopSanPhamStatusRepository;
        ISoftChannelRepository _softChannelRepository;
        ISoftBranchRepository _softBranchRepository;
        IdonhangRepository _donhangRepository;
        IdonhangctRepository _donhangctRepository;
        IshopbientheRepository _shopbientheRepository;
        IUnitOfWork _unitOfWork;

        public ShopeeController(ISoftStockRepository softStockRepository, IShopSanPhamRepository shopSanPhamRepository, IUnitOfWork unitOfWork, ISoftChannelProductPriceRepository softChannelProductPriceRepository, IShopSanPhamStatusRepository shopSanPhamStatusRepository, ISoftChannelRepository softChannelRepository, ISoftBranchRepository softBranchRepository, IdonhangRepository donhangRepository, IdonhangctRepository donhangctRepository, IshopbientheRepository shopbientheRepository)
        {
            _shopSanPhamRepository = shopSanPhamRepository;
            _softChannelProductPriceRepository = softChannelProductPriceRepository;
            _softStockRepository = softStockRepository;
            _shopSanPhamStatusRepository = shopSanPhamStatusRepository;
            _softChannelRepository = softChannelRepository;
            _softBranchRepository = softBranchRepository;
            _donhangRepository = donhangRepository;
            _donhangctRepository = donhangctRepository;
            _shopbientheRepository = shopbientheRepository;
            _unitOfWork = unitOfWork;
        }
        

    }
}