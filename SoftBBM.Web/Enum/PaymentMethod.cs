using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Enum
{
    public enum PaymentMethod
    {
        [Display(Name = "Tiền mặt")]
        Cash = 1,
        [Display(Name = "Chuyển khoản")]
        BankTransfer = 2,
        [Display(Name = "Thu hộ")]
        COD = 3,
        [Display(Name = "Thanh toán trực tuyến")]
        OnlinePayment = 4,
        [Display(Name = "Bằng thẻ ngân hàng khi nhận hàng")]
        BankCardOnDelivery = 5,
        [Display(Name = "QRCode")]
        QRCode = 6,
        [Display(Name = "Quẹt thẻ")]
        QuetThe = 7
    }
}