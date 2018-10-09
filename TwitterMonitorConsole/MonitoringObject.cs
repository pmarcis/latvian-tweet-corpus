using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TwitterMonitorConsole
{
    public class MonitoringObject
    {
        public uint id;
        public string name;
        public string twitterAccount;
        public string type;
        public uint? parentId;
        public decimal? lastId;

        public MonitoringObject()
        {
            id = 0;
            name = null;
            twitterAccount = null;
            parentId = null;
            lastId = null;
        }

        public static List<MonitoringObject> Load(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return null;
            return JsonConvert.DeserializeObject<List<MonitoringObject>>(File.ReadAllText(file, Encoding.UTF8));
        }

        public static void SaveMonitoringObjects(List<MonitoringObject> monitoringObjectList, string outFile)
        {
            File.WriteAllText(outFile, JsonConvert.SerializeObject(monitoringObjectList, Newtonsoft.Json.Formatting.Indented), new UTF8Encoding(false));
        }

    }
}
