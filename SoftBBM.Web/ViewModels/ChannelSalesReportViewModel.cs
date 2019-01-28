using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ChannelSalesRevenuesReportViewModel
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public double Sales { get; set; }
        public double Revenues { get; set; }
        public string NewDate { get; set; }
        public int NewDay { get; set; }
        public string NewDayStr { get; set; }
        public double Quantity { get; set; }
    }
    public class ChannelSalesRevenuesReportParamsViewModel
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int branchId { get; set; }
    }
    public class SalesRevenuesReportParamsViewModel
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int branchId { get; set; }
        public int channelId { get; set; }
    }
}