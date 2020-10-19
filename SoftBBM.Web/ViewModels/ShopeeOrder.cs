using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{

    public class ShopeeOrder
    {
        public List<ShopeeOrderDetailItem> items { get; set; }
        public ShopeeRecipientAddress recipient_address { get; set; }
        public string shipping_carrier { get; set; }
        public string note { get; set; }
        public string total_amount { get; set; }
        public string message_to_seller { get; set; }
        public string tracking_no { get; set; }
        public string order_status { get; set; }
        public double create_time { get; set; }
        public bool cod { get; set; }
    }

    public class GetOrderDetailsRes
    {
        public List<string> errors { get; set; }
        public List<ShopeeOrder> orders { get; set; }
    }

    public class ShopeeOrderDetailItem
    {
        public long item_id { get; set; }
        public string item_sku { get; set; }
        public string variation_sku { get; set; }
        public int variation_quantity_purchased { get; set; }
        public string variation_discounted_price { get; set; }
        public string variation_original_price { get; set; }
        public string item_name { get; set; }
        public string variation_name { get; set; }
        public double weight { get; set; }
    }
    public class ShopeeRecipientAddress
    {
        public string full_address { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public string state { get; set; } //tp,tỉnh
        public string city { get; set; } //quận, huyện

    }
    public class ShopeeTimeSlot
    {
        public List<ShopeePickupTime> pickup_time { get; set; }
        public string request_id { get; set; }
    }

    public class ShopeePickupTime
    {
        public string pickup_time_id { get; set; }
        public long date { get; set; }
    }

    public class ShopeeInit
    {
        public string tracking_number { get; set; }
        public string request_id { get; set; }
    }

    public class ShopeeOrderLogistics
    {
        public ShopeeLogistics logistics { get; set; }
        public string request_id { get; set; }
    }
    public class ShopeeLogistics
    {
        public ShopeeRecipientSortCode recipient_sort_code { get; set; }
        public ShopeeSenderSortCode sender_sort_code { get; set; }
    }
    public class ShopeeRecipientSortCode
    {
        public string first_recipient_sort_code { get; set; }
        public string second_recipient_sort_code { get; set; }
        public string third_recipient_sort_code { get; set; }
    }
    public class ShopeeSenderSortCode
    {
        public string first_sender_sort_code { get; set; }
        public string second_sender_sort_code { get; set; }
        public string third_sender_sort_code { get; set; }
    }

    public class OrderGetOrdersList
    {
        public string ordersn { get; set; }
        public string order_status { get; set; }
        public int update_time { get; set; }
    }

    public class ShopeeGetOrdersList
    {
        public List<OrderGetOrdersList> orders { get; set; }
    }
}