//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag
using FKTV_BLL;
using FKTV_DAL;
using System.Linq.Expressions;
using System.Xml.Serialization;
namespace Plukliste

{
    class PluklisteProgram
    {

        static void Main()
        {
            ConsoleKeyInfo key;
            var plukliste = new Pluklist();
            List<string> files = new List<string>();
            int index = 0;
            //create the import folder at runtime
            Directory.CreateDirectory("import");
            //check if export folder exists already during runtime
            if (!Directory.Exists("export"))
            {
                Console.WriteLine("Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory() + "/export");
                return;
            }
            //enumerate files and convert to list items, put into files list
            files = Directory.EnumerateFiles("export").ToList();

        //ACT

        InitFile:
            Console.Clear();
            //check if export list items is in files list
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
            }
            else
            {//print pluklist info
                
                Console.WriteLine($"Plukliste {index + 1} af {files.Count}"); //display file of number of files
                Console.WriteLine($"file: {files[index]}"); //display file name

                FileStream file = File.OpenRead(files[index]);
                //create an xmlSerializer of type Pluklist
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Pluklist));
                //put file list items, parsed as type Pluklist, into plukliste list
                plukliste = (Pluklist?)xmlSerializer.Deserialize(file); //Deserialize list items in file list

                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name); //display Name of persons order
                    Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse); //display by which carrier the order is sent
                    Console.WriteLine("{0,-13}{1}", "Adresse:", plukliste.Adresse); //display address to which the package is sent
                    //print info of the order
                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3,-32}{4}", "Antal", "Type", "Produktnr.", "Navn", "Paa Lager");
                    foreach (var item in plukliste.Lines)
                    {
                        ItemsRepository storageAmount = new ItemsRepository();
                        //Get and display the current Pluklist item's amount, type, productID, Title and amount in storage
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3,-32}{4}", 
                            item.Amount, 
                            item.Type, 
                            item.ProductID, 
                            item.Title, 
                            //get storage amount from html database based on productID
                            storageAmount.GetStorageAmount(item.ProductID)); 
                    }
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
            //remove "Afslut plukseddel" option if no pluklist items are available
            if (index >= 0 == false)
            {
                options.Remove("Afslut plukseddel");
            }
            //Remove "forrige plukseddel" option if there are no pluklist items before the current one
            if (index > 0 == false)
            {
                options.Remove("Forrige plukseddel");
            }
            //remove "naeste plukseddel" option if there are no pluklist items left after the current one
            if (index < files.Count - 1 == false)
            {
                options.Remove("Naeste plukseddel");
            }
            while (!isConfirmed)
            {
                Console.SetCursorPosition(right, top);
                //write available option with the cursor if the option is selected
                for (int i = 0; i < options.Count; i++)
                {
                    Console.WriteLine(options[i] + $"{(optionSelected == options.IndexOf(options[i]) ? cursor : "  ")}");
                }
                key = Console.ReadKey(true);

                //control logic
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
            //Options logic
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
                    //Update storage
                    ItemsRepository itemsRepository = new ItemsRepository();
                    foreach (var line in plukliste.Lines)
                    {
                        //update the database for the amount in lager
                        itemsRepository.UpdateAmount(line.ProductID, line.Amount, true);
                        DataAccess access = new DataAccess();
                        access.SyncData(true); //sync data so it matches across html and pluklist
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
}