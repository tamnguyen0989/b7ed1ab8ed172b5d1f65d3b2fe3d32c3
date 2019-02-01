using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class ExportPriceViewModel
    {
        public int id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int PriceBaseOld { get; set; }
        public int PriceBase { get; set; }
        public int PriceAvg { get; set; }
        public int PriceWholesale { get; set; }
        public int PriceChannel { get; set; }
    }

    public class ExportPriceNoIdViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int PriceOld { get; set; }
        public int PriceBase { get; set; }
        public int PriceAvg { get; set; }
        public int PriceWholesale { get; set; }
        public int PriceChannel { get; set; }
    }

    public class ExportPriceParamsDetail
    {
        public int id { get; set; }
    }

    public class ExportPriceParams
    {
        public IEnumerable<ExportPriceParamsDetail> ExportPriceParamsDetails { get; set; }
    }

}