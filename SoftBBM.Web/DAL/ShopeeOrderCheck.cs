using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Quartz;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Enum;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;

namespace SoftBBM.Web.DAL
{
    public class ShopeeOrderCheck : IJob
    {
        IUnitOfWork _unitOfWork;
        IShopeeRepository _shopeeRepository;
        IdonhangRepository _donhangRepository;
        ISoftPointUpdateLogRepository _softPointUpdateLogRepository;
        ISystemLogRepository _systemLogRepository;
        public ShopeeOrderCheck(IUnitOfWork unitOfWork,
            IdonhangRepository donhangRepository,
            ISoftPointUpdateLogRepository softPointUpdateLogRepository,
            IShopeeRepository shopeeRepository,
            ISystemLogRepository systemLogRepository)
        {
            _unitOfWork = unitOfWork;
            _shopeeRepository = shopeeRepository;
            _donhangRepository = donhangRepository;
            _softPointUpdateLogRepository = softPointUpdateLogRepository;
            _systemLogRepository = systemLogRepository;
        }

        public void Execute(IJobExecutionContext context)
        {
            var quantity = 2;
            var shopeeOrderLastDay = new List<OrderGetOrdersList>();
            try
            {
                var log = new SystemLog();
                log.InitSystemLog(0, "Start_Job", "ShopeeOrderCheck", "", (int)SystemError.SHOPEE, "Shopee");
                _systemLogRepository.Add(log);
                _unitOfWork.Commit();

                var shopeeOrderLastDay_ReadyToShip = _shopeeRepository.GetOrdersListLastWithHour(quantity, "READY_TO_SHIP").orders;
                if (shopeeOrderLastDay_ReadyToShip != null)
                    shopeeOrderLastDay.AddRange(shopeeOrderLastDay_ReadyToShip);

                var shopeeOrderLastDay_Unpaid = _shopeeRepository.GetOrdersListLastWithHour(quantity, "UNPAID").orders;
                if (shopeeOrderLastDay_Unpaid != null)
                    shopeeOrderLastDay.AddRange(shopeeOrderLastDay_Unpaid);

                if (shopeeOrderLastDay.Count > 0)
                {
                    foreach (var orderSPE in shopeeOrderLastDay)
                    {
                        var orderDB = _donhangRepository.GetSingleByCondition(x => x.OrderIdShopeeApi == orderSPE.ordersn);
                        if (orderDB == null)
                        {
                            DateTime? updateDate = null;
                            if (orderSPE.update_time > 0)
                                updateDate = UtilExtensions.UnixTimeStampToDateTime(orderSPE.update_time);
                            _shopeeRepository.AddOrderLack(orderSPE.ordersn, orderSPE.order_status, updateDate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new SystemLog();
                log.InitSystemLog(0, "Error_Job", "ShopeeOrderCheck", JsonConvert.SerializeObject(ex), (int)SystemError.SHOPEE, "Shopee");
                _systemLogRepository.Add(log);
                _unitOfWork.Commit();
            }
        }
    }
}