using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftBranchViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Phải nhập Tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phải nhập Code")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Phải nhập Địa chỉ")]
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
    }
    public class SoftBranchLoginViewModel
    {
        public ApplicationUserViewModel ApplicationUser { get; set; }
        public IEnumerable<SoftBranchViewModel> SoftBranchs { get; set; }
    }
    public class SoftBranchImportSampleViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
    
}