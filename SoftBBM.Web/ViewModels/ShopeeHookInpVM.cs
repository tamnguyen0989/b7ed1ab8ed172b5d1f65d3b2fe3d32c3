using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ShopeeHookInpVM
    {
        public long Code { get; set; }
        public long ShopId { get; set; }
        public long TimeStamp { get; set; }
        public ShopeeHookData Data { get; set; }
    }
    public class ShopeeHookData
    {
        public string Ordersn { get; set; }
        public string Status { get; set; }
        public long Update_time { get; set; }
        public string TrackingNo { get; set; }
    }
}