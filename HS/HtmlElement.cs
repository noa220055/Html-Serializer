using System.Text.RegularExpressions;

public class HtmlElement
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Classes { get; set; } = new();
    public Dictionary<string, string> Attributes { get; set; } = new();
    public List<HtmlElement> Children { get; set; } = new();
    public HtmlElement Parent { get; set; }
    public string InnerHtml { get; set; } = string.Empty;

    public static HtmlElement BuildHtmlTree(List<string> htmlLines)
    {
        var root = new HtmlElement { Name = "root" };
        var currentElement = root;

        foreach (var line in htmlLines)
        {
            if (line.StartsWith("</"))
            {
                currentElement = currentElement.Parent ?? root;
            }
            else if (line.StartsWith("<") && !line.StartsWith("</"))
            {
                var newElement = ParseHtmlElement(line);
                if (newElement != null)
                {
                    newElement.Parent = currentElement;
                    currentElement.Children.Add(newElement);

                    if (!newElement.IsSelfClosing())
                    {
                        currentElement = newElement;
                    }
                }
            }
            else
            {
                currentElement.InnerHtml += line.Trim() + " ";
            }
        }

        return root;
    }

    private static HtmlElement ParseHtmlElement(string htmlElement)
    {
        var tagNameMatch = Regex.Match(htmlElement, @"<\s*(\w+)");
        if (!tagNameMatch.Success) return null;

        var tagName = tagNameMatch.Groups[1].Value;
        var attributes = Regex.Matches(htmlElement, @"(\w+)\s*=\s*""(.*?)""")
                              .Cast<Match>()
                              .GroupBy(m => m.Groups[1].Value)  // קיבוץ המאפיינים לפי השם שלהם
                              .ToDictionary(g => g.Key, g => g.Last().Groups[2].Value);  // בחר את הערך האחרון במקרה של כפילויות

        var id = attributes.TryGetValue("id", out var idValue) ? idValue : string.Empty;
        var classes = attributes.TryGetValue("class", out var classValue) ? classValue.Split(' ').ToList() : new List<string>();

        return new HtmlElement
        {
            Name = tagName,
            Id = id,
            Classes = classes,
            Attributes = attributes
        };
    }

    public IEnumerable<HtmlElement> Descendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.Descendants())
            {
                yield return descendant;
            }
        }
    }

    public IEnumerable<HtmlElement> FindBySelector(string selector)
    {
        var result = new HashSet<HtmlElement>();
        RecursiveFind(this, selector.Split(' '), 0, result);
        return result;
    }

    private void RecursiveFind(HtmlElement element, string[] selectors, int index, HashSet<HtmlElement> result)
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

    private bool MatchSelector(HtmlElement element, string selector)
    {
        if (selector.StartsWith("#"))
            return element.Id == selector.Substring(1);

        if (selector.StartsWith("."))
            return element.Classes.Contains(selector.Substring(1));

        return element.Name == selector;
    }

    public bool IsSelfClosing()
    {
        return new[] { "br", "img", "hr", "input", "meta", "link" }.Contains(Name);
    }
}