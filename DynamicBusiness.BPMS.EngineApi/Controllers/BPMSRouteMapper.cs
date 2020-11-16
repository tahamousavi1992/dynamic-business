using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.Domain;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace DynamicBusiness.BPMS.EngineApi.Controllers
{
    public class BPMSRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            //it must add in dnnStartup but at this moment of codding it is not possible.
            //DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(BpmsRequiredAttribute), typeof(RequiredAttributeAdapter));

            System.Web.Http.GlobalConfiguration.Configuration.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(new MultipartFormatterSettings()));
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            //call from client http://localhost/API/BpmsApi/controllerName/actionName
            var route = mapRouteManager.MapHttpRoute("BpmsApi", "default", "{controller}/{action}", new { }, new[] { "DynamicBusiness.BPMS.EngineApi.Controllers" });
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