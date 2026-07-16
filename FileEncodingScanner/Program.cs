using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

var options = ParseArguments(args);
var rootPath = ResolveRootPath(options.Path);
var files = EnumerateFiles(rootPath, options.IncludeAll, options.SkipBinaryFiles);
var results = new List<FileEncodingResult>();

foreach (var file in files)
{
    try
    {
        var bytes = File.ReadAllBytes(file);
        var encoding = DetectEncoding(bytes);
        results.Add(new FileEncodingResult(file, encoding));
    }
    catch (Exception ex)
    {
        results.Add(new FileEncodingResult(file, $"Error: {ex.Message}"));
    }
}

results = results.OrderBy(r => r.Path, StringComparer.InvariantCultureIgnoreCase).ToList();

Console.WriteLine($"Scanned {results.Count} files under {rootPath}");
Console.WriteLine("Path | Encoding");
Console.WriteLine(new string('-', 120));

foreach (var result in results)
{
    Console.WriteLine($"{result.Path} | {result.Encoding}");
}

if (!string.IsNullOrWhiteSpace(options.OutputPath))
{
    var outputPath = Path.GetFullPath(options.OutputPath);
    var outputDirectory = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrWhiteSpace(outputDirectory))
    {
        Directory.CreateDirectory(outputDirectory);
    }

    using (var writer = new StreamWriter(outputPath, false, new UTF8Encoding(true)))
    {
        writer.WriteLine("Path,Encoding");
        foreach (var result in results)
        {
            writer.WriteLine($"{EscapeCsv(result.Path)},{EscapeCsv(result.Encoding)}");
        }
    }

    Console.WriteLine($"CSV written to {outputPath}");
}

return 0;

static string ResolveRootPath(string? path)
{
    if (!string.IsNullOrWhiteSpace(path))
    {
        return Path.GetFullPath(path);
    }

    var current = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (current != null)
    {
        if (File.Exists(Path.Combine(current.FullName, "PortalApps.sln")))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    return Directory.GetCurrentDirectory();
}

static IEnumerable<string> EnumerateFiles(string rootPath, bool includeAll, bool skipBinaryFiles)
{
    if (!Directory.Exists(rootPath))
    {
        throw new DirectoryNotFoundException($"Path not found: {rootPath}");
    }

    var extensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
    {
        ".cs", ".csproj", ".sln", ".config", ".xml", ".json", ".js", ".css",
        ".html", ".htm", ".txt", ".aspx", ".ascx", ".master", ".resx",
        ".sql", ".ps1", ".md"
    };

    var excludedDirectories = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
    {
        ".git", ".vs", "bin", "obj", "packages", "node_modules"
    };

    foreach (var file in Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories))
    {
        var directory = Path.GetDirectoryName(file);
        if (directory == null)
        {
            continue;
        }

        var relativeDirectory = directory.Substring(rootPath.Length).Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (relativeDirectory.Length > 0)
        {
            var parts = relativeDirectory.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Any(p => excludedDirectories.Contains(p)))
            {
                continue;
            }
        }

        var extension = Path.GetExtension(file);
        if (skipBinaryFiles && !includeAll && !string.IsNullOrEmpty(extension) && !extensions.Contains(extension))
        {
            continue;
        }

        yield return file;
    }
}

static string DetectEncoding(byte[] bytes)
{
    if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
    {
        return "UTF-8 with BOM";
    }

    if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
    {
        return bytes.Length >= 4 && bytes[2] == 0x00 && bytes[3] == 0x00 ? "UTF-32 LE with BOM" : "UTF-16 LE with BOM";
    }

    if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
    {
        return bytes.Length >= 4 && bytes[2] == 0x00 && bytes[3] == 0x00 ? "UTF-32 BE with BOM" : "UTF-16 BE with BOM";
    }

    var candidates = new Encoding[]
    {
        new UTF8Encoding(false, false),
        Encoding.GetEncoding(1254),
        Encoding.GetEncoding(1252),
        Encoding.GetEncoding(28599)
    };

    foreach (var encoding in candidates)
    {
        try
        {
            var decoded = encoding.GetString(bytes);
            var reencoded = encoding.GetBytes(decoded);
            if (reencoded.Length == bytes.Length && bytes.SequenceEqual(reencoded))
            {
                return encoding is UTF8Encoding ? "UTF-8 (no BOM)" : encoding.WebName;
            }
        }
        catch
        {
        }
    }

    return "Unknown";
}

static Options ParseArguments(string[] args)
{
    var options = new Options();
    for (var i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--path":
                if (i + 1 < args.Length)
                {
                    options.Path = args[++i];
                }
                break;
            case "--output":
                if (i + 1 < args.Length)
                {
                    options.OutputPath = args[++i];
                }
                break;
            case "--include-all":
                options.IncludeAll = true;
                break;
            case "--no-skip-binary":
                options.SkipBinaryFiles = true;
                break;
        }
    }

    return options;
}

static string EscapeCsv(string value)
{
    if (value == null)
    {
        return string.Empty;
    }

    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    return value;
}

internal sealed class FileEncodingResult
{
    public FileEncodingResult(string path, string encoding)
    {
        Path = path;
        Encoding = encoding;
    }

    public string Path { get; }
    public string Encoding { get; }
}

internal sealed class Options
{
    public string? Path { get; set; }
    public string? OutputPath { get; set; }
    public bool IncludeAll { get; set; }
    public bool SkipBinaryFiles { get; set; }
}
