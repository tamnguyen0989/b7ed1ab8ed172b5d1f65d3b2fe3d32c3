using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Enum
{
    public enum SystemError
    {
        [Display(Name = "StockIn")]
        STOCK_IN = 1,
        [Display(Name = "Shopee")]
        SHOPEE = 2,
        [Display(Name = "Order")]
        ORDER = 3,
    }
}