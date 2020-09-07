using Microsoft.Extensions.Options;
using SearchUrl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchUrl.Utility;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;


namespace SearchUrl.Services
{
    public class SearchService
    {
        
        public SearchService()
        {
        }

        public bool IsValidUrl(string url)
        {
            Uri uri;
            if (url.StartsWith("/url?q="))
            {
                url = url.Substring(7, url.Length - 7);
            }
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }

        public string ProcessSearchResult(SearchConfig searchEngine, string searchString,string searchUrl)
        {
            searchString = searchString.Trim();
            var response = WebUtil.GetRequestData(searchEngine, searchString);
            List<int> searchRequests = new List<int>();
            using (Stream dataStream = response.GetResponseStream())
            {
                //
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                List<string> filterUrls = new List<string>();
                filterUrls.Add("www.w3.org");
                filterUrls.Add("schemas.live.com");
                filterUrls.Add(searchEngine.url);

                var regex = new Regex("<a [^>]*href=(?:'(?<href>.*?)')|(?:\"(?<href>.*?)\")", RegexOptions.IgnoreCase);
                var urls = regex.Matches(responseFromServer).OfType<Match>().Select(m => m.Groups["href"].Value).ToList();
                urls = urls.FindAll(t => IsValidUrl(t) && (t.Contains("http://") || t.Contains("https://")) &&
                    !(filterUrls.Any(filter => t.Contains(filter))));
                string prevDomain = "";
                var searchIndex = 0;

                for (int i = 0; i < urls.Count(); i++)
                {
                    var currentUrl = urls[i];
                    if (urls[i].StartsWith("/url?q="))
                    {
                        currentUrl = urls[i].Substring(7, urls[i].Length - 7);
                    }
                    Uri url = new Uri(currentUrl);
                    string currentUrlHost = url.Host;

                    if (i > 0)
                    {                        
                        prevDomain = urls[i - 1];
                        if (urls[i - 1].StartsWith("/url?q="))
                        {
                            prevDomain = prevDomain.Substring(7, urls[i - 1].Length - 7);
                        }
                        url = new Uri(prevDomain);
                        prevDomain = url.Host;

                        
                    }
                    if (prevDomain != currentUrlHost || i == 0)
                    {
                        searchIndex++;
                        if (urls[i].Contains(searchUrl))
                        {
                            searchRequests.Add(searchIndex);
                        }
                    }

                }
                // Display the content.
                response.Close();
                
            }
            string result = "0";
            if (searchRequests.Count > 0)
            {
                result = string.Join(",", searchRequests);
            }
            if (result == "0")
                return "No records found";
            return "Search Available at : " + result;
            
        }
    }
}
