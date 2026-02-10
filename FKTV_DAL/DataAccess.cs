using System;
using System.IO;
using System.Linq;

namespace FKTV_DAL
{
    public class DataAccess
    {
        private static string LocalDataPath =>
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Data", "Items.json"));

        public string GetData()
        {
            if (!File.Exists(LocalDataPath))
                throw new FileNotFoundException("Local data file not found.", LocalDataPath);

            return File.ReadAllText(LocalDataPath);
        }

        public void UpdateAmount(string json)
        {
            var dir = Path.GetDirectoryName(LocalDataPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(LocalDataPath, json);
        }

        private static string? FindRepositoryRoot(string markerFolderName = "Story3_Lager")
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (dir.Name.Equals(markerFolderName, StringComparison.OrdinalIgnoreCase)
                    || Directory.EnumerateFiles(dir.FullName, "*.sln").Any()
                    || Directory.Exists(Path.Combine(dir.FullName, ".git")))
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            return null;
        }

        public void SyncData(bool calledFromPluklist = false)
        {
            try
            {
                var repoRoot = FindRepositoryRoot() ?? Path.GetFullPath(AppContext.BaseDirectory);

                if (!calledFromPluklist)
                {
                    // Change originates from this app (e.g., FKTV web app)
                    var sourcePath = LocalDataPath;
                    if (!File.Exists(sourcePath))
                    {
                        Console.Error.WriteLine($"SyncData: source file not found: {sourcePath}");
                        return;
                    }

                    var sourceContent = File.ReadAllText(sourcePath);

                    var fkTV_DAL_Path = Path.Combine(repoRoot, "FKTV_DAL", "Data", "Items.json");
                    var pluklistePath = Path.Combine(repoRoot, "DTP_Case_Plukliste_ConsoleApp", "bin", "Debug", "net10.0", "Data", "Items.json");

                    Directory.CreateDirectory(Path.GetDirectoryName(fkTV_DAL_Path) ?? string.Empty);
                    Directory.CreateDirectory(Path.GetDirectoryName(pluklistePath) ?? string.Empty);

                    var plukContent = File.Exists(pluklistePath) ? File.ReadAllText(pluklistePath) : null;
                    var dalContent = File.Exists(fkTV_DAL_Path) ? File.ReadAllText(fkTV_DAL_Path) : null;

                    // If either file differs from the source, overwrite it
                    if (!string.Equals(plukContent, sourceContent, StringComparison.Ordinal) ||
                        !string.Equals(dalContent, sourceContent, StringComparison.Ordinal))
                    {
                        File.WriteAllText(pluklistePath, sourceContent);
                        File.WriteAllText(fkTV_DAL_Path, sourceContent);
                    }
                }
                else
                {
                    // Change originates from Plukliste console app
                    var pluklisteLocal = LocalDataPath;
                    if (!File.Exists(pluklisteLocal))
                    {
                        Console.Error.WriteLine($"SyncData (from pluklist): source file not found: {pluklisteLocal}");
                        return;
                    }

                    var content = File.ReadAllText(pluklisteLocal);

                    var fkTVExePath = Path.Combine(repoRoot, "FKTV", "bin", "Debug", "net10.0", "Data", "Items.json");
                    var fkTV_DAL_Path = Path.Combine(repoRoot, "FKTV_DAL", "Data", "Items.json");

                    Directory.CreateDirectory(Path.GetDirectoryName(fkTVExePath) ?? string.Empty);
                    Directory.CreateDirectory(Path.GetDirectoryName(fkTV_DAL_Path) ?? string.Empty);

                    File.WriteAllText(fkTVExePath, content);
                    File.WriteAllText(fkTV_DAL_Path, content);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"SyncData failed: {ex.Message}");
                throw;
            }
        }
    }
}
