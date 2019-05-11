using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Enum
{
    public enum ChannelEnum
    {
        [Display(Name = "Cửa hàng")]
        CHA = 1,
        [Display(Name = "Online")]
        ONL = 2,
        [Display(Name = "Lazada")]
        LZD = 3,
        [Display(Name = "Shopee")]
        SPE = 4
    }
}