namespace PeinearyDevelopment.Web.HtmlQuery
{
    using System;
    using System.Collections.Generic;

    public class HtmlElement
    {
        public string TagName { get; set; }
        public IDictionary<string, IEnumerable<string>> Attributes { get; set; }
        public IEnumerable<HtmlElement> Descendants { get; set; }
        public string Text { get; set; }
        internal bool HasEndingTag { get; set; }

        // TODO: figure out logic to order element ids in breadth first approach
        internal Tuple<int, int> Id { get; set; }

        public HtmlElement()
        {
            Attributes = new Dictionary<string, IEnumerable<string>>();
            Descendants = new List<HtmlElement>();
        }
    }
}
