using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class SoftChannelProductPriceViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ChannelId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> PriceDiscount { get; set; }
        public Nullable<System.DateTime> StartDateDiscount { get; set; }
        public Nullable<System.DateTime> EndDateDiscount { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }

        public virtual ShopSanPhamViewModel shop_sanpham { get; set; }
        public virtual SoftChannelViewModel SoftChannel { get; set; }
    }
    public class SoftChannelProductPriceSearchViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ChannelId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> PriceDiscount { get; set; }
        public Nullable<System.DateTime> StartDateDiscount { get; set; }
        public Nullable<System.DateTime> EndDateDiscount { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
    }
    public class GetAllPriceChannelProductOutputViewModel
    {
        public int ProductId { get; set; }
        public int PriceBase { get; set; }
        public int PriceBaseOld { get; set; }
        public int PriceRef { get; set; }
        public List<PriceChannelViewModel> PriceChannels { get; set; }
        public int UpdateBy { get; set; }
    }
    public class PriceChannelViewModel
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> PriceDiscount { get; set; }
        public Nullable<System.DateTime> StartDateDiscount { get; set; }
        public Nullable<System.DateTime> EndDateDiscount { get; set; }
        public string Description { get; set; }
    }
}