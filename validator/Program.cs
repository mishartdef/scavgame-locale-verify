using static validator.Helper;

// Getting path to locales directory
string localeDir = Directory.GetCurrentDirectory();

// Getting path to original locale file
string originalFilePath = args.Length >= 2 ? Path.Combine(localeDir, args[1])
    : Path.Combine(localeDir, "EN.json");

if (!File.Exists(originalFilePath))
{
    Console.WriteLine($"::error::Original locale file not found: {originalFilePath}");
    MarkAsFailed();
}

else
{
    Console.WriteLine($"Original locale file: {Path.GetFileName(originalFilePath)}");

    // Getting all keys from original locale file
    var originalKeys = GetFullKeys(originalFilePath);

    // Getting all verify files from locale directory
    var filesToVerify = args.Length >= 1 ? [Path.Combine(localeDir, args[0])]
        : Directory.GetFiles(localeDir, "*.json");

    // Checking all locale files
    foreach (var filePath in filesToVerify)
    {
        string fileName = Path.GetFileName(filePath);
        try
        {
            Console.WriteLine($"\n>>> Checking: {fileName}");
            var currentKeys = GetFullKeys(filePath);

            var missingKeys = originalKeys.Except(currentKeys).ToList();
            var extraKeys = currentKeys.Except(originalKeys).ToList();

            if (!missingKeys.Any() && !extraKeys.Any())
            {
                Console.WriteLine($"[Pass] {fileName} is valid.");
            }
            else
            {
                // Missing keys are critical, without them game will use keys name (e.g. bandage bandagedcs)
                if (missingKeys.Any())
                {
                    if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
                    {
                        Console.WriteLine($"::error file={fileName}::Missing {missingKeys.Count} keys.");
                    }
                    else
                    {
                        Console.WriteLine($"[Error] Missing keys: ({missingKeys.Count}):");
                    }
                    missingKeys.ForEach(k => Console.WriteLine($"   - {k}"));
                    MarkAsFailed();
                }

                // Extra keys are not that critical, but they can be confusing for the translators
                if (extraKeys.Any())
                {
                    if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
                    {
                        Console.WriteLine($"::warning file={fileName}::Found {extraKeys.Count} extra keys.");
                    }
                    else
                    {
                        Console.WriteLine($"[Warning] Extra keys: ({extraKeys.Count}):");
                    }
                    extraKeys.ForEach(k => Console.WriteLine($"   + {k}"));
                }
            }
        }
        // If there is a critical error in parse (e.g. corrupted json), then exit with error code
        catch (Exception ex)
        {
            Console.WriteLine($"::error file={fileName}::Critical error: {ex.Message}");
            MarkAsFailed();
        }
    }
}



// If there was a critical error (missing keys or exception), then exit with error code
ValidationCheck();

Console.WriteLine("\n[RESULT] All files passed successfully!");
