using System;
using System.IO;
using System.Linq;
using Plukliste;

namespace DTP_Case_Plukliste_ConsoleApp
{
    // Converts/Parses data from CSV file to pluklist.
    internal class CsvPluklistParser : IPluklistParser
    {
        // sets it so that this parser can only parse if the file in the filePath is of type .csv
        public bool CanParse(string filePath) => string.Equals(Path.GetExtension(filePath), ".csv", StringComparison.OrdinalIgnoreCase);

        public Pluklist Parse(string filePath)
        {
            var lines = File.ReadAllLines(filePath); //read all lines in file
            var items = lines
                       .Skip(1) //skip the CSV Header line (usually line 1)
                       .Where(line => !string.IsNullOrWhiteSpace(line)) //filter out any empty lines
                       .Select(line => line.Split(';')) //split each line into columns by ';'
                       .Select(columns => new Item
                       {
                           ProductID = columns.ElementAtOrDefault(0) ?? string.Empty,
                           //Try to parse the content in column 1 as an enum of ItemType, if it is null, then set it to empty, if that is true, set the type to Fysisk
                           Type = Enum.TryParse<ItemType>(columns.ElementAtOrDefault(1) ?? string.Empty, true, out var type) ? type : ItemType.Fysisk,
                           Title = columns.ElementAtOrDefault(2) ?? string.Empty,
                           //Try to parse the content of column 3 as an int, and if it doens't, set the amount to 0
                           Amount = int.TryParse(columns.ElementAtOrDefault(3), out var amount) ? amount : 0
                       }).ToList();
            
            var name = Path.GetFileName(filePath) ?? string.Empty;
            if (name.Contains("_"))
            {
                //get the name from the file name by starting at "_" and replace all "_" with a space instead
                name = name.Substring(name.IndexOf("_", StringComparison.Ordinal) + 1).Replace("_", " ");
            }
            //Remove the file extension
            name = name.Replace(".csv", "", StringComparison.OrdinalIgnoreCase);
            
            return new Pluklist
            {
                Name = name,
                Forsendelse = "Pickup",
                Lines = items
            };
        }
    }
}
