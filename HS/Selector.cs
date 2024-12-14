using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HS
{
    internal class Selector
    {
        public string TagName { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<string> Classes { get; set; } = new List<string>();

        public Selector Parent { get; set; }
        public List<Selector> Children { get; set; } = new List<Selector>();

        public static Selector FromQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.");

            var parts = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Selector root = null;
            Selector currentSelector = null;

            foreach (var part in parts)
            {
                var newSelector = new Selector();
                var segments = Regex.Split(part, @"(?=[#.])");

                foreach (var segment in segments)
                {
                    if (string.IsNullOrWhiteSpace(segment)) continue;

                    if (segment.StartsWith("#"))
                    {
                        newSelector.Id = segment.Substring(1);
                    }
                    else if (segment.StartsWith("."))
                    {
                        newSelector.Classes.Add(segment.Substring(1));
                    }
                    else
                    {
                        newSelector.TagName = segment;
                    }
                }

                if (root == null)
                {
                    root = newSelector;
                }
                else
                {
                    currentSelector.Children.Add(newSelector);
                    newSelector.Parent = currentSelector;
                }

                currentSelector = newSelector;
            }

            return root;
        }

        public void PrintHierarchy(string indent = "")
        {
            Console.WriteLine($"{indent}Selector: TagName={TagName}, Id={Id}, Classes=[{string.Join(", ", Classes)}]");
            foreach (var child in Children)
            {
                child.PrintHierarchy(indent + "  ");
            }
        }

        public bool Matches(Selector querySelector)
        {
            bool matchesTag = string.IsNullOrEmpty(querySelector.TagName) || querySelector.TagName == this.TagName;
            bool matchesId = string.IsNullOrEmpty(querySelector.Id) || querySelector.Id == this.Id;
            bool matchesClasses = !querySelector.Classes.Any() || querySelector.Classes.All(c => this.Classes.Contains(c));

            return matchesTag && matchesId && matchesClasses;
        }
    }
}

