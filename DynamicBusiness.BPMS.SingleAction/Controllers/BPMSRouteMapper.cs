using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace DynamicBusiness.BPMS.SingleAction
{
    public class BPMSRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            //call from client http://localhost/API/BpmsSingleActionApi/controllerName/actionName
            var route = mapRouteManager.MapHttpRoute("BpmsSingleActionApi", "default", "{tabmid}/{controller}/{action}", new { }, new[] { "DynamicBusiness.BPMS.SingleAction.Controllers" });
            foreach (Route r in route)
            {
                r.RouteHandler = new MyHttpControllerRouteHandler();
            }
        }

        public class MyHttpControllerHandler : HttpControllerHandler, IRequiresSessionState
        {
            public MyHttpControllerHandler(RouteData routeData) : base(routeData)
            {
            }
        }
        public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
        {
            protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                return new MyHttpControllerHandler(requestContext.RouteData);
            }
        }

    }
}
