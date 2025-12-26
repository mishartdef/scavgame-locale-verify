using System.Text.Json;

namespace validator;

public static class Helper
{
    private static bool _hasCriticalError = false;
    
    /// <summary>
    /// Returns a list of all keys from the locale file.
    /// </summary>
    /// <param name="path">Path to the locale file.</param>
    /// <returns>A list of all keys in the given JSON file.</returns>
    public static List<string> GetFullKeys(string path)
    {
        try 
        {
            var keys = new List<string>();
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            ExtractKeys(doc.RootElement, "", keys);
            return keys;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"::error file={Path.GetFileName(path)}::Failed to parse JSON: {ex.Message}");
            _hasCriticalError = true;
            ValidationCheck();
            return new List<string>();
        }
    }

    /// <summary>
    /// Recursively extracts all keys from the JSON element.
    /// </summary>
    /// <param name="element">The JSON element to extract keys from.</param>
    /// <param name="prefix">The current path.</param>
    /// <param name="keys">The list of keys to add to.</param>
    public static void ExtractKeys(JsonElement element, string prefix, List<string> keys)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                string currentPath = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                keys.Add(currentPath);
                ExtractKeys(property.Value, currentPath, keys);
            }
        }
    }

    /// <summary>
    /// Marks the validation as failed.
    /// </summary>
    public static void MarkAsFailed() => _hasCriticalError = true;

    /// <summary>
    /// Exit with error code, if there is a critical error.
    /// </summary>
    /// <param name="hasCriticalError"></param>
    public static void ValidationCheck()
    {
        if (_hasCriticalError)
        {
            Console.WriteLine("\n[RESULT] Validation failed. Fix the errors above.");
            Environment.Exit(1);
        }
        
        Console.WriteLine("\n[RESULT] All checks passed successfully.");
    }
}