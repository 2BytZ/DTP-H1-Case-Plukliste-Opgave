//De kørende montører; CSV til plukliste XML og vice versa
using System.Linq.Expressions;
using System.Xml.Serialization;
namespace Plukliste

{
    class PluklisteProgram
    {
        static void Main()
        {
            ConsoleKeyInfo key;
            List<string> files = new List<string>();
            List<string> templates = new List<string>();
            int index = 0;
            Directory.CreateDirectory("import");

            Directory.CreateDirectory("print");

            if (!Directory.Exists("export"))
            {
                Console.WriteLine("Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory() + "/export");
                return;
            }
            files = Directory.EnumerateFiles("export").ToList();

            if (!Directory.Exists("templates"))
            {
                Console.WriteLine("Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory() + "/templates");
                return;
            }
            templates = Directory.EnumerateFiles("templates").ToList();

            Pluklist? plukliste;
        //ACT

        InitFile:
            Console.Clear();
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
            }
            else
            {
                Console.WriteLine($"Plukliste {index + 1} af {files.Count}");
                Console.WriteLine($"file: {files[index]}");

                FileStream file = File.OpenRead(files[index]);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Pluklist));
                plukliste = (Pluklist?)xmlSerializer.Deserialize(file);

                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
                    Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse);
                    //TODO: Add adresse to screen print

                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                    foreach (var item in plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                    file.Close();
                    Console.WriteLine("\n\nOptions:");
                }
                //Print options
                int optionSelected = 0;
                bool isConfirmed = false;
                string cursor = "<-";
                (int right, int top) = Console.GetCursorPosition();
                List<string> options = new List<string>
            {
                "Forrige plukseddel",
                "Naeste plukseddel",
                "Genindlaes pluksedler",
                "Afslut plukseddel",
                "Quit"
            };
                if (index >= 0 == false)
                {
                    options.Remove("Afslut plukseddel");
                }
                if (index > 0 == false)
                {
                    options.Remove("Forrige plukseddel");
                }
                if (index < files.Count - 1 == false)
                {
                    options.Remove("Naeste plukseddel");
                }
                while (!isConfirmed)
                {
                    Console.SetCursorPosition(right, top);

                    for (int i = 0; i < options.Count; i++)
                    {
                        Console.WriteLine(options[i] + $"{(optionSelected == options.IndexOf(options[i]) ? cursor : "  ")}");
                    }
                    key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.DownArrow:
                            optionSelected = (optionSelected == options.Count - 1 ? 0 : optionSelected + 1);
                            break;
                        case ConsoleKey.UpArrow:
                            optionSelected = (optionSelected == 0 ? options.Count - 1 : optionSelected - 1);
                            break;
                        case ConsoleKey.Enter:
                            isConfirmed = true;
                            break;
                    }
                    Console.SetCursorPosition(right, top);
                }
                switch (options[optionSelected])
                {
                    case "Forrige plukseddel":
                        if (index > 0) index--;
                        goto InitFile;
                    case "Naeste plukseddel":
                        if (index < files.Count - 1) index++;
                        goto InitFile;
                    case "Genindlaes pluksedler":
                        files = Directory.EnumerateFiles("export").ToList();
                        index = 0;
                        Console.WriteLine("Pluklister genindlæst");
                        goto InitFile;
                    case "Afslut plukseddel":
                        foreach (var item in plukliste.Lines)
                        {
                            if (item.Type == ItemType.Print)
                            {
                                switch (item.ProductID)
                                {
                                    case "PRINT-OPGRADE":
                                        string newHTMLData;
                                        string htmlData = File.ReadAllText(@"templates\\PRINT-OPGRADE.html");
                                        newHTMLData = htmlData.Replace("[Name]", plukliste.Name);
                                        newHTMLData = newHTMLData.Replace("[Adresse]", plukliste.Adresse);
                                        newHTMLData = newHTMLData.Replace("[Plukliste]", PrintPluklistItems(plukliste));
                                        File.WriteAllText($@"print\\{item.ProductID}.html", newHTMLData);
                                        break;

                                    case "PRINT-OPSIGELSE":
                                        htmlData = File.ReadAllText(@"templates\\PRINT-OPGRADE.html");
                                        newHTMLData = htmlData.Replace("[Name]", plukliste.Name);
                                        newHTMLData = newHTMLData.Replace("[Adresse]", plukliste.Adresse);
                                        newHTMLData = newHTMLData.Replace("[Plukliste]", PrintPluklistItems(plukliste));
                                        File.WriteAllText($@"print\\{item.ProductID}.html", newHTMLData);
                                        break;

                                    case "PRINT-WELCOME":
                                        htmlData = File.ReadAllText(@"templates\\PRINT-OPGRADE.html");
                                        newHTMLData = htmlData.Replace("[Name]", plukliste.Name);
                                        newHTMLData = newHTMLData.Replace("[Adresse]", plukliste.Adresse);
                                        newHTMLData = newHTMLData.Replace("[Plukliste]", PrintPluklistItems(plukliste));
                                        File.WriteAllText($@"print\\{item.ProductID}.html", newHTMLData);
                                        break;
                                }
                            }
                        }
                        //Move files to import directory
                        var filewithoutPath = files[index].Substring(files[index].LastIndexOf('\\'));
                        File.Move(files[index], string.Format(@"import\\{0}", filewithoutPath));
                        Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
                        files.Remove(files[index]);
                        if (index == files.Count) index--;
                        goto InitFile;
                    case "Quit":
                        Directory.Delete("import", true);
                        return;
                }
            }
        }
        static string PrintPluklistItems(Pluklist? plukliste)
        {
            string result = "";
            foreach (var item in plukliste.Lines)
            {
                result += $"<p>{item.Amount}x {item.Title} ({item.ProductID})</p>\n";
            }
            return result;
        }
    }
}