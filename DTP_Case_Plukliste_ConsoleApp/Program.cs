//De kørende montører; CSV til plukliste
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using CsvHelper.Configuration;
using CsvHelper;
using System.IO;
using DTP_Case_Plukliste_ConsoleApp;
using System.Collections.Generic;

namespace Plukliste
{
    class PluklisteProgram
    {
        static void Main()
        {
            var fileService = new FileService();
            var templateRender = new TemplateRenderer();
            var displayUI = new ConsoleUi();

            //check if export file exists
            if (!fileService.ExportExists())
            {
                Console.WriteLine("(!) Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory() + "/export");
                return;
            }

            //check if template folder exists
            if (!fileService.TemplateExists())
            {
                Console.WriteLine("(!) Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory() + "/templates");
                return;
            }

            //create an array of the possible file parsers
            var parsers = new IPluklistParser[]
            {
                    new CsvPluklistParser(),
                    new XmlPluklistParser()
            };

            //names of the possible html templates
            var templateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "PRINT-OPGRADE", "PRINT-OPGRADE.html" },
                    { "PRINT-OPSIGELSE", "PRINT-OPGRADE.html" },
                    { "PRINT-WELCOME", "PRINT-OPGRADE.html" }
                };
            
            var files = fileService.GetExportFiles().ToList();  //get export files as their path and put in "files" as a list
            int index = 0;

            while (files.Count > 0)
            {
                var filePath = files[index];

                // choose parser for file
                var parser = parsers.FirstOrDefault(_parser => _parser.CanParse(filePath)); //check if this parser can parse file in path
                if (parser == null)
                {
                    Console.WriteLine("(!) Unsupported file format. Skipping..");
                    //files.RemoveAt(index);
                    //if (index >= files.Count) index = Math.Max(0, files.Count - 1);
                    continue;
                }

                Pluklist? plukliste;
                try
                {
                    plukliste = parser.Parse(filePath); //try to parse file
                }
                catch (Exception exNoParse)
                {
                    Console.WriteLine($"Failed to parse file: {exNoParse.Message}");
                    //files.RemoveAt(index);
                    //if (index >= files.Count) index = Math.Max(0, files.Count - 1);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    continue;
                }

                // display via UI
                displayUI.DisplayPluklist(plukliste, filePath, index, files.Count);

                // build and prune options
                var options = new List<string>
                    {
                        "Forrige plukseddel",
                        "Naeste plukseddel",
                        "Genindlaes pluksedler",
                        "Afslut plukseddel",
                        "Quit"
                    };
                var availableOptions = new List<string>(options);
                
                if (index <= 0) 
                { 
                    availableOptions.Remove("Forrige plukseddel");
                }
                if (index >= files.Count - 1)
                {
                    availableOptions.Remove("Naeste plukseddel");
                }
                //display the info for the currently viewed file with the available options
                var selectedIndex = displayUI.ShowMenu(availableOptions);
                //define the currently selected options from the list of options
                var choice = availableOptions[selectedIndex];

                switch (choice)
                {
                    case "Forrige plukseddel":
                        if (index > 0) index--;
                        break;

                    case "Naeste plukseddel":
                        if (index < files.Count - 1) index++;
                        break;

                    case "Genindlaes pluksedler":
                        files = fileService.GetExportFiles().ToList();
                        index = 0;
                        //Console.WriteLine("Pluklister genindlæst");
                        //Console.WriteLine("Press any key to continue...");
                        //Console.ReadKey(true);
                        break;

                    case "Afslut plukseddel":
                        if (plukliste?.Lines != null)
                        {
                            //[1] for each item where the item type is of type "Print"...
                            foreach (var item in plukliste.Lines.Where(_item => _item.Type == ItemType.Print))
                            {
                                //[2] get the name of the manuel.
                                var templateName = templateMap.TryGetValue(item.ProductID, out var template) ? template : "PRINT-OPGRADE.html";
                                //[3] Try to read the content of the html file and fill out the template
                                // then make a html file in print folder with the name of the customer, followed by the template name
                                try
                                {
                                    var templateContent = fileService.ReadTemplate(templateName);
                                    var rendered = templateRender.RenderFromTemplate(templateContent, plukliste);
                                    fileService.WritePrintOutput($"{plukliste.Name}_{templateName}.html", rendered);
                                }
                                catch (Exception exNoRender)
                                {
                                    Console.WriteLine($"Failed to render/write template for {item.ProductID}: {exNoRender.Message}");
                                }
                            }
                        }
                        //try to move the file to import folder
                        try
                        {
                            fileService.MoveToImport(filePath);
                        }
                        catch (Exception exNoMoveImport)
                        {
                            Console.WriteLine($"Failed to move file to import: {exNoMoveImport.Message}");
                            //Console.WriteLine("Press any key to continue...");
                            //Console.ReadKey(true);
                            break;
                        }

                        Console.WriteLine($"Plukseddel {Path.GetFileName(filePath)} afsluttet.");
                        files.RemoveAt(index);
                        if (index >= files.Count) index = Math.Max(0, files.Count - 1);
                        //Console.WriteLine("Press any key to continue...");
                        //Console.ReadKey(true);
                        break;

                    case "Quit":
                        fileService.DeleteImportIfEmptyOrForce(true); //only delete because this is school project
                        return;
                }
            } // end while
        }

        // unused method
        static string PrintPluklistItems(Pluklist? plukliste)
        {
            if (plukliste == null || plukliste.Lines == null) return string.Empty;
            return string.Join(Environment.NewLine, plukliste.Lines.Select(item => $"<_parser>{item.Amount}x {item.Title} ({item.ProductID})</_parser>"));
        }
    }
}