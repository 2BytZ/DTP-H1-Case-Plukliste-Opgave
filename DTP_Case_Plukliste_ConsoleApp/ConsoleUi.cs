using System;
using System.Collections.Generic;
using System.Text;

namespace Plukliste
{
    //layout and display data
    public class ConsoleUi
    {
        public void DisplayPluklist(Pluklist plukliste, string filePath, int index, int total)
        {
            Console.Clear();
            Console.WriteLine($"Plukliste {index + 1} af {total}");
            Console.WriteLine($"file: {filePath}");

            if (plukliste == null)
            {
                Console.WriteLine("\n(!) No plucklist to display.");
                return;
            }

            Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
            Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse);
            if (plukliste.Forsendelse != "Pickup")
            {
                Console.WriteLine("{0, -13}{1}", "To-Adresse:", plukliste.Adresse);
            }
            Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");

            if (plukliste.Lines != null)
            {
                foreach (var item in plukliste.Lines)
                {
                    Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                }
            }
            Console.WriteLine();
        }
        //navigation
        public int ShowMenu(List<string> options, int initialSelected = 0)
        {
            if (options == null || options.Count == 0)
            {
                throw new ArgumentException("(!) Options must contain at least one item.", nameof(options));
            }
            //find the selected option based on how much the initialSelected has risen (from 0)
            int selected = Math.Clamp(initialSelected, 0, options.Count - 1); 
            ConsoleKeyInfo key;

            RenderOptions(options, selected);

            while (true)
            {
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                        selected = (selected == options.Count - 1 ? 0 : selected + 1);   //this is too confusing to read --> (selected = (selected + 1) % options.Count;)
                        break;
                    case ConsoleKey.UpArrow:
                        selected = (selected == 0 ? options.Count - 1 : selected - 1);
                        break;
                    case ConsoleKey.Enter:
                        // leave the cursor just after menu for clarity
                        Console.WriteLine();
                        return selected;
                }

                MoveCursorUp(options.Count);
                RenderOptions(options, selected);
            }
        }
        private void RenderOptions(List<string> options, int selected)
        {
            for (int i = 0; i < options.Count; i++)
            {
                //display the option with the cursor to the right of the selected
                var cursor = i == selected ? "<-" : "  ";
                var line = options[i] + cursor;
                Console.WriteLine(line);
            }
        }

        private void MoveCursorUp(int lines)
        {
            try
            {
                var (left, top) = Console.GetCursorPosition();
                var newTop = Math.Max(0, top - lines);
                Console.SetCursorPosition(0, newTop);
            }
            catch (Exception)
            {
                Console.Clear();
            }
        }
    }
}
