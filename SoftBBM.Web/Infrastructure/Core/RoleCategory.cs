using SoftBBM.Web.Models;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Infrastructure.Core
{
    public class RoleCategory
    {
        public IEnumerable<ApplicationRoleViewModel> OrderRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> StockInRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> StockOutRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> BookRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> BookBranchRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> ProductRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> AdjustmentStockRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> StampRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> BranchRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> SupplierRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> ChannelRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> UserRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> OwnRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> GroupRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> CustomerRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> ProductCategoryRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> ReturnSupplierRoles { get; set; }
        public IEnumerable<ApplicationRoleViewModel> ReportRoles { get; set; }
    }
}