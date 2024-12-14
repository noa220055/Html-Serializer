using System;
using System.IO;
using System.Text.Json;

public class HtmlHelper
{
    private static readonly HtmlHelper _instance = new HtmlHelper();
    public static HtmlHelper Instance => _instance;

    public string[] HtmlTags { get; private set; }
    public string[] SelfClosingTags { get; private set; }

    private HtmlHelper()
    {
        HtmlTags = LoadTagsFromFile("htmlTags.json");
        SelfClosingTags = LoadTagsFromFile("selfClosingTags.json");
    }

    private string[] LoadTagsFromFile(string fileName)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[Error] File not found: {filePath}");
            return Array.Empty<string>();
        }

        try
        {
            var jsonContent = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<string[]>(jsonContent) ?? Array.Empty<string>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Error] Invalid JSON format in {fileName}: {ex.Message}");
            return Array.Empty<string>();
        }
    }
}
