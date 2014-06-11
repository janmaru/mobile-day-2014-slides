using mahamudra.it.slides.dal;
using Owin;

/// <summary>
/// Summary description for StartUp
/// </summary>
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseNancy(); //use Nancy!!!
        SlidesRepository.create(); //create our repository
        UsersRepository.create(); //load users
    }
}