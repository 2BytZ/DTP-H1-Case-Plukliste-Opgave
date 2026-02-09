using Plukliste;

namespace DTP_Case_Plukliste_ConsoleApp
{
    internal interface IPluklistParser
    {
        bool CanParse(string filePath); //used to determine if the parser can parse the file based on its extension (aka. filetype)
        Pluklist Parse(string filePath); //used to determine how to parse the file, and then parse it into a Pluklist object
    }
}
