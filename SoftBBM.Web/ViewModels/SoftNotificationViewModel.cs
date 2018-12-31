using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftNotificationViewModel
    {
        public int Id { get; set; }
        public Nullable<int> FromBranchId { get; set; }
        public Nullable<int> ToBranchId { get; set; }
        public string Message { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public string Url { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public string CreatedDateConvert { get; set; }
        public Nullable<int> StockinId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public SoftBranchViewModel SoftBranch { get; set; }
        public SoftBranchViewModel SoftBranch1 { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
        public SoftStockInViewModel SoftStockIn { get; set; }
    }
}