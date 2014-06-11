using Nancy;
using Nancy.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for HomeModule
/// </summary>
public class LogoutModule : NancyModule
{
    public LogoutModule(IRootPathProvider pathProvider)
    {

        Get["/logout"] = _ =>
        {
            //delete cookie, http only with encrypted username and add it to the current response
            var mc = new NancyCookie("flex", "", true);
            mc.Expires = DateTime.Now.Subtract(new TimeSpan(100000)); //deletes the cookie
            var res = Response.AsRedirect("/");
            res.WithCookie(mc);
            return res;
        };

    }
}
