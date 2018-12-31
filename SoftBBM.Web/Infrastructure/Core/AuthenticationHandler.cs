using SoftBBM.Web.DAL.Infrastructure;
using SoftBBM.Web.DAL.Repositories;
using SoftBBM.Web.Infrastructure.Extensions;
using SoftBBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SoftBBM.Web.Infrastructure.Core
{
    public class CredentialChecker
    {
        public ApplicationUser CheckCredential(string username, string password)
        {
            using (var ctx = new SoftBBMDbContext())
            {
                return ctx.ApplicationUsers.Where(un => un.UserName == username && un.Password == password && un.Status == false).FirstOrDefault();
            }
        }
    }

    public class AuthenticationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var tokens = request.Headers.GetValues("Authorization").FirstOrDefault();
                if (tokens != null)
                {
                    //var tokensTmp = tokens.Split(' ');
                    byte[] data = Convert.FromBase64String(tokens);
                    string decodedString = Encoding.UTF8.GetString(data);
                    string[] tokensValues = decodedString.Split(':');

                    ApplicationUser ObjUser = new CredentialChecker().CheckCredential(tokensValues[0],tokensValues[1].Base64Encode());
                    if (ObjUser != null)
                    {
                        string[] roles;
                        using (var ctx = new SoftBBMDbContext())
                        {
                            var query = from ur in ctx.ApplicationUserRoles
                                        join r in ctx.ApplicationRoles
                                        on ur.RoleId equals r.Id
                                        where ur.UserId == ObjUser.Id
                                        select r.Name;
                            roles = query.ToArray();
                        }
                        IPrincipal principal = new GenericPrincipal(new GenericIdentity(ObjUser.UserName), roles);
                        Thread.CurrentPrincipal = principal;
                        HttpContext.Current.User = principal;
                    }
                    else
                    {
                        //The user is unauthorize and return 401 status  
                        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        var tsc = new TaskCompletionSource<HttpResponseMessage>();
                        tsc.SetResult(response);
                        return tsc.Task;
                    }
                }
                else
                {
                    //Bad Request request because Authentication header is set but value is null  
                    var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return tsc.Task;
                }
                return base.SendAsync(request, cancellationToken);
            }
            catch
            {
                //User did not set Authentication header  
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
        }

    }
}