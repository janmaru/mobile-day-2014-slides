using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for User
/// </summary>
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public List<string> Roles
    {
        get
        {
            return SimpleRoles.ToString().Split(',').ToList();
        }
    }
    public string SimpleRoles { get; set; }
    public bool IsAdmin
    {
        get
        {
            return SimpleRoles.Contains("0");
        }
    }
}