using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TPWeb.Models;

namespace TPWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            HttpRuntime.Cache["Actors"] = new Actors(Server.MapPath(@"~/App_Data/Actors.txt"));
            HttpRuntime.Cache["Movies"] = new Movies(Server.MapPath(@"~/App_Data/Movies.txt"));
            HttpRuntime.Cache["Parutions"] = new Parutions(Server.MapPath(@"~/App_Data/Parutions.txt"));
        }
        protected void Session_Start()
        {
            // Keep the reference of the ContactsView in order to allow all the controllers and views have access to it
            // This allows all the session to have their own sorted contact list
            Session["ActorsView"] = new ActorsView((Actors)HttpRuntime.Cache["Actors"]);
            Session["MoviesView"] = new MoviesView((Movies)HttpRuntime.Cache["Movies"]);
        }
    }
}
