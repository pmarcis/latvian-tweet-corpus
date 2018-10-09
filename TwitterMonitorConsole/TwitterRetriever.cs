using LanguageDetection;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Twitterizer;

namespace TwitterMonitorConsole
{
    public class TwitterRetriever
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TwitterRetriever));

        OAuthTokens tokens;
        SearchOptions so;
        LanguageDetector detector;
        public TwitterRetriever(string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
        {
            tokens = new OAuthTokens()
            {
                AccessToken = accessToken,
                AccessTokenSecret = accessTokenSecret,
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };

            so = new SearchOptions();
            so.ResultType = SearchOptionsResultType.Recent;
            so.IncludeEntities = false;
            so.Count = 100;

            detector = new LanguageDetector();
            detector.AddAllLanguages(AppDomain.CurrentDomain.BaseDirectory+"Profiles");
        }

        public TwitterRetriever(string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret, int resultCount)
        {
            tokens = new OAuthTokens()
            {
                AccessToken = accessToken,
                AccessTokenSecret = accessTokenSecret,
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };

            so = new SearchOptions();
            so.ResultType = SearchOptionsResultType.Default;
            so.IncludeEntities = false;
            so.Count = resultCount;
        }

        public List<Tweet> GetTweetsForUser(string user, decimal? sinceId)
        {
            try
            {
                UserTimelineOptions uto = new UserTimelineOptions();
                uto.Count = 1000;
                uto.IncludeRetweets = true;
                uto.ScreenName = user;
                if (sinceId == null) uto.SinceStatusId = 0;
                else uto.SinceStatusId = sinceId.Value;
                TwitterResponse<TwitterStatusCollection> twitterResponse = TwitterTimeline.UserTimeline(tokens, uto); // TwitterAccount.VerifyCredentials(tokens);
                List<Tweet> tweets = new List<Tweet>();

                if (twitterResponse.Result == RequestResult.Success)
                {
                    log.Info("Successfully retrieved results for user \"" + user + "\".");
                    foreach (TwitterStatus status in twitterResponse.ResponseObject)
                    {
                        Tweet t = GetTweetFromStatus(status);
                        if (t != null && t.language != null) tweets.Add(t);
                    }
                }
                else
                {
                    log.Warn("Cannot retrieve results for user \"" + user + "\".");
                }
                return tweets;
            }
            catch (Exception ex)
            {
                log.Warn("Communication error! Cannot retrieve results for user \"" + user + "\".", ex);
                return new List<Tweet>();
            }
        }

        public List<Tweet> GetTweetsForQuery(string query, decimal? sinceId, bool checkWhetherContainsInMessage = false)
        {
            try
            {
                if (sinceId == null) so.SinceId = 0;
                else so.SinceId = sinceId.Value;
                TwitterResponse<TwitterSearchResultCollection> twitterResponse = TwitterSearch.Search(tokens, "\"" + query + "\"", so); // TwitterAccount.VerifyCredentials(tokens);
                List<Tweet> tweets = new List<Tweet>();

                if (twitterResponse.Result == RequestResult.Success)
                {
                    log.Info("Successfully retrieved results for query \"" + query + "\".");
                    foreach (TwitterStatus status in twitterResponse.ResponseObject)
                    {
                        Tweet t = GetTweetFromStatus(status);
                        if (t != null && t.language != null && (!checkWhetherContainsInMessage || t.message.ToLower().Contains(query.ToLower()))) tweets.Add(t);
                    }
                }
                else
                {
                    log.Warn("Cannot retrieve results for query \"" + query + "\".");
                }
                return tweets;
            }
            catch (Exception ex)
            {
                log.Warn("Communication error! Cannot retrieve results for query \"" + query + "\".", ex);
                return new List<Tweet>();
            }
        }

        public Tweet GetTweetFromStatusID(string statusId)
        {
            TwitterResponse<TwitterStatus> twitterResponse = TwitterStatus.Show(tokens, Convert.ToDecimal(statusId));
            if (twitterResponse.Result == RequestResult.Success)
            {
                Tweet t = GetTweetFromStatus(twitterResponse.ResponseObject);
                if (t != null && t.language != null) return t;
            }
            return null;
        }

        public Tweet GetTweetFromStatus(TwitterStatus status)
        {
            Tweet t = new Tweet();
            if (status.Place!=null)
            {
                t.placeName = status.Place.Name;
                t.placeFullName = status.Place.FullName;
                t.placeType = status.Place.PlaceType;

            }
            t.createdAt = status.CreatedDate;
            t.tweetId = status.Id;
            t.inReplyToScreenName = status.InReplyToScreenName;
            t.inReplyToStatusId = status.InReplyToStatusId;
            t.inReplyToUserId = status.InReplyToUserId;
            if (status.RetweetedStatus != null && ((status.Text!=null&&status.Text.StartsWith("RT"))|| (status.FullText != null && status.FullText.StartsWith("RT"))))
            {
                if (!string.IsNullOrWhiteSpace(status.RetweetedStatus.FullText)) Console.WriteLine("Full text stored: " + status.RetweetedStatus.FullText);
                string tmpString = RemoveTwitterTags(string.IsNullOrWhiteSpace(status.RetweetedStatus.FullText) ? status.Text : status.RetweetedStatus.FullText);
                if (!string.IsNullOrWhiteSpace(tmpString)) t.language = detector.Detect(tmpString);
                else t.language = "en";
                t.message = string.IsNullOrWhiteSpace(status.RetweetedStatus.FullText) ? "RT "+status.RetweetedStatus.Text : "RT " + status.RetweetedStatus.FullText;
                t.retweetedId = status.RetweetedStatus.Id;
                if (status.User != null)
                {
                    t.userId = status.User.Id;
                    t.userName = status.User.Name;
                    t.userScreenName = status.User.ScreenName;
                }
                else return null;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(status.FullText)) Console.WriteLine("Full text stored: " + status.FullText);
                string tmpString = RemoveTwitterTags(string.IsNullOrWhiteSpace(status.FullText) ? status.Text : status.FullText);
                if (!string.IsNullOrWhiteSpace(tmpString)) t.language = detector.Detect(tmpString);
                else t.language = "en";
                t.message = string.IsNullOrWhiteSpace(status.FullText) ? status.Text : status.FullText;
                t.retweetedId = null;
                if (status.User != null)
                {
                    t.userId = status.User.Id;
                    t.userName = status.User.Name;
                    t.userScreenName = status.User.ScreenName;
                }
                else return null;

            }
            return t;
        }

        private string RemoveTwitterTags(string text)
        {
            Regex r = new Regex("\\s+");
            StringBuilder sb = new StringBuilder();
            string[] arr = r.Split(text);
            foreach(string s in arr)
            {
                if (s.StartsWith("#") || s.StartsWith("@") || s.StartsWith("$")) continue;
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.Append(s);
            }
            return sb.ToString();
        }
    }
}
