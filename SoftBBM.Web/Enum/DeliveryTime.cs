using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Enum
{
    public enum DeliveryTime
    {
        [Display(Name = "08h – 17h giờ hành chánh")]
        HCM08h17h = 1,
            [Display(Name = "17h – 22h ngoài giờ hành chánh")]
        HCM17h22h = 2,
            [Display(Name = "Ngày chủ nhật")]
        HCMSunday = 3,
            [Display(Name = "Bất kỳ giờ nào trong ngày")]
        HCMAnytime = 4,
            [Display(Name = "Nhanh, chỉ trong 3 tiếng")]
        HCMFast3hours = 5
    }
}