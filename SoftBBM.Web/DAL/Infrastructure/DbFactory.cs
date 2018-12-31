using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private SoftBBMDbContext dbContext;

        public SoftBBMDbContext Init()
        {
            return dbContext ?? (dbContext = new SoftBBMDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}