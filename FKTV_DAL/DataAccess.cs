using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FKTV_DAL
{
    public class DataAccess
    {
        public string GetData()
        {
            String strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Data", "Items.json");
            return File.ReadAllText(path);
        }

        public void UpdateAmount(string json)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Data", "Items.json");
            File.WriteAllText(path, json);
        }
    }
}
