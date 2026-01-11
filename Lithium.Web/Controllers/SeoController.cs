using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Lithium.Web.Core.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Lithium.Web.Controllers;

public sealed class SeoController(IWebHostEnvironment env) : Controller
{
    [Route("/robots.txt")]
    public ContentResult RobotsTxt()
    {
        var sb = new StringBuilder().AppendLine("User-agent: *");

        sb.AppendLine(env.IsProduction() ? "Allow: /" : "Disallow: /");
        sb.AppendLine($"Sitemap: {Request.Scheme}://{Request.Host}/sitemap.xml");

        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }

    [Route("/sitemap.xml")]
    public ContentResult Sitemap()
    {
        var xmlns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
        var xhtml = XNamespace.Get("http://www.w3.org/1999/xhtml");

        var root = new XElement(xmlns + "urlset", new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));

        var pages = typeof(SeoController).Assembly.ExportedTypes
            .Where(p => p.IsDefined(typeof(Microsoft.AspNetCore.Components.RouteAttribute), false) &&
                        p.IsSubclassOf(typeof(Microsoft.AspNetCore.Components.ComponentBase)))
            .ToList();

        var webSiteUrl = $"{Request.Scheme}://{Request.Host}";

        foreach (var page in pages)
        {
            var routes = page.GetCustomAttributes<Microsoft.AspNetCore.Components.RouteAttribute>().ToList();

            var priorityAttr = page.GetCustomAttribute<Sitemap.PriorityAttribute>();
            var priority = priorityAttr?.Priority ?? 0.5; // Default priority

            var changeFreqAttr = page.GetCustomAttribute<Sitemap.ChangeFrequencyAttribute>();
            var changeFreq = changeFreqAttr?.ChangeFrequency.ToString().ToLowerInvariant();

            var lastModAttr = page.GetCustomAttribute<Sitemap.LastModificationAttribute>();
            var lastMod = lastModAttr?.ToString();

            foreach (var route in routes)
            {
                var pageUrl = route.Template;
                if (pageUrl.Contains('{')) continue;

                var url = new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", $"{webSiteUrl}{pageUrl}"),
                    new XElement(xmlns + "priority",
                        priority.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture))
                );

                if (!string.IsNullOrEmpty(changeFreq))
                {
                    url.Add(new XElement(xmlns + "changefreq", changeFreq));
                }

                if (!string.IsNullOrEmpty(lastMod))
                {
                    url.Add(new XElement(xmlns + "lastmod", lastMod));
                }

                if (routes.Count > 1)
                {
                    foreach (var altRoute in routes)
                    {
                        var altUrl = altRoute.Template;
                        if (altUrl.Contains('{')) continue;

                        var segments = altUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                        var lang = "x-default";

                        if (segments.Length > 0)
                        {
                            var segment = segments[0];
                            if (segment.Length == 2 || (segment.Length == 5 && segment.Contains('-')))
                            {
                                lang = segment;
                            }
                        }

                        if (altUrl == "/") lang = "x-default";

                        url.Add(new XElement(xhtml + "link",
                            new XAttribute("rel", "alternate"),
                            new XAttribute("hreflang", lang),
                            new XAttribute("href", $"{webSiteUrl}{altUrl}")));
                    }
                }

                root.Add(url);
            }
        }

        var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        var sb = new StringBuilder();

        using (var writer = new Utf8StringWriter(sb))
        {
            xDoc.Save(writer);
        }

        return Content(sb.ToString(), "text/xml", Encoding.UTF8);
    }

    private class Utf8StringWriter(StringBuilder sb) : StringWriter(sb)
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}