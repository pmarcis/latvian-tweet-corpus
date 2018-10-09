# latvian-tweet-corpus

This repository is supposed to contain two things:

* The **Latvian Tweet Corpus** - a collection of tweets that I am continuously collecting for various sentiment analysis and computational social science application purposes (mostly in Latvian). Due to the size of the corpus, I cannot share it openly, but if you will contact me directly with describing what you intend to do with it, we will definitely find a way how I can share the data with you.
* Tools for a tweet corpus collection. I have factored the **Twitter Monitor**, the solution that collects the tweets, out of my experimental projects that can be used to collect tweets from Twitter. So ... if you are interested in the **Twitter Monitor**, read further.

If you are using the Latvian Tweet Corpus or the **Twitter Monitor** and you happen to publish anything, please be so kind and reference the following paper:

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

The **Twitter Monitor** adapts to the frequency of how users tweet, meaning that users/queries who/that produce tweets more frequently will be monitored more frequently than those that/who tweet less frequently.

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

For those who know what .NET and C# is: skip this part!

### On Windows

To use the Twitter Monitor on Windows, install Visual Studio (the Community Eddition is free...) and open the project.

`git clone ...` -> open `TwitterMonitor.sln` -> select `Build` and `Build Solution`

The executable files will be in the 

### On Linux

To use the Twitter Monitor on Linux, you will need _mono_. To install mono, execute: `sudo apt-get install mono-complete`.

Then:
```bash
git clone https://github.com/pmarcis/latvian-tweet-corpus.git
cd latvian-tweet-corpus/TwitterMonitorConsole
xbuild /p:Configuration=Release TwitterMonitorConsole.csproj
```

## Usage

To use the Twitter Monitor, you have to:

* Acquire access details in order to be able to use the Twitter API. There are four strings you need to acquire - an access token and access token secret (for your application), and a consumer key and a consumer secret (so that your application can impersonate you) . For more details, head over to [apps.twitter.com](https://apps.twitter.com).

* Create a monitoring object JSON file (the file that lists the Twitter users that you will monitor). There is an example [here](TwitterMonitorConsole/monitoring_object_example.json).

* Create a query JSON file (the file that lists the queries that you will monitor). There is an example [here](TwitterMonitorConsole/query_example.json). Note that it is adviseable to list queries that are not too ambiguous, otherwise you will end up with a lot of irrelevant garbage in your data!

To launch the Twitter Monitor on Windows, execute:

```bash
.\TwitterMonitorConsole.exe -q query_example.json -mo monitoring_object_example.json -o tweets -si 50000 -st 6 -at [AccessToken] -ats [AccesstokenSecret] -ck [ConsumerKey] -cs [ConsumerSecret]
```

To launch the Twitter Monitor on Linux, execute:
mono TwitterMonitorConsole.exe -q query_example.json -mo monitoring_object_example.json -o tweets -si 50000 -st 6 -at [AccessToken] -ats [AccesstokenSecret] -ck [ConsumerKey] -cs [ConsumerSecret]
