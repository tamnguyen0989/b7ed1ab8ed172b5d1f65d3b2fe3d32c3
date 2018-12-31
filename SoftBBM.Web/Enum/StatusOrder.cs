using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Enum
{
    public enum StatusOrder
    {
        [Display(Name = "Chờ xử lý", ShortName = "Process")]
        Process = 1,
        [Display(Name = "Đang giao hàng", ShortName = "Shipped")]
        Shipped = 2,
        [Display(Name = "Hoàn thành", ShortName = "Done")]
        Done = 3,
        [Display(Name = "Hủy", ShortName = "Cancel")]
        Cancel = 4,
        [Display(Name = "Hoãn", ShortName = "Refund")]
        Refund = 5,
        [Display(Name = "Giao hàng thất bại", ShortName = "ShipCancel")]
        ShipCancel = 6
    }

}