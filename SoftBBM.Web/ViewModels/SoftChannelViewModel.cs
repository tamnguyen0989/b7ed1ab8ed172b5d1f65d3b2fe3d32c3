using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftChannelViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Phải nhập Tên")]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}