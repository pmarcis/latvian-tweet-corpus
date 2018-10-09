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
    public class Query
    {
        public uint id;
        public string query;
        public int sleepTime;
        public uint monitoringObjectId1;
        public uint? monitoringObjectId2;
        public decimal? lastId;
        public Query()
        {
            
            id = 0;
            query = null;
            sleepTime = 5;
            monitoringObjectId1 = 0;
            monitoringObjectId2 = null;
            lastId = null;
        }


        public static List<Query> Load(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return null;
            return JsonConvert.DeserializeObject<List<Query>>(File.ReadAllText(file, Encoding.UTF8));
        }
        
        public static void SaveQueries(List<Query> queryList, string outFile)
        {
            File.WriteAllText(outFile, JsonConvert.SerializeObject(queryList, Newtonsoft.Json.Formatting.Indented), new UTF8Encoding(false));
        }
    }
}
