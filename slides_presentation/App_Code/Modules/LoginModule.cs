using mahamudra.it.slides.dal;
using Nancy;
using Nancy.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for HomeModule
/// </summary>
public class LoginModule : NancyModule
{
    public LoginModule(IRootPathProvider pathProvider)
    {
 
        Before += ctx =>
        {
            if (ctx.Request.Cookies.ContainsKey("flex"))
            {
                var myId = ctx.Request.Cookies["flex"];
                var id_user = new EncryptHelper(AppConfig.Provider,
                                                    Xmlconfig.get(
                                                      "cryptokey",
                                                      pathProvider.GetRootPath()).Value).decrypt(myId);

                if (!string.IsNullOrEmpty(id_user))
                {
                    return Response.AsRedirect("/slides"); //redirects to items
                }
            }

            return null; //it means you can carry on!!!!
        };

 

        Get["/login"] = _ =>
        {
            var model = new
            {
                title = "Mobile Day 2014 - Reveal.js - The HTML Presentation Framework"
            };

            return View["login", model];
        };

        Post["/login"] = _ =>
        {
            dynamic model = null;

            var us = new User
            {
                UserName = Request.Form.username,
                Password = Request.Form.password,
            };

            //first of all validate data

            if (string.IsNullOrEmpty(us.UserName) || string.IsNullOrEmpty(us.Password))
            {
                model = new
                {
                    title = "Mobile Day 2014 - Reveal.js - The HTML Presentation Framework",
                    user = us,
                    success = false,
                    messages = new List<string> { "Please, provide username and password" }
                };
            }
            else
            {
                us.Password = new EncryptHelper(AppConfig.Provider, Xmlconfig.get("cryptokey",
                                                                     pathProvider.GetRootPath()).Value).encrypt(us.Password); //real_password 

                var ut_res = UsersRepository.authenticate(us);

                if (ut_res != null)
                {
                    var myEncryptedId = new EncryptHelper(AppConfig.Provider, Xmlconfig.get("cryptokey",
                                                     pathProvider.GetRootPath()).Value).encrypt(ut_res.Id.ToString() ); //encrypt 4 cookie

                    //create cookie, http only with encrypted id user and add it to the current response
                    var mc = new NancyCookie("flex", myEncryptedId, true);
                    
                    var res = Response.AsRedirect("/slides");
                    res.WithCookie(mc);
                    return res;
                }
                else
                {
                    model = new
                    {
                        title = "Mobile Day 2014 - Reveal.js - The HTML Presentation Framework",
                        user = us,
                        success = false,
                        messages = new List<string> { "Wrong username or password" }
                    };
                }
            }

            return View["login", model];
        };
    
    }
}
