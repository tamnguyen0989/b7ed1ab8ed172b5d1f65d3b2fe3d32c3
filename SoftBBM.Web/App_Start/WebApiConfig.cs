using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using SoftBBM.Web.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SoftBBM.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.Formatters.JsonPreserveReferences();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
            new DefaultContractResolver { IgnoreSerializableAttribute = true };
            config.MessageHandlers.Add(new AuthenticationHandler());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
