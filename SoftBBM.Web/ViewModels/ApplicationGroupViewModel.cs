using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ApplicationGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Status { get; set; }

        public IEnumerable<ApplicationRoleViewModel> Roles { get; set; }
        public List<ApplicationRoleViewModel> OrderRoles { get; set; }
        public List<ApplicationRoleViewModel> StockInRoles { get; set; }
        public List<ApplicationRoleViewModel> StockOutRoles { get; set; }
        public List<ApplicationRoleViewModel> BookRoles { get; set; }
        public List<ApplicationRoleViewModel> BookBranchRoles { get; set; }
        public List<ApplicationRoleViewModel> ProductRoles { get; set; }
        public List<ApplicationRoleViewModel> AdjustmentStockRoles { get; set; }
        public List<ApplicationRoleViewModel> StampRoles { get; set; }
        public List<ApplicationRoleViewModel> BranchRoles { get; set; }
        public List<ApplicationRoleViewModel> SupplierRoles { get; set; }
        public List<ApplicationRoleViewModel> ChannelRoles { get; set; }
        public List<ApplicationRoleViewModel> UserRoles { get; set; }
        public List<ApplicationRoleViewModel> OwnRoles { get; set; }
        public List<ApplicationRoleViewModel> GroupRoles { get; set; }
        public List<ApplicationRoleViewModel> CustomerRoles { get; set; }
        public List<ApplicationRoleViewModel> ProductCategoryRoles { get; set; }
        public List<ApplicationRoleViewModel> ReturnSupplierRoles { get; set; }
    }
}