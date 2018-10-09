using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TwitterMonitorConsole
{
    public class Tweet
    {
        public string message=null;
        public uint id = 0;
        public decimal tweetId;
        public DateTime createdAt;
        public string language = null;
        public decimal? inReplyToStatusId = null;
        public decimal? inReplyToUserId = null;
        public string inReplyToScreenName = null;
        public decimal userId = 0;
        public string userName = null;
        public string userScreenName = null;
        public string countryCode = null;
        public string placeName = null;
        public string placeFullName = null;
        public string placeType = null;
        public decimal? retweetedId = null;
        public uint monitoringObjectId1 = 0;
        public uint? monitoringObjectId2 = null;
        public uint? queryId = 0;
        public double? sentiment = null; //Currently not in use (factored out from the public release)
        public Tweet()
        {

        }

        public static List<Tweet> LoadTweets(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return null;
            return JsonConvert.DeserializeObject<List<Tweet>>(File.ReadAllText(file, Encoding.UTF8));
        }

        public static void SaveTweets(List<Tweet> tweetList, string outFile)
        {
            File.WriteAllText(outFile, JsonConvert.SerializeObject(tweetList, Newtonsoft.Json.Formatting.Indented), new UTF8Encoding(false));
        }
    }
}
