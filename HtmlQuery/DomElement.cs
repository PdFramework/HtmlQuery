namespace PeinearyDevelopment.Web.HtmlQuery
{
    using System;
    using System.IO;
    using System.Net;

    public class DomElement : HtmlElement
    {
        public static HtmlElement ParseDomElement(TextReader reader)
        {
            return reader.ParseDomElement();
        }

        public static HtmlElement ParseDomElement(string htmlString)
        {
            return htmlString.ParseDomElement();
        }

        public static HtmlElement ParseDomElement(Stream stream)
        {
            return stream.ParseDomElement();
        }

        public static HtmlElement ParseDomElement(WebClient client, string url)
        {
            return client.ParseDomElement(url);
        }

        public static HtmlElement ParseDomElement(WebClient client, Uri url)
        {
            return client.ParseDomElement(url);
        }
    }
}
