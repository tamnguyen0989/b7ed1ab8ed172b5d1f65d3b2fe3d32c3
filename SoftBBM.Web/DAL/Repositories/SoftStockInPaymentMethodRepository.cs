﻿using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftStockInPaymentMethodRepository : IRepository<SoftStockInPaymentMethod>
    {

    }
    public class SoftStockInPaymentMethodRepository : RepositoryBase<SoftStockInPaymentMethod>, ISoftStockInPaymentMethodRepository
    {
        public SoftStockInPaymentMethodRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}