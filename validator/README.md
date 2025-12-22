# Locale Validator by mishart

## How to run

1. Clone the repository:
   ```bash
   git clone https://github.com/mishartdef/scavgame-locale-verify.git
   cd scavgame-locale-verify
   ```

2. Install a recent .NET SDK
   Make sure you have a recent .NET SDK installed (version 8 or later).
   ```bash
   dotnet --version
   ```

3. Run the validator:
   ```bash
   dotnet run --project validator/validator.csproj [path-to-locale-file]
   ```

4. Review the results:
   The validator will check the specified locale file and compare it with the original English file for missing and extra keys.

   - If the locale file contains no errors, validation will pass.
   - If there are missing keys, validation will fail and the validator will print the count and a list of missing keys.
   - The validator will also warn about any extra keys, listing their count and names.

   Example output:
   ```bash
   [ERROR]: Missing keys: 1
      - missing.key

   [WARNING]: Extra keys: 1
      + extra.key
   [RESULT] Validation failed. Fix the errors above.
   ```

## GitHub Actions support

The validator is used to check files in PRs via GitHub Actions. It should be configured automatically.