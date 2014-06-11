using mahamudra.it.slides.dal;
using Nancy;

/// <summary>
/// Summary description for HomeModule
/// </summary>
public class HomeModule : NancyModule
{ 
    public HomeModule() 
    {
        Get["/"] = _ => {
            var model = new
            {
                title = "Mobile Day 2014 - Reveal.js - The HTML Presentation Framework",
                Slides =  SlidesRepository.Slides }; 
                return View["index", model]; 
        }; 
    } 
}
