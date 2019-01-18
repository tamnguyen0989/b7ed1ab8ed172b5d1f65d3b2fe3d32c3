using SoftBBM.Web.Enum;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Infrastructure.Extensions
{
    public static class UtilExtensions
    {
        public static string ConvertPaymentMethod(int value)
        {
            string result = "";
            switch (value)
            {
                case (int)PaymentMethod.Cash:
                    result = "Tiền mặt";
                    break;
                case (int)PaymentMethod.COD:
                    result = "Thu hộ";
                    break;
                case (int)PaymentMethod.BankTransfer:
                    result = "Chuyển khoản";
                    break;
                case (int)PaymentMethod.OnlinePayment:
                    result = "Thanh toán trực tuyến";
                    break;
                case (int)PaymentMethod.BankCardOnDelivery:
                    result = "Bằng thẻ ngân hàng khi nhận hàng";
                    break;
            }
            return result;
        }
        public static string ConvertDeliveryTime(int value)
        {
            string result = "";
            switch (value)
            {
                case (int)DeliveryTime.HCM08h17h:
                    result = "08h – 17h giờ hành chánh";
                    break;
                case (int)DeliveryTime.HCM17h22h:
                    result = "17h – 22h ngoài giờ hành chánh";
                    break;
                case (int)DeliveryTime.HCMSunday:
                    result = "Ngày chủ nhật";
                    break;
                case (int)DeliveryTime.HCMAnytime:
                    result = "Bất kỳ giờ nào trong ngày";
                    break;
                case (int)DeliveryTime.HCMFast3hours:
                    result = "Nhanh, chỉ trong 3 tiếng";
                    break;
            }
            return result;
        }
        public static string ConvertDeliveryMethod(int value)
        {
            string result = "";
            switch (value)
            {
                case (int)PaymentMethod.Cash:
                    result = "Tiền mặt";
                    break;
                case (int)PaymentMethod.BankTransfer:
                    result = "Chuyển khoản";
                    break;
                case (int)PaymentMethod.OnlinePayment:
                    result = "Thanh toán trực tuyến";
                    break;
                case (int)PaymentMethod.BankCardOnDelivery:
                    result = "Bằng thẻ ngân hàng khi nhận hàng";
                    break;
            }
            return result;
        }
        public static SoftBranchProductStock InitStock(int branchId, int productId)
        {
            var model = new SoftBranchProductStock();
            model.BranchId = branchId;
            model.ProductId = productId;
            model.CreatedDate = DateTime.Now;
            return model;
        }
        public static string ConvertDate(DateTime source)
        {
            var date = DateTime.Today;
            string result = "";
            if (source.Day == date.Day && source.Month == date.Month && source.Year == date.Year)
                result = "Hôm nay " + source.ToString("HH:mm");
            else if (source.Day == date.Day - 1 && source.Month == date.Month && source.Year == date.Year)
                result = "Hôm qua " + source.ToString("HH:mm");
            else result = source.ToString("dd/MM/yyyy HH:mm").Replace("-", "/");
            return result;
        }
        public static DateTime ConvertStartDate(DateTime src)
        {
            var result = new DateTime(src.Year, src.Month, src.Day, 0, 0, 0);
            return result;
        }
        public static DateTime ConvertEndDate(DateTime src)
        {
            src = new DateTime(src.Year, src.Month, src.Day, 0, 0, 0);
            var result = src.AddDays(1).AddTicks(-1);
            return result;
        }
        public static int GetPriceWholesaleByPriceAvgOnl(int? priceAvg, int? priceOnl)
        {
            double result = 0;
            priceAvg = priceAvg == null ? 0 : priceAvg.Value;
            priceOnl = priceOnl == null ? 0 : priceOnl.Value;
            result = (priceOnl.Value - priceAvg.Value) / 5.3 + priceAvg.Value;
            result = Math.Ceiling(result / 100);
            result = result * 100;
            return (int)result;
        }
    }
}