using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Models;

namespace SoftBBM.Web.Hubs
{
    public class ChatHub : Hub
    {
        public void send(int branchId)
        {

            Clients.All.broadcastMessage(branchId);
        }
        public void update_order_window(int userId)
        {

            Clients.All.broadcastMessage("update_offline_window",userId);
        }
    }
}