using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

/// <summary>
/// Summary description for Contenuto
/// </summary>
public class XmlApp
{
    public string Value { get; set; }
    public string Key { get; set; }
}

public class Xmlconfig
{
    public static string filePath =  @"App_Data\config.xml";
    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    public static XmlApp get(string key, string path)
    {
        path += filePath;
        XDocument xmlDoc = XDocument.Load(path);
        return (from apps in xmlDoc.Descendants("app")
                where apps.Attribute("key").Value.ToString() == key
                select new XmlApp()
                {
                    Value = apps.Attribute("value").Value,
                    Key = apps.Attribute("key").Value
                }).FirstOrDefault();
    }

    /// <summary>
    /// Gets the users.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    public static List<User> getUsers(string path)
    {
        path += filePath;
        XDocument xmlDoc = XDocument.Load(path);
        return (from u in xmlDoc.Descendants("user")
                select new User()
                {
                    Id = Convert.ToInt32(u.Attribute("id").Value),
                    UserName = u.Attribute("username").Value,
                    Password = u.Attribute("password").Value,
                    SimpleRoles = u.Attribute("roles").Value 
                }).ToList();
    }
}
