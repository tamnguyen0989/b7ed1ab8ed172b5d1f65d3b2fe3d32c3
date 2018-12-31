using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.ViewModels
{
    public class donhangchuyenphatvungViewModel
    {
        public int id { get; set; }
        public Nullable<double> kilogram { get; set; }
        public string mavung { get; set; }
        public Nullable<int> ship { get; set; }
    }
}