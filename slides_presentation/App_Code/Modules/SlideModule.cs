using Nancy;
using Nancy.Cookies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Drawing;
using mahamudra.it.slides.dal;

/// <summary>
/// Summary description for HomeModule
/// </summary>
public class SlideModule : NancyModule
{
    public SlideModule(IRootPathProvider pathProvider)
    {
        User me = null; //add the user  as a property to the model :)


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
                    me = UsersRepository.getById(Convert.ToInt32(id_user));
                    return null; //it means you can carry on!!!!
                }
            }

            var res = new Response();
            res.StatusCode = HttpStatusCode.Forbidden;
            return res;
        };

        Get["/Slides"] = _ =>
        {
            var model = new
            {
                title = "Mobile Day 2014",
                Slides = SlidesRepository.Slides,
                me = me
            };
            return View["Slides", model];
        };

        Get[@"/Slides/{order}"] = parameters =>
        {
            //*important
            byte order = parameters.order; //I'm forcing the right conversion
            dynamic model = null;

            if (order == 0)
            {
                model = new
                   {
                       title = "Mobile Day 2014",
                       Slide = new Slide() { Ordine = 0, Contenuto = "", Stato = true },
                       me = me
                   };
            }
            else
            {
                model = new
                  {
                      title = "Mobile Day 2014",
                      Slide = SlidesRepository.getByOrder(order),
                      me = me
                  };
            }
            return View["single_Slide", model];
        };

        Post["/Slides/{order}"] = parameters =>
        {
            short order = parameters.order;
            Slide new_slide = null;

            new_slide = new Slide
            {
                Ordine = Request.Form["ordine"],
                Contenuto = Request.Form["contenuto"],
                Attributi = Request.Form["attributi"],
                Stato = Request.Form["stato"]
            };

            var old_slide = SlidesRepository.getByOrder(order);

            dynamic model = null;
            Slide slide = null;

            if (old_slide==null)
            {
                if (new_slide.Ordine != 0)
                    slide = SlidesRepository.muovi(new_slide);
                else
                    slide = SlidesRepository.nuovo(new_slide);
            }
            else
            {
                slide = SlidesRepository.update(order, new_slide);
            }


            if (slide != null)
            {
                model = new
                {
                    title = "Mobile Day 2014",
                    Slide = slide,
                    success = true,
                    messages = new List<string> { "The Slide has been successfull modified" },
                    me = me
                };
                if (order == 0)
                    return Response.AsRedirect("/slides/" + slide.Ordine); //redirects to items
            }
            else
            {
                model = new
                {
                    title = "Mobile Day 2014",
                    Slide = new_slide, //I'm going to return back the one given
                    success = false,
                    messages = new List<string> { "The Slide could not be modified" },
                    me = me
                };
            }
            return View["single_Slide", model];
        };
    }
}
