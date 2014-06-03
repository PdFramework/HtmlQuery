namespace PeinearyDevelopment.Web.HtmlQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ElementNodeExtensions
    {
        public static IEnumerable<HtmlElement> SelectNodes(this HtmlElement domTree, string selector)
        {
            var results = new List<HtmlElement>();
            var funcs = new QueryGenerator().CreateHtmlFuncs(selector);

            foreach (var func in funcs.Where(func => domTree.Descendants != null))
            {
                results.AddRange(func(domTree));
                results.AddRange(domTree.Descendants.Selector(func));
            }
            
            return results;
        }

        private static IEnumerable<HtmlElement> Selector(this IEnumerable<HtmlElement> domTree, Func<HtmlElement, IEnumerable<HtmlElement>> func)
        {
            var matchingNodes = new List<HtmlElement>();

            foreach (var elementNode in domTree.Where(elementNode => elementNode.Descendants != null))
            {
                matchingNodes.AddRange(func(elementNode));
                matchingNodes.AddRange(elementNode.Descendants.Selector(func));
            }

            return matchingNodes;
        }
    }
}