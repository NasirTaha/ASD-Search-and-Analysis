using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
  public  class BingWebSearcher  
    {                
        private static string template = @"https://api.cognitive.microsoft.com/bing/v5.0/search?q={0}&count=5&offset=0&mkt=en-us&safesearch=Moderate";
        private static HtmlWeb web = new HtmlWeb();

        //Subscription-Key
        private static string SubscriptionKey = "3bd34c9d50a6430fabe06057617e2e66";

     
        public static List<SearchResult> Search(string queryFilePath)
        {            
            var json = GetBingSearchJsonResult(queryFilePath);
            var result = JsonConvert.DeserializeObject<Rootobject>(json);

            return result?.webPages.value.Select(item => new SearchResult {
                Name = item.name,
                Link = item.url
            }).ToList();
        }


       
        private static string GetBingSearchJsonResult(string keywords)
        {
            string JsonString = null;
            string url = string.Format(template, keywords);

            using (var client = new WebClient())
            {
                //client.Headers[HttpRequestHeader.UserAgent] = "some user agent if you wish";

                client.Headers[HttpRequestHeader.AcceptLanguage] = "es-ES";                
                client.Headers["Ocp-Apim-Subscription-Key"] = SubscriptionKey;
                string html = client.DownloadString(url);
                
                // feed the HTML to HTML Agility Pack
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                JsonString = doc.DocumentNode.InnerText;
                return JsonString;
            }
        }
    }
}

public class SearchResult
{
    public string Name { get; set; }
    public string Link { get; set; }
}



public class Rootobject
{
    public string _type { get; set; }
    public Webpages webPages { get; set; }
    public Rankingresponse rankingResponse { get; set; }
}

public class Webpages
{
    public string webSearchUrl { get; set; }
    public int totalEstimatedMatches { get; set; }
    public Value[] value { get; set; }
}

public class Value
{
    public string id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string displayUrl { get; set; }
    public string snippet { get; set; }
    public DateTime dateLastCrawled { get; set; }
}

public class Rankingresponse
{
    public Mainline mainline { get; set; }
}

public class Mainline
{
    public Item[] items { get; set; }
}

public class Item
{
    public string answerType { get; set; }
    public int resultIndex { get; set; }
    public Value1 value { get; set; }
}

public class Value1
{    public string id { get; set; }
}
