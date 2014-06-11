using mahamudra.it.slides.dal;
using Nancy;
using Nancy.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// 
/// </summary>
public class UserModule : NancyModule
{
    public UserModule(IRootPathProvider pathProvider)
    {
        User me = null; //add the user  as a property to the model :)

        Before += ctx =>
        {
            if (ctx.Request.Cookies.ContainsKey("flex"))
            {
                var myId = ctx.Request.Cookies["flex"];
                var id_u = new EncryptHelper(AppConfig.Provider,
                                                    Xmlconfig.get(
                                                      "cryptokey",
                                                      pathProvider.GetRootPath()).Value).decrypt(myId);

                if (!string.IsNullOrEmpty(id_u))
                {
                    me = UsersRepository.getById(Convert.ToInt32(id_u));
                    if (me != null)
                    {
                        return null; //it means you can carry on!!!!
                    }

                }
            }

            var res = new Response();
            res.StatusCode = HttpStatusCode.Forbidden;
            return res;
        };

        Get["/users"] = _ =>
        {
            var model = new
            {
                title = "Mobile Day 2014",
                users = UsersRepository.getOrderedByName(),
                me = me
            };

            if (!me.IsAdmin) //check if I am an admin  
            {
                var res = new Response();
                res.StatusCode = HttpStatusCode.Forbidden;
                return res;
            }
            else
                return View["users", model];
        };

        Get[@"/users/{id:int}"] = parameters =>
        {
            //*important
            int id = parameters.id; //I'm forcing the right conversion
            var puser = UsersRepository.getById(id);

            if (puser == null) //the user does not exists
            {
                var res = new Response();
                res.StatusCode = HttpStatusCode.NotFound;
                return res;
            }

            var model = new
            {
                title = "Mobile Day 2014",
                user = puser,
                me = me
            };

            if ((me.Id != id) && !me.IsAdmin) //check if I am not an admin and I'm changing someone's else profile
            {
                var res = new Response();
                res.StatusCode = HttpStatusCode.Forbidden;
                return res;
            }

            return View["single_user", model];
        };

        Post["/users/{id:int}"] = parameters =>
        {
            //*important
            int id = parameters.id;

            dynamic model = null;
            //check first if I'm a simple editor, not an Admin and I want to change someone's else profile
            if ((me.Id != id) && !me.IsAdmin)
            {
                var res = new Response();
                res.StatusCode = HttpStatusCode.Forbidden;
                return res;
            }

            var us = new User
            {
                Id = id,
                UserName = Request.Form.username,
                Password = Request.Form.password,
                SimpleRoles = Request.Form.hr
            };

            if ((me.Id == id) && me.IsAdmin && !us.SimpleRoles.Contains("0"))
            {
                model = new
                {
                    title = "Mobile Day 2014",
                    user = us,
                    me = me,
                    success = false,
                    messages = new List<string> { "You can't quit being an admin!" }
                };
            }
            else
            {
                var rip_password = Request.Form.repeate_password;

                //first of all validate data
                if ((us.Password != rip_password) && (!string.IsNullOrEmpty(us.Password)))
                {
                    model = new
                    {
                        title = "Mobile Day 2014",
                        user = us,
                        me = me,
                        success = false,
                        messages = new List<string> { "Please, the passwords must match" }
                    };
                }
                else
                {
                    //first of all validate data
                    if (string.IsNullOrEmpty(us.UserName) || (string.IsNullOrEmpty(us.SimpleRoles) && me.IsAdmin))
                    {
                        model = new
                        {
                            title = "Mobile Day 2014",
                            user = us,
                            me = me,
                            success = false,
                            messages = new List<string> { "Please, provide username and at least one role." }
                        };
                    }
                    else
                    {
                        var isChangePassword = false;
                        //Am I trying to change the password?
                        if (!string.IsNullOrEmpty(us.Password))
                        {

                            us.Password = new EncryptHelper(AppConfig.Provider, Xmlconfig.get("cryptokey",
                                                                                  pathProvider.GetRootPath()).Value).encrypt(us.Password); //real_password 

                            isChangePassword = true;
                        }

                        if (me.IsAdmin) //only an admin can change the roles
                        {
                            us = UsersRepository.insertIfAdmin(us, isChangePassword);
                        }
                        else
                        {
                            us = UsersRepository.insert(us, isChangePassword);
                        }

                        if (us != null)
                        {
                            model = new
                            {
                                title = "Mobile Day 2014",
                                user = us,
                                me = me,
                                success = true,
                                messages = new List<string> { "User modified succesfully" }
                            };
                        }
                        else
                        {
                            model = new
                            {
                                title = "Mobile Day 2014",
                                user = us,
                                me = me,
                                success = false,
                                messages = new List<string> { "Sorry, we couldn't find the user specified!" }
                            };
                        }
                    }
                }
            }

            return View["single_user", model];
        };
    }
}
