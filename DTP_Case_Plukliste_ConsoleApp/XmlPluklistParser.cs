using Plukliste;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DTP_Case_Plukliste_ConsoleApp
{
    internal class XmlPluklistParser : IPluklistParser
    {
        // sets it so that this parser can only parse if the file in the filePath is of type .xml
        public bool CanParse(string filePath) => string.Equals(Path.GetExtension(filePath), ".xml", StringComparison.OrdinalIgnoreCase);

        public Pluklist Parse(string filePath)
        {
            using (FileStream file = File.OpenRead(filePath))
            {
                //initialize new XmlSerializer with type of Pluklist.
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Pluklist));
                return (Pluklist?)xmlSerializer.Deserialize(file) ?? new Pluklist();  //Deserialize the XML file.
            }
        }
    }
}
