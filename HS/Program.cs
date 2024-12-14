// See https://aka.ms/new-console-template for more information

using Clean.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Program
{
    private static readonly ConsoleColor DefaultColor = Console.ForegroundColor;

    public static async Task Main(string[] args)
    {
        try
        {
            WriteColored("\n=== Step 1: Loading HTML ===", ConsoleColor.Cyan);
            var html = await LoadHtmlAsync("https://www.w3schools.com/"); 
            //var html = await LoadHtmlAsync("https://httpbin.org/html");
            WriteColored("HTML loaded successfully! Showing the first 500 characters:", ConsoleColor.Green);
            Console.WriteLine(html.Length > 500 ? html[..500] + "..." : html);

            WriteColored("\n=== Step 2: Parsing HTML ===", ConsoleColor.Cyan);
            var htmlLines = ParseHtmlLines(html);
            WriteColored($"Parsed {htmlLines.Count} lines of HTML.", ConsoleColor.Green);

            WriteColored("\n=== Step 3: Building HTML Tree ===", ConsoleColor.Cyan);
            var root = HtmlElement.BuildHtmlTree(htmlLines);
            WriteColored("HTML tree built successfully!", ConsoleColor.Green);

            WriteColored("\n=== Step 4: Displaying Descendants ===", ConsoleColor.Cyan);
            PrintDescendants(root);

            WriteColored("\n=== Step 5: CSS Querying ===", ConsoleColor.Cyan);
            var queries = new[] { "div", "p", "body", "meta" }; // כל הסלקטורים הבסיסיים
            foreach (var query in queries)
            {
                WriteColored($"\nQuery: {query}", ConsoleColor.Yellow);
                var queryResults = root.FindBySelector(query);

                if (queryResults.Any())
                {
                    WriteColored($"Found {queryResults.Count()} results:", ConsoleColor.Green);
                    foreach (var element in queryResults)
                    {
                        Console.WriteLine($"- <{element.Name}> (Id: {element.Id}, Classes: [{string.Join(", ", element.Classes)}])");
                    }
                }
                else
                {
                    WriteColored($"No results found for query '{query}'.", ConsoleColor.Red);
                }
            }
        }
        catch (Exception ex)
        {
            WriteColored($"[Error] {ex.Message}", ConsoleColor.Red);
        }
    }

    private static async Task<string> LoadHtmlAsync(string url)
    {
        using HttpClient client = new();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode(); // יוודא שהסטטוס קוד תקין (200)
        return await response.Content.ReadAsStringAsync(); // מחזיר את התוכן של ה-HTML
    }

    private static List<string> ParseHtmlLines(string html)
    {
        var matches = Regex.Matches(html, @"<[^>]+>|[^<]+");
        return matches
            .Cast<Match>()
            .Select(m => m.Value.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    private static void PrintDescendants(HtmlElement root)
    {
        Console.WriteLine("\n--- Existing Format ---");
        foreach (var descendant in root.Descendants())
        {
            // Existing format output
            Console.WriteLine($"<{descendant.Name}> (Id: {descendant.Id}, Classes: [{string.Join(", ", descendant.Classes)}])");
        }

        Console.WriteLine("\n--- New Format ---");
        foreach (var descendant in root.Descendants())
        {
            // New format output
            Console.WriteLine($"Tag: {descendant.Name}, Id: {descendant.Id ?? "[Not Found]"}, Classes: [{string.Join(", ", descendant.Classes)}]");
        }
    }

    private static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = DefaultColor;
    }
}