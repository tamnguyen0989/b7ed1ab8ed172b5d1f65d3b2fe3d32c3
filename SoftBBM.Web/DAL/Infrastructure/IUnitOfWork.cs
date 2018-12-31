using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}