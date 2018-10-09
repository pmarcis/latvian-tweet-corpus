# latvian-tweet-corpus

This repository is supposed to contain two things:

* The **Latvian Tweet Corpus** - a collection of tweets that I am continuously collecting for various sentiment analysis and computational social science application purposes (mostly in Latvian). Due to the size of the corpus, I cannot share it openly, but if you will contact me directly with describing what you intend to do with it, we will definitely find a way how I can share the data with you.
* Tools for tweet corpus collection. I have factored the **Twitter Monitor**, the solution that collects the tweets, out of my experimental projects that can be used to collect tweets from Twitter. So ... if you are interested in the **Twitter Monitor**, read further.

If you are using the **Latvian Tweet Corpus** or the **Twitter Monitor** and you happen to publish anything, please be so kind and reference the following paper:

```bibtex
@inproceedings{Pinnis2018,
 address = {Tartu, Estonia},
 author = {Pinnis, Mārcis},
 booktitle = {Human Language Technologies – The Baltic Perspective - Proceedings of the Seventh International Conference Baltic HLT 2018},
 doi = {10.3233/978-1-61499-912-6-112},
 keywords = {latvian,sentiment analysis,social networks,tweet corpus},
 pages = {112--119},
 publisher = {IOS Press},
 title = {{Latvian Tweet Corpus and Investigation of Sentiment Analysis for Latvian}},
 year = {2018}
}
```

# Twitter Monitor

The **Twitter Monitor** is a console application that provides functionality for continuous monitoring of tweets from a pre-defined list of Twitter users and queries (both can be specified).

The **Twitter Monitor** adapts to the frequency of how users tweet, meaning that users/queries who/that produce tweets more frequently will be monitored more frequently than those who/that tweet less frequently.

## Deployment

The GitHub repository contains a compiled version of the **Twitter Monitor**.

Just:

```bash
git clone https://github.com/pmarcis/latvian-tweet-corpus.git
```

The compiled version is in the `CompiledVersion` folder.

On Windows you will need the [.NET Framework 4.5.2](https://www.microsoft.com/net/download/dotnet-framework-runtime/net452).

On Linux you will need _mono_ (`sudo apt-get install mono-complete`).

## Building from Source

For those who know what _.NET_ and _C#_ is: skip this part!

### On Windows

Install Visual Studio (the Community Eddition is free...) and then:

`git clone https://github.com/pmarcis/latvian-tweet-corpus.git` -> open `TwitterMonitor.sln` -> select `Build` and `Build Solution`

The executable files will be in the `TwitterMonitorConsole\bin\Release` folder.

### On Linux

You will need _mono_. To install mono, execute:

```bash
sudo apt-get install mono-complete
```

Then:

```bash
git clone https://github.com/pmarcis/latvian-tweet-corpus.git
cd latvian-tweet-corpus
msbuild /p:Configuration=Release TwitterMonitor.sln
cd TwitterMonitorConsole/bin/Release
```

## Usage

To use the Twitter Monitor, you have to:

* Acquire access details in order to be able to use the Twitter API. There are four strings you need to acquire - an access token and access token secret (for your application), and a consumer key and a consumer secret (so that your application can impersonate you) . For more details, head over to [apps.twitter.com](https://apps.twitter.com).

* Create a monitoring object JSON file (a file that lists the Twitter users that you will monitor). There is an example [here](TwitterMonitorConsole/monitoring_object_example.json).

* Create a query JSON file (a file that lists the queries that you will monitor). There is an example [here](TwitterMonitorConsole/query_example.json). Note that it is advisable to list queries that are not too ambiguous, otherwise you will end up with a lot of irrelevant garbage in your data!

To launch the Twitter Monitor on Windows, execute:

```bash
.\TwitterMonitorConsole.exe -q query_example.json -mo monitoring_object_example.json -o tweets -si 50000 -st 6 -at [AccessToken] -ats [AccesstokenSecret] -ck [ConsumerKey] -cs [ConsumerSecret]
```

The following arguments are needed:

* `-q [QueryFile]` - path of the query JSON file.
* `-mo [MonitoringObjectFile]` - path of the monitoring object JSON file.
* `-o   [OutPrefix]` - prefix of the output files (Twitter Monitor will output 3 JSON files (tweets, queries, and monitoring objects) after _X_ (specified by `-si`) collected tweets and the `-o` prefix specifies where to save the JSON files (the ending fill have a timestamp - so ... no worries about possibly overwriting existing data).
* `-si  [SavingInterval]` - the tweet interval that will be used to create JSON files (default: 50000) (optional). If this number is higher than 1000, there will be an initial set of JSONs created after the first 1000 collected tweets.
* `-st  [SleepTime]` - to obey Twitter's rules, a sleep time is used (the polite interval (and the default) is 6 seconds per request (Twitter may change its policies though ... so this is not carved in stone); make sure not to set this lower or you will risk getting banned from Twitter) (optional).
* `-at  [AccessToken]` - the access token for Twitter API.
* `-ats [AccessTokenSecret]` - the access token secret for Twitter API.
* `-ck  [ConsumerKey]` - the consumer key for Twitter API.
* `-cs  [ConsumerSecret]` - the consumer secret for Twitter API.

To launch the Twitter Monitor on Linux, execute:

```bash
mono TwitterMonitorConsole.exe -q query_example.json -mo monitoring_object_example.json -o tweets -si 50000 -st 6 -at [AccessToken] -ats [AccesstokenSecret] -ck [ConsumerKey] -cs [ConsumerSecret]
```

The **Twitter Monitor** can be gracefully stopped by creating a file named `stop` in the folder where the executable file is located. This will trigger the creation of the last set of JSON files.

## Integrated Libraries

The **Tweet Monitor** uses the following third party libraries:

* [LanguageDetection](https://github.com/pdonald/language-detection) - licence [here](DLLs/LanguageDetection.license.txt).
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - licence [here](DLLs/Newtonsoft.Json.license.md).
* [log4net](https://logging.apache.org/log4net/) - licence [here](DLLs/log4net.license.txt).
* [Twitterizer2](https://github.com/Twitterizer/Twitterizer) - licence [here](DLLs/Twitterizer2.license.txt).
