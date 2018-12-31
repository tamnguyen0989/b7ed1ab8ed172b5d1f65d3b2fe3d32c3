using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ApplicationRoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> CategoryId { get; set; }

        public ApplicationRoleCategoryViewModel ApplicationRoleCategory { get; set; }
    }
}