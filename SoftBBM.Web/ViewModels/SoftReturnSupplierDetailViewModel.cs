using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftReturnSupplierDetailViewModel
    {
        public int ReturnSupplierId { get; set; }
        public int ProductId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string ProductName { get; set; }

    }
    public class SoftReturnSupplierDetailAddViewModel
    {
        public int Id { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}