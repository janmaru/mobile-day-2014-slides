using Encryption;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// Summary description for AppConfig
/// </summary>
public class AppConfig
{
    public static Encryption.Symmetric.Provider Provider = Symmetric.Provider.Rijndael;
}

public class CustomRootPathProvider : IRootPathProvider
{
    private static string path =  HttpContext.Current.Server.MapPath("~/"); 
    //@"M:\DOTNETCAMPANIA\Community Day - Owin\flexslider-kwiks\";
  
    public string GetRootPath()
    {
        //return "What ever path you want to use as your application root";
        return path;
    }
    public static string rootPath()
    {
        return path;
    }
}

public class CustomBootstrapper : DefaultNancyBootstrapper
{
    protected override IRootPathProvider RootPathProvider
    {
        get { return new CustomRootPathProvider(); }
    }
}