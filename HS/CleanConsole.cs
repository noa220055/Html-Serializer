using System.Collections.Generic;
using System.Linq;

namespace Clean.Console
{
    public static class HtmlElementExtensions
    {
        public static IEnumerable<HtmlElement> FindBySelector(this HtmlElement root, string selector)
        {
            var result = new HashSet<HtmlElement>();
            RecursiveFind(root, selector.Split(' '), 0, result);
            return result;
        }

        private static void RecursiveFind(HtmlElement element, string[] selectors, int index, HashSet<HtmlElement> result)
        {
            if (index >= selectors.Length) return;

            var currentSelector = selectors[index];
            var matchingDescendants = element.Descendants()
                                              .Where(e => MatchSelector(e, currentSelector));

            if (index == selectors.Length - 1)
            {
                foreach (var match in matchingDescendants)
                {
                    result.Add(match);
                }
            }
            else
            {
                foreach (var match in matchingDescendants)
                {
                    RecursiveFind(match, selectors, index + 1, result);
                }
            }
        }

        private static bool MatchSelector(HtmlElement element, string selector)
        {
            if (selector.StartsWith("#"))
                return element.Id == selector.Substring(1);

            if (selector.StartsWith("."))
                return element.Classes.Contains(selector.Substring(1));

            return element.Name == selector;
        }
    }
}

