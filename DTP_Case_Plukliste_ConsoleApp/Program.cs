//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag
using System.Xml.Serialization;
namespace Plukliste

{
    class PluklisteProgram
    {

        static void Main()
        {
            ConsoleKeyInfo key;
            List<string> files = new List<string>();
            int index = 0;
            Directory.CreateDirectory("import");
            if (!Directory.Exists("export"))
            {
                Console.WriteLine("Folder not found.");
                Console.WriteLine(Directory.GetCurrentDirectory()+"/export");
                return;
            }
            files = Directory.EnumerateFiles("export").ToList();

            //ACT
            bool isConfirmed = false;
            while (!isConfirmed)
            {
            InitFile:
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
                    var plukliste = (Pluklist?)xmlSerializer.Deserialize(file);

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
                    }
                    file.Close();
                    Console.WriteLine("\n\nOptions:");
                }
                //Print options
                int option = 0;
                int optionsCount = 0;
                isConfirmed = false;
                string cursor = "<-";
                (int right, int top) = Console.GetCursorPosition();
                Console.SetCursorPosition(right, top);
                Console.WriteLine($"Quit{(option == 0 ? cursor : "  ")}");
                Console.WriteLine($"Afslut plukseddel{(option == 1 ? cursor : "  ")}");
                if (index > 0)
                {
                    Console.WriteLine($"Forrige plukseddel{(option == 2 ? cursor : "  ")}");
                }
                if (index < files.Count - 1)
                {
                    Console.WriteLine($"Næste plukseddel{(option == 3 ? cursor : "  ")}");
                }
                Console.WriteLine($"Genindlæs pluksedler{(option == 4 ? cursor : "  ")}");
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                        option = (option == 4 ? 0 : option + 1);
                        break;
                    case ConsoleKey.UpArrow:
                        option = (option == 0 ? 4 : option - 1);
                        break;
                    case ConsoleKey.Enter:
                        switch (option)
                        {
                            case 0:
                                isConfirmed = true;
                                break;
                            case 1:
                                var filewithoutPath = files[index].Substring(files[index].LastIndexOf('\\'));
                                File.Move(files[index], string.Format(@"import\\{0}", filewithoutPath));
                                Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
                                files.Remove(files[index]);
                                if (index == files.Count) index--;
                                goto InitFile;
                                break;


                        }
                        break;
                }
            }
        }
    }
}