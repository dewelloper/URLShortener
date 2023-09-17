using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using UrlShortener.Db;
using UrlShortener.Models;

namespace UrlShortener.Util
{
    public static class ShortUrlValidator
    {
        public static bool IsURLValid(string? url)
        {
            // Check if the URL is not empty or null
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            // Check if the URL is in a valid format
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) || uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                return false;
            }

            // Additional checks (optional):
            // 1.We can implement custom logic to check for specific domains or patterns.
            // 2.We can add rate limiting checks here to prevent abuse.

            return true;
        }

        public static bool IsCustomAliasValid(string? alias)
        {
            // Check if the alias is not empty or null
            if (string.IsNullOrWhiteSpace(alias))
            {
                return false;
            }

            // Check if the alias contains only alphanumeric characters and hyphens
            return Regex.IsMatch(alias, "^[a-zA-Z0-9-]+$");
        }

        public static bool IsLengthFixed(string? aliasOrShortenedUrl, int? fixedLength)
        {
            if (aliasOrShortenedUrl.Length != fixedLength)
            {
                return false;
            }

            return true;
        }

        public static bool IsUrlAliasUnique(string originalUrl, string? alias)
        {
            var uri = new Uri(originalUrl);
            var host = uri.Host;

            if (MemoryDb.Urls.ContainsKey(host + "/" + alias))
            {
                  return false;
            }
                // Check if the alias is unique in your storage mechanism (e.g., a dictionary or database)
                // Return true if the alias is unique, false otherwise
                // we have used MemoryDb.Urls dictionary to store the short url and long url, it would be redis, mongodb or sql server in real world
             return true;
        }

        public static bool IsURLSafeForRedirection(string? url)
        {
            // Check if the URL is safe for redirection, e.g., no JavaScript or data URIs
            return !url.ToLower().StartsWith("javascript:") && !url.ToLower().StartsWith("data:");
        }

        public static bool IsURLReachable(string? url)
        {
            // Check if the URL is reachable (valid HTTP status code)
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        //ValidateAntiForgeryTokenAttribute
        public static string IsOriginalURLValid(string? originalURL, int? isLengthFixed = 6)
        {
            var errorMessages = string.Empty;

            if (!IsURLValid(originalURL))
                errorMessages = "Original URL is not valid.";

            if (!IsURLSafeForRedirection(originalURL))
                errorMessages = "URL is not safe for redirection.";

            //This section has been commented for easier testing.
            //if (!IsURLReachable(originalURL))
            //    errorMessages = "URL is not reachable.";

            return errorMessages;
        }

        public static string IsCustomURLValid(string? originalURL, string? customUrl, int? isLengthFixed = 6)
        {
            var errorMessages = string.Empty;

            if (!IsCustomAliasValid(customUrl))
                errorMessages = "Custom alias is not valid.";

            if (!IsUrlAliasUnique(originalURL, customUrl))
                errorMessages = "Custom alias is not unique.";

            return errorMessages;
        }

    }

}
