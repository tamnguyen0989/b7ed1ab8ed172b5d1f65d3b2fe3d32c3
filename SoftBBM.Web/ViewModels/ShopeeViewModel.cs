using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ShopeeViewModel
    {
    }

    public class ShopInfo
    {
        public string status { get; set; }
        public int item_limit { get; set; }
        public int disable_make_offer { get; set; }
        public List<string> videos { get; set; }
        public string country { get; set; }
        public string shop_description { get; set; }
        public int shop_id { get; set; }
        public string request_id { get; set; }
        public List<string> images { get; set; }
        public string shop_name { get; set; }
        public bool enable_display_unitno { get; set; }
    }

    public class UpdateStockRes
    {
        public UpdateStockResItem item;
        public string request_id;
    }

    public class UpdateStockResItem
    {
        public int item_id;
        public int modified_time;
        public int stock;
    }
}