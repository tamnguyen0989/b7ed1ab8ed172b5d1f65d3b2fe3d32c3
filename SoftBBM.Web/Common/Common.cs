using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.Common
{
    public static class CommonClass
    {
        public const string UNPAID = "UNPAID";
        public const string READY_TO_SHIP = "READY_TO_SHIP";
        public const string RETRY_SHIP = "RETRY_SHIP";
        public const string SHIPPED = "SHIPPED";
        public const string TO_CONFIRM_RECEIVE = "TO_CONFIRM_RECEIVE";
        public const string IN_CANCEL = "IN_CANCEL";
        public const string CANCELLED = "CANCELLED";
        public const string TO_RETURN = "TO_RETURN";
        public const string COMPLETED = "COMPLETED";
    }
}