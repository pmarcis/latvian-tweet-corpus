using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Twitterizer;

namespace TwitterMonitorConsole
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            string queryFile = null;
            string monitoringObjectFile = null;
            string outPrefix = null;
            int savingInterval = 50000;
            int sleepTimeInSeconds = 6;
            string accessToken = null;
            string accessTokenSecret = null;
            string consumerKey = null;
            string consumerSecret = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-q" && args.Length > i + 1)
                {
                    queryFile = args[i + 1];
                }
                else if (args[i] == "-mo" && args.Length > i + 1)
                {
                    monitoringObjectFile = args[i + 1];
                }
                else if (args[i] == "-o" && args.Length > i + 1)
                {
                    outPrefix = args[i + 1];
                }
                else if (args[i] == "-si" && args.Length > i + 1)
                {
                    savingInterval = Convert.ToInt32(args[i + 1]);
                }
                else if (args[i] == "-st" && args.Length > i + 1)
                {
                    sleepTimeInSeconds = Convert.ToInt32(args[i + 1]);
                }
                else if (args[i] == "-at" && args.Length > i + 1)
                {
                    accessToken = args[i + 1];
                }
                else if (args[i] == "-ats" && args.Length > i + 1)
                {
                    accessTokenSecret = args[i + 1];
                }
                else if (args[i] == "-ck" && args.Length > i + 1)
                {
                    consumerKey = args[i + 1];
                }
                else if (args[i] == "-cs" && args.Length > i + 1)
                {
                    consumerSecret = args[i + 1];
                }
            }
            if (string.IsNullOrWhiteSpace(queryFile)
                || !File.Exists(queryFile)
                || string.IsNullOrWhiteSpace(monitoringObjectFile)
                || !File.Exists(monitoringObjectFile)
                || string.IsNullOrWhiteSpace(outPrefix)
                || string.IsNullOrWhiteSpace(accessToken)
                || string.IsNullOrWhiteSpace(accessTokenSecret)
                || string.IsNullOrWhiteSpace(consumerKey)
                || string.IsNullOrWhiteSpace(consumerSecret))
            {
                Console.Error.WriteLine("Not all mandatory arguments found!");
                Console.Error.WriteLine("Usage: mono TwitterMonitorConsole.exe [ARGS]");
                Console.Error.WriteLine("where [ARGS] are:");
                Console.Error.WriteLine("  -q   [QueryFile]            - path of the query JSON file");
                Console.Error.WriteLine("  -mo  [MonitoringObjectFile] - path of the monitoring object JSON");
                Console.Error.WriteLine("                                file");
                Console.Error.WriteLine("  -o   [OutPrefix]            - prefix of the output files");
                Console.Error.WriteLine("  -si  [SavingInterval]       - the tweet interval that will be used");
                Console.Error.WriteLine("                                to create JSON files(default: 50000)");
                Console.Error.WriteLine("                                (optional)");
                Console.Error.WriteLine("  -st  [SleepTime]            - to obey Twitter\'s rules, a sleep time");
                Console.Error.WriteLine("                                is used (the polite interval(and the");
                Console.Error.WriteLine("                                default) is 6 seconds per request; make");
                Console.Error.WriteLine("                                sure not to set this lower or you will");
                Console.Error.WriteLine("                                risk getting banned from Twitter)");
                Console.Error.WriteLine("                                (optional)");
                Console.Error.WriteLine("  -at  [AccessToken]          - the access token for Twitter API");
                Console.Error.WriteLine("  -ats [AccessTokenSecret]    - the access token secret for Twitter API");
                Console.Error.WriteLine("  -ck  [ConsumerKey]          - the consumer key for Twitter API");
                Console.Error.WriteLine("  -cs  [ConsumerSecret]       - the consumer secret for Twitter API");
            }
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            log.Info("Starting Twitter Monitor Console.");
            TwitterMonitor m = new TwitterMonitor(monitoringObjectFile,queryFile,outPrefix,accessToken,accessTokenSecret,consumerKey,consumerSecret,savingInterval,sleepTimeInSeconds);
            m.Run();
            
            log.Info("Stopping Twitter Monitor Console.");
        }
    }
}
