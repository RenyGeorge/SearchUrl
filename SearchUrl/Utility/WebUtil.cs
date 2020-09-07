using Microsoft.AspNetCore.Http;
using SearchUrl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SearchUrl.Utility
{
    public class WebUtil
    {
        public static string ParseUrl(string uri)
        {

            if (uri.StartsWith("//"))
                return new Uri("http:" + uri).AbsoluteUri;
            if (uri.StartsWith("://"))
                return new Uri("http" + uri).AbsoluteUri;

           if(!uri.StartsWith("http://") && !uri.StartsWith("https://"))
                 return new Uri("http://" + uri).AbsoluteUri;
            return uri;
        }
        public static string GenerateRequestUrl(SearchConfig searchEngine, string searchString)
        {
            if(!string.IsNullOrEmpty(searchEngine.url) && !string.IsNullOrEmpty(searchString))
            {
                StringBuilder searchUrl = new StringBuilder();
                var url = ParseUrl(searchEngine.url);
                searchUrl.Append(url);
                if (!searchEngine.url.EndsWith("/"))
                {
                    searchUrl.Append("/search?");
                }
                else
                {
                    searchUrl.Append("search?");
                }
                searchUrl.Append(searchEngine.queryParam);
                searchUrl.Append("=");
                searchString = searchString.Replace(" ", "+");
               
                searchUrl.Append(searchString);
                searchUrl.Append("&");
                searchUrl.Append(searchEngine.pageCountParam);
                searchUrl.Append("=10");
                return searchUrl.ToString();
            }
            return string.Empty;
        }
       
        public static WebResponse GetRequestData(SearchConfig searchEngine, string searchString)
        {
            string searchUrl = GenerateRequestUrl(searchEngine, searchString);
            WebRequest request = WebRequest.Create(searchUrl);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            return response;
        }

    }
}
