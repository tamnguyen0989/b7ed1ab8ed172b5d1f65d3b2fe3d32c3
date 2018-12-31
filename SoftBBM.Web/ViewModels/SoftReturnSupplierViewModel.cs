using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftReturnSupplierViewModel
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string Description { get; set; }
        public int TotalQuantity { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }

        public ApplicationUserViewModel ApplicationUser { get; set; }
        public ApplicationUserViewModel ApplicationUser1 { get; set; }
        public SoftBranchViewModel SoftBranch { get; set; }
        public ICollection<SoftReturnSupplierDetailViewModel> SoftReturnSupplierDetails { get; set; }
        public SoftSupplierViewModel SoftSupplier { get; set; }
    }

    public class SoftReturnSupplierAddViewModel
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string Description { get; set; }
        public int TotalQuantity { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }

        public ICollection<SoftReturnSupplierDetailAddViewModel> SoftReturnSupplierDetails { get; set; }
    }

    public class SoftReturnSupplierFilterViewModel
    {
        public List<SoftSupplierViewModel> selectedSupplierFilters { get; set; }
        public int branchId { get; set; }
        public int supplierId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string filter { get; set; }
        public string sortBy { get; set; }
        public DateTime startDateFilter { get; set; }
        public DateTime endDateFilter { get; set; }
    }
}