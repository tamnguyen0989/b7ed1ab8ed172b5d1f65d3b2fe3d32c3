using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface ISoftNotificationRepository : IRepository<SoftNotification>
    {
        IQueryable<SoftNotification> GetAllPaging(int page, int pageSize,int branchId, out int totalRow, out int totalIsRead);
    }
    public class SoftNotificationRepository : RepositoryBase<SoftNotification>, ISoftNotificationRepository
    {
        public SoftNotificationRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<SoftNotification> GetAllPaging(int page, int pageSize, int branchId, out int totalRow, out int totalIsRead)
        {
            var query = from d in DbContext.SoftNotifications
                        select d;
            query = query.Where(x => x.ToBranchId == branchId);
            IQueryable<SoftNotification> softNotifications = null;
            totalIsRead = query.Where(x => x.IsRead == false).Count();
            softNotifications = query;
            totalRow = softNotifications.Count();
            softNotifications = softNotifications.OrderByDescending(x => x.Id);
            return softNotifications.Skip(page * pageSize).Take(pageSize);
        }
    }
}