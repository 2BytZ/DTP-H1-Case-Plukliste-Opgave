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
            var ui = new ConsoleUi();

            if (!fileService.ExportExists())
            {
                Console.WriteLine("(!) Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory() + "/export");
                return;
            }

            if (!fileService.TemplateExists())
            {
                Console.WriteLine("(!) Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory() + "/templates");
                return;
            }

            var parsers = new IPluklistParser[]
            {
                    new CsvPluklistParser(),
                    new XmlPluklistParser()
            };

            var templateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "PRINT-OPGRADE", "PRINT-OPGRADE.html" },
                    { "PRINT-OPSIGELSE", "PRINT-OPGRADE.html" },
                    { "PRINT-WELCOME", "PRINT-OPGRADE.html" }
                };

            var files = fileService.GetExportFiles().ToList();
            int index = 0;

            while (files.Count > 0)
            {
                var filePath = files[index];

                // choose parser for file
                var parser = parsers.FirstOrDefault(p => p.CanParse(filePath));
                if (parser == null)
                {
                    Console.WriteLine("(!) Unsupported file format. Skipping..");
                    files.RemoveAt(index);
                    if (index >= files.Count) index = Math.Max(0, files.Count - 1);
                    continue;
                }

                Pluklist? plukliste;
                try
                {
                    plukliste = parser.Parse(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to parse file: {ex.Message}");
                    files.RemoveAt(index);
                    if (index >= files.Count) index = Math.Max(0, files.Count - 1);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    continue;
                }

                // display via UI
                ui.DisplayPluklist(plukliste, filePath, index, files.Count);

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
                if (index <= 0) availableOptions.Remove("Forrige plukseddel");
                if (index >= files.Count - 1) availableOptions.Remove("Naeste plukseddel");

                var selectedIndex = ui.ShowMenu(availableOptions);
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
                        Console.WriteLine("Pluklister genindlæst");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case "Afslut plukseddel":
                        if (plukliste?.Lines != null)
                        {
                            foreach (var item in plukliste.Lines.Where(l => l.Type == ItemType.Print))
                            {
                                var templateName = templateMap.TryGetValue(item.ProductID, out var t) ? t : "PRINT-OPGRADE.html";
                                try
                                {
                                    var templateContent = fileService.ReadTemplate(templateName);
                                    var rendered = templateRender.RenderFromTemplate(templateContent, plukliste);
                                    fileService.WritePrintOutput($"{item.ProductID}.html", rendered);
                                }
                                catch (Exception tex)
                                {
                                    Console.WriteLine($"Failed to render/write template for {item.ProductID}: {tex.Message}");
                                }
                            }
                        }

                        try
                        {
                            fileService.MoveToImport(filePath);
                        }
                        catch (Exception mx)
                        {
                            Console.WriteLine($"Failed to move file to import: {mx.Message}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }

                        Console.WriteLine($"Plukseddel {Path.GetFileName(filePath)} afsluttet.");
                        files.RemoveAt(index);
                        if (index >= files.Count) index = Math.Max(0, files.Count - 1);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case "Quit":
                        fileService.DeleteImportIfEmptyOrForce(true);
                        return;
                }
            } // end while
        }

        static string PrintPluklistItems(Pluklist? plukliste)
        {
            if (plukliste == null || plukliste.Lines == null) return string.Empty;
            return string.Join(Environment.NewLine, plukliste.Lines.Select(item => $"<p>{item.Amount}x {item.Title} ({item.ProductID})</p>"));
        }
    }
}