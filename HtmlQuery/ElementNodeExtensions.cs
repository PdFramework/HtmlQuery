namespace PeinearyDevelopment.Framework.HtmlQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ElementNodeExtensions
    {
        public static IEnumerable<DomElement> SelectNodes(this DomElement domTree, string selector)
        {
            var results = new List<DomElement>();
            var funcs = new QueryGenerator().CreateHtmlFuncs(selector);

            foreach (var func in funcs.Where(func => domTree.Descendants != null))
            {
                results.AddRange(func(domTree));
                results.AddRange(domTree.Descendants.Selector(func));
            }
            
            return results;
        }

        private static IEnumerable<DomElement> Selector(this IEnumerable<DomElement> domTree, Func<DomElement, IEnumerable<DomElement>> func)
        {
            var matchingNodes = new List<DomElement>();

            foreach (var elementNode in domTree.Where(elementNode => elementNode.Descendants != null))
            {
                matchingNodes.AddRange(func(elementNode));
                matchingNodes.AddRange(elementNode.Descendants.Selector(func));
            }

            return matchingNodes;
        }
    }
}