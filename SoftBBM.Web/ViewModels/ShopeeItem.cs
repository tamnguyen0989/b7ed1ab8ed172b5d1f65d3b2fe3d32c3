using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ShopeeItem
    {
        public long item_id { get; set; }
        public string item_sku { get; set; }
        public string name { get; set; }
        public List<ShopeeVariation> variations { get; set; }
        public float package_length { get; set; }
        public float package_width { get; set; }
        public float package_height { get; set; }
    }
    public class GetItemDetail
    {
        public ShopeeItem item { get; set; }
    }
    public class ShopeeVariation
    {
        public long variation_id { get; set; }
        public string variation_sku { get; set; }
        public string name { get; set; }
    }
}