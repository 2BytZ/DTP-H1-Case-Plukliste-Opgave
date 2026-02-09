using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Plukliste
{
    // Base class for extract file system and template rendering.
    public class FileService
    {
        //Define the directories of project folders
        private readonly string _exportDirectory = "export";        //private readonly so that only FileStream has access
        private readonly string _templateDirectory = "templates";   //thereby giving it the role of forwarding the directories
        private readonly string _importDirectory = "import";
        private readonly string _printDirectory = "print";

        public FileService()
        {
            //create output folders to ensure runtime folders are available
            Directory.CreateDirectory(_importDirectory);
            Directory.CreateDirectory(_printDirectory);
        }

        //make sure folders exist
        //check if folders exists where(=>) folder Directory is (root)
        public bool ExportExists() => Directory.Exists(_exportDirectory);       //create method "ExportExists"
        public bool TemplateExists() => Directory.Exists(_templateDirectory);   //create method "TemplateExists"

        public List<string> GetExportFiles()
        {
            //return enumerated export files as a list if the folder exists
            return Directory.Exists(_exportDirectory)                   //if exportDirectory exists...
                ? Directory.EnumerateFiles(_exportDirectory).ToList()   //? = then
                : new List<string>();                                   //: = else
        }
        public List<string> GetTemplates()
        {
            //return enumerated template files as a list if the folder exists
            return Directory.Exists(_templateDirectory)
                ? Directory.EnumerateFiles(_templateDirectory).ToList()
                : new List<string>();
        }
        public string ReadTemplate(string templateFileName)
        {
            //save the path of the template(s) in variable  "path"
            var path = Path.Combine(_templateDirectory, templateFileName);
            return File.ReadAllText(path);
        }
        public void WritePrintOutput(string fileName, string content)
        {
            //create a file and write all text from template html in it, inside printDirectory
            var path = Path.Combine(_printDirectory, fileName);
            File.WriteAllText(path, content);
        }
        public void MoveToImport(string filePath)
        {
            var destinationPath = Path.Combine(_importDirectory, Path.GetFileName(filePath));
            File.Move(filePath, destinationPath);
        }
        public void DeleteImportIfEmptyOrForce(bool deleteRecursive = true) //delete Import folder if it is empty
        {
            //check if import folder exists
            if (Directory.Exists(_importDirectory))
            {
                Directory.Delete(_importDirectory, deleteRecursive);
            }
        }
    }
}
