using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

public class CustomSessionManager : IHttpModule, ISessionIDManager
{
    private SessionStateSection sessionConfig = null;

    public bool InitializeRequest(HttpContext context, bool suppressAutoDetectRedirect, out bool supportSessionIDReissue)
    {
        if (sessionConfig.Cookieless == HttpCookieMode.UseCookies)
        {
            supportSessionIDReissue = false;
            return false;
        }

        supportSessionIDReissue = true;
        return context.Response.IsRequestBeingRedirected;
    }

    public string GetSessionID(HttpContext context)
    {
        string id = null;

        if (sessionConfig.Cookieless == HttpCookieMode.UseUri)
        {
            if (context.Request.QueryString[sessionConfig.CookieName] != null)
            {
                id = context.Request.QueryString[sessionConfig.CookieName];
            }
        }
        else
        {
            if (context.Request.Cookies[sessionConfig.CookieName] != null)
            {
                id = context.Request.Cookies[sessionConfig.CookieName].Value;
            }
        }

        if (!Validate(id))
        {
            id = null;
        }

        return id;
    }

    public string CreateSessionID(HttpContext context)
    {
        return $"{RandomString(_random.Next(16))}{_phrases[_random.Next(_phrases.Count)].Replace(" ", RandomString(_random.Next(8)))}{RandomString(_random.Next(16))}";
    }
     
    public void SaveSessionID(HttpContext context, string id, out bool redirected, out bool cookieAdded)
    {
        if (sessionConfig.Cookieless == HttpCookieMode.UseUri)
        {
            // Add the SessionID to the URI. Set the redirected variable as appropriate.
            redirected = true;
            cookieAdded = false;
        }
        else
        {
            context.Response.Cookies.Add(new HttpCookie(sessionConfig.CookieName, id));
            cookieAdded = true;
            redirected = false;
        }
    }

    public bool Validate(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        foreach (var s in _phrases)
        {
            var x = s.Split(' ').All(id.Contains);
            if (!x) continue;
            return true;
        }

        return false;
    }

    public void Initialize()
    {
        if (sessionConfig == null)
        {
            var cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            sessionConfig = (SessionStateSection) cfg.GetSection("system.web/sessionState");
        }
    }

    public void Init(HttpApplication context)
    {
        Initialize();
    }

    public void Dispose()
    {
    }

    public void RemoveSessionID(HttpContext context)
    {
        context.Response.Cookies.Remove(sessionConfig.CookieName);
    }

    private readonly List<string> _phrases = new List<string>()
                                             {
                                                 "THEY ARE WATCHING YOU",
                                                 "FEAR EVERYONE AROUND YOU",
                                                 "DID YOU FORGET YOUR KEYS",
                                                 "DO NOT OPEN THE DOOR AFTER MIDNIGHT",
                                                 "THEY MADE ME DO THIS",
                                                 "I AM UNWELL AFTER THE RITUAL"
                                             };

    private readonly Random _random = new Random();

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}