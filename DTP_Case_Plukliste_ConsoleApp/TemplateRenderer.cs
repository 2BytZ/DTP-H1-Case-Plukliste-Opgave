using System;
using System.Collections.Generic;
using System.Text;

namespace Plukliste
{
    // Render Html files to put into print folder.
    public class TemplateRenderer
    {
        public string RenderPluklistItems(Pluklist plukliste)
        {
            var stringBuilder = new StringBuilder(); //sounds like a machine Dr. Doofenschmurtz would make xd
            if (plukliste?.Lines == null) //check if items in plukliste is empty
            {
                return string.Empty;
            }
            foreach (var item in plukliste.Lines)
            {
                //saves the Amount, Title and ProductID of every item in a string
                stringBuilder.AppendLine($"<p>{item.Amount}x {item.Title} ({item.ProductID})</p>");  //saves a line in a variable of stringBuilder class, without actually writing it anywhere
            }
            return stringBuilder.ToString(); //saves it as a string, and returns
        }

        public string RenderFromTemplate(string templateContent, Pluklist plukliste)
        {
            if (templateContent == null)
            {
                return string.Empty;
            }
            return templateContent
                .Replace("[Name]", plukliste?.Name ?? string.Empty) //?? meaning "if null(?) then(?) write (string.Empty)"
                .Replace("[Adresse]", plukliste?.Adresse ?? string.Empty)
                .Replace("[Plukliste]", RenderPluklistItems(plukliste));
        }
    }
}
