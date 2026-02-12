using System;
using System.IO;
using System.Linq;

namespace FKTV_DAL
{
    public class DataAccess
    {
        //get location of local items data
        private static string LocalDataPath => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Data", "Items.json"));

        private static string? GetMarkerFolderName //Thank you CoPilot
        {
            get
            {
                try
                {
                    var fullPath = Path.GetFullPath(LocalDataPath);
                    var separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
                    var segments = fullPath.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    // Look for "repos" segment and return the next segment if present
                    for (int i = 0; i < segments.Length - 1; i++)
                    {
                        if (segments[i].Equals("repos", StringComparison.OrdinalIgnoreCase))
                            return segments[i + 1];
                    }

                    // Walk up from the base directory and return the first ancestor that looks like a repo root
                    var dir = new DirectoryInfo(AppContext.BaseDirectory);
                    for (int up = 0; up < 8 && dir != null; up++)
                    {
                        if (Directory.Exists(Path.Combine(dir.FullName, ".git")) || Directory.EnumerateFiles(dir.FullName, "*.sln").Any())
                            return dir.Name;
                        dir = dir.Parent;
                    }

                    // Fallback: return the topmost ancestor's name
                    var top = new DirectoryInfo(fullPath);
                    while (top.Parent != null) top = top.Parent;
                    return top.Name;
                }
                catch
                {
                    return null;
                }
            }
        }

        // Public property to resolve the console-app export folder inside the repository
        public string GetPluklistExportFolder =>
            Path.GetFullPath(Path.Combine(FindRepositoryRoot() ?? AppContext.BaseDirectory, "FKTV_DAL", "export"));

        public string PluklistImportLocation =>
            Path.Combine(FindRepositoryRoot() ?? AppContext.BaseDirectory, "FKTV_DAL", "import");

        public string GetData()
        {
            if (!File.Exists(LocalDataPath))
                throw new FileNotFoundException("Local data file not found.", LocalDataPath);
            //return all json data from items file
            return File.ReadAllText(LocalDataPath);
        }

        public void UpdateAmount(string json)
        {
            //create the folder and file for items.json if it doesn't exist already
            var dir = Path.GetDirectoryName(LocalDataPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            //update data in items file with new data
            File.WriteAllText(LocalDataPath, json);
        }

        private static string? FindRepositoryRoot() //used to locate and sync data files' data across solution
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            var marker = GetMarkerFolderName;
            while (dir != null)
            {
                // Check: marker match OR contains .sln OR contains .git
                if ((!string.IsNullOrEmpty(marker) && dir.Name.Equals(marker, StringComparison.OrdinalIgnoreCase))
                    || Directory.EnumerateFiles(dir.FullName, "*.sln").Any()
                    || Directory.Exists(Path.Combine(dir.FullName, ".git")))
                {
                    //if either the name of project folder is correct,
                    //the project folder contains the solution
                    //or the project git file exists
                    return dir.FullName; //return the full path name of the project folder
                }

                dir = dir.Parent; //directory parent folder location
            }

            return null; //return if dir is an orphan
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
                    var sourceContent = File.ReadAllText(sourcePath); //get all data in local items json
                    var fkTV_DAL_Path = Path.Combine(repoRoot, "FKTV_DAL", "Data", "Items.json"); //get location of DAL items data file

                    //get location of plukliste local items data file
                    var pluklistePath = Path.Combine(repoRoot, "DTP_Case_Plukliste_ConsoleApp", "bin", "Debug", "net10.0", "Data", "Items.json");

                    //overwrite Items.json file at these locations with a new file
                    Directory.CreateDirectory(Path.GetDirectoryName(fkTV_DAL_Path) ?? string.Empty);
                    Directory.CreateDirectory(Path.GetDirectoryName(pluklistePath) ?? string.Empty);

                    //get all data in plukliste local items data file if it exists
                    var plukContent = File.Exists(pluklistePath) ? File.ReadAllText(pluklistePath) : null;

                    //get all data in DAL items data file if it exists
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

                    //overwrite data in these locations
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
