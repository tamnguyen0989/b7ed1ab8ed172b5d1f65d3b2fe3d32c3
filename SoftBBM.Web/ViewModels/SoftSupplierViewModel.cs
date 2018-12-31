using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftSupplierViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Tên phải nhập")]
        public string Name { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<int> VatId { get; set; }
        public string AccBank { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> Prioty { get; set; }
        public string Vat { get; set; }
        public SoftSupplierVatStatuViewModel SoftSupplierVatStatu { get; set; }
    }
}