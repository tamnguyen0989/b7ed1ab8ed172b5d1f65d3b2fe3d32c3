using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Enum
{
    public enum BranchEnum
    {
        [Display(Name = "TVK")]
        TVK = 1,
        [Display(Name = "Kho chính")]
        KHO_CHINH = 2,
    }
}