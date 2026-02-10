using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FKTV_DAL
{
    public class DataAccess
    {
        public string GetData()
        {
            var strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Data", "Items.json");
            return File.ReadAllText(path);
        }

        public void UpdateAmount(string json)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Data", "Items.json");
            File.WriteAllText(path, json); // this exe. json
        }

        public void SyncData(bool calledFromPluklist = false)
        {   
            if (calledFromPluklist == false)
            {
                var strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = Path.Combine(path, "Data", "Items.json");
                var FKTV_DALJson = Path.Combine(path.Substring(0, path.IndexOf("Story3_Lager")), "Story3_Lager\\FKTV_DAL\\Data\\Items.json"); //this json
                var FKTVExeJson = File.ReadAllText(path); //database executable json
                var PluklisteExeJson = Path.Combine(path.Substring(0, path.IndexOf("Story3_Lager")), "Story3_Lager\\DTP_Case_Plukliste_ConsoleApp\\bin\\Debug\\net10.0\\Data\\Items.json"); ; //plukliste executable json

                if (!File.ReadAllText(PluklisteExeJson).Equals(FKTVExeJson) || !FKTV_DALJson.Equals(FKTVExeJson))
                {
                    File.WriteAllText(PluklisteExeJson, FKTVExeJson);
                    File.WriteAllText(FKTV_DALJson, FKTVExeJson);
                }
            }
            else if (calledFromPluklist == true)
            {
                var PluklisteExeJson = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data","Items.json");
                var FKTVExeJson = Path.Combine(PluklisteExeJson.Substring(0, PluklisteExeJson.IndexOf("Story3_Lager")), "Story3_Lager\\FKTV\\bin\\Debug\\net10.0\\Data\\Items.json"); //plukliste executable json
                var FKTV_DALJson = Path.Combine(PluklisteExeJson.Substring(0, PluklisteExeJson.IndexOf("Story3_Lager")), "Story3_Lager\\FKTV_DAL\\Data\\Items.json"); //this json
                File.WriteAllText(FKTVExeJson, File.ReadAllText(PluklisteExeJson));
                File.WriteAllText(FKTV_DALJson, File.ReadAllText(PluklisteExeJson));

            }

        }
    }
}
