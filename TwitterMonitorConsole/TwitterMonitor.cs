using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterMonitorConsole
{
    public class TwitterMonitor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TwitterMonitor));

        public List<MonitoringObject> monitoringObjects = null;
        public List<Query> queries = null;
        TwitterRetriever tr = null;
        string jsonOutFilePrefix = null;
        int tweetSaveInterval = 50000;
        int sleepInterval = 6;
        public TwitterMonitor()
        {

        }
        public TwitterMonitor(string monitoringObjectJson, string queryJson, string jsonFilePrefix, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret, int savingInterval = 50000, int sleepTimeInSeconds = 6)
        {
            if (string.IsNullOrWhiteSpace(accessToken)||string.IsNullOrWhiteSpace(accessTokenSecret) || string.IsNullOrWhiteSpace(consumerKey)|| string.IsNullOrWhiteSpace(consumerSecret))
            {
                throw new ArgumentNullException("Twitter access details not provided! AccessToken, AccessTokenSecret, ConsumerKey, or ConsumerSecret is/are not given!");
            }
            tweetSaveInterval = savingInterval;
            sleepInterval = sleepTimeInSeconds;
            tr = new TwitterRetriever(accessToken,accessTokenSecret,consumerKey,consumerSecret);
            if (!string.IsNullOrWhiteSpace(monitoringObjectJson) && File.Exists(monitoringObjectJson))
            {
                monitoringObjects = MonitoringObject.Load(monitoringObjectJson);
                log.Info("Found " + monitoringObjects.Count.ToString() + " objects to monitor.");
            }
            if (!string.IsNullOrWhiteSpace(queryJson) && File.Exists(queryJson))
            {
                queries = Query.Load(queryJson);
                log.Info("Found " + queries.Count.ToString() + " queries for monitoring.");
            }
            jsonOutFilePrefix = jsonFilePrefix;
        }


        public void Run()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Starting Twitter Monitor.");
            Dictionary<int, int> queryDefaultSleepTimes = new Dictionary<int, int>();
            Dictionary<int, int> queryCurrentSleepTimes = new Dictionary<int, int>();
            List<KeyValuePair<int, DateTime>> queryList = new List<KeyValuePair<int, DateTime>>();
            for(int i=0;i<queries.Count;i++)
            {
                queryDefaultSleepTimes.Add(i, queries[i].sleepTime);
                queryCurrentSleepTimes.Add(i, queries[i].sleepTime);
                queryList.Add(new KeyValuePair<int, DateTime>(i, DateTime.Now));
            }


            Dictionary<int, int> objectCurrentSleepTimes = new Dictionary<int, int>();
            List<KeyValuePair<int, DateTime>> objectList = new List<KeyValuePair<int, DateTime>>();
            for (int i = 0; i < monitoringObjects.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(monitoringObjects[i].twitterAccount))
                {
                    objectCurrentSleepTimes.Add(i, 60);
                    objectList.Add(new KeyValuePair<int, DateTime>(i, DateTime.Now));
                }
            }

            string stopFile = AppDomain.CurrentDomain.BaseDirectory;
            TimeSpan sleep = new TimeSpan(0,0, sleepInterval);
            if (!stopFile.EndsWith(Path.DirectorySeparatorChar.ToString()))stopFile+=Path.DirectorySeparatorChar.ToString();
            stopFile += "stop";
            List<Tweet> currTweetList = new List<Tweet>();
            int counter = 0;
            bool firstSave = true;
            while (!File.Exists(stopFile))//A whacky way to stop a programm, but a very useful one!
            {
                queryList.Sort((x, y) => x.Value.CompareTo(y.Value));
                objectList.Sort((x, y) => x.Value.CompareTo(y.Value));
                if (DateTime.Now>queryList[0].Value)
                {
                    bool checkMessageBody = !queries[queryList[0].Key].query.StartsWith("@");
                    
                    List<Tweet> tweets = tr.GetTweetsForQuery(queries[queryList[0].Key].query,queries[queryList[0].Key].lastId,checkMessageBody);

                    if (tweets.Count > 0)
                    {
                        log.Info("Found " + tweets.Count.ToString() + " tweets for query " + queries[queryList[0].Key].id.ToString() + ".");
                        decimal lastId = 0;
                        foreach (Tweet t in tweets)
                        {
                            t.monitoringObjectId1 = queries[queryList[0].Key].monitoringObjectId1;
                            t.monitoringObjectId2 = queries[queryList[0].Key].monitoringObjectId2;
                            t.queryId = queries[queryList[0].Key].id;
                            currTweetList.Add(t);
                            counter++;
                            if (t.tweetId > lastId) lastId = t.tweetId;
                        }
                        if (lastId > 0)
                        {
                            queries[queryList[0].Key].lastId = lastId;
                        }
                        queryCurrentSleepTimes[queryList[0].Key] = queryDefaultSleepTimes[queryList[0].Key];
                        log.Info("Resetting sleep time for query " + queries[queryList[0].Key].id.ToString() + " to " + queryCurrentSleepTimes[queryList[0].Key] + "."); 
                    }
                    else
                    {
                        if (queryCurrentSleepTimes[queryList[0].Key] < 2880) //48 hours are the maximum to wait!B
                            queryCurrentSleepTimes[queryList[0].Key] *= 2;
                        log.Info("Increasing sleep time for query " + queries[queryList[0].Key].id.ToString() + " to " + queryCurrentSleepTimes[queryList[0].Key] + ".");
                    }
                    queryList[0] = new KeyValuePair<int, DateTime>(queryList[0].Key, DateTime.Now.AddMinutes(queryCurrentSleepTimes[queryList[0].Key]));
                }
                System.Threading.Thread.Sleep(sleep);
                if (DateTime.Now > objectList[0].Value)
                {
                    List<Tweet> tweets = new List<Tweet>();
                    if (monitoringObjects[objectList[0].Key].twitterAccount.Contains(";"))
                    {
                        char[] sep = new char[] { ';' };
                        string[] accounts = monitoringObjects[objectList[0].Key].twitterAccount.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string account in accounts)
                        {
                            tweets.AddRange(tr.GetTweetsForUser(account, monitoringObjects[objectList[0].Key].lastId));
                        }
                    }
                    else
                    {
                        tweets = tr.GetTweetsForUser(monitoringObjects[objectList[0].Key].twitterAccount, monitoringObjects[objectList[0].Key].lastId);
                    }
                    if (tweets.Count > 0)
                    {
                        log.Info("Found " + tweets.Count.ToString() + " tweets for user " + monitoringObjects[objectList[0].Key].twitterAccount + ".");
                        decimal lastId = 0;
                        foreach (Tweet t in tweets)
                        {
                            t.monitoringObjectId1 = monitoringObjects[objectList[0].Key].id;
                            t.monitoringObjectId2 = monitoringObjects[objectList[0].Key].parentId;
                            t.queryId = null;
                            currTweetList.Add(t);
                            counter++;
                            if (t.tweetId > lastId) lastId = t.tweetId;
                        }
                        if (lastId > 0)
                        {
                            monitoringObjects[objectList[0].Key].lastId = lastId;
                        }
                        objectCurrentSleepTimes[objectList[0].Key] = 60;
                        log.Info("Resetting sleep time for user " + monitoringObjects[objectList[0].Key].twitterAccount + " to " + 60 + ".");

                    }
                    else
                    {
                        if (objectCurrentSleepTimes[objectList[0].Key] < 2880) //48 hours are the maximum to wait!
                            objectCurrentSleepTimes[objectList[0].Key] *= 2;
                        log.Info("Increasing sleep time for user " + monitoringObjects[objectList[0].Key].twitterAccount + " to " + objectCurrentSleepTimes[objectList[0].Key].ToString() + ".");

                    }
                    objectList[0] = new KeyValuePair<int, DateTime>(objectList[0].Key, DateTime.Now.AddMinutes(objectCurrentSleepTimes[objectList[0].Key]));
                    System.Threading.Thread.Sleep(sleep);
                }
                if (counter>=1000&& firstSave && tweetSaveInterval>1000)
                {
                    firstSave = false;
                    DateTime time = DateTime.Now;
                    string date_suffix = time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString() + "-" + time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();
                    Tweet.SaveTweets(currTweetList, jsonOutFilePrefix + "-" + date_suffix + ".json");
                    Query.SaveQueries(queries, jsonOutFilePrefix + "-queries-" + date_suffix + ".json");
                    MonitoringObject.SaveMonitoringObjects(monitoringObjects, jsonOutFilePrefix + "-monitoring-objects-" + date_suffix + ".json");
                    currTweetList.Clear();
                }
                else if (currTweetList.Count>tweetSaveInterval)
                {
                    DateTime time = DateTime.Now;
                    string date_suffix = time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString() + "-" + time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();
                    Tweet.SaveTweets(currTweetList, jsonOutFilePrefix + "-" + date_suffix + ".json");
                    Query.SaveQueries(queries, jsonOutFilePrefix + "-queries-" + date_suffix + ".json");
                    MonitoringObject.SaveMonitoringObjects(monitoringObjects, jsonOutFilePrefix + "-monitoring-objects-" + date_suffix + ".json");
                    currTweetList.Clear();
                }
            }

            DateTime finalTime = DateTime.Now;
            string finalDateSuffix = finalTime.Day.ToString() + "-" + finalTime.Month.ToString() + "-" + finalTime.Year.ToString() + "-" + finalTime.Hour.ToString() + "-" + finalTime.Minute.ToString() + "-" + finalTime.Second.ToString();
            Tweet.SaveTweets(currTweetList, jsonOutFilePrefix + "-" + finalDateSuffix + ".json");
            Query.SaveQueries(queries, jsonOutFilePrefix + "-queries-" + finalDateSuffix + ".json");
            MonitoringObject.SaveMonitoringObjects(monitoringObjects, jsonOutFilePrefix + "-monitoring-objects-" + finalDateSuffix + ".json");
            currTweetList.Clear();
            log.Info("Stopping Twitter Monitor.");
        }
    }
}
