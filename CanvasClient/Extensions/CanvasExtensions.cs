using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace CanvasClient.Extensions
{
    public static class CanvasExtensions
    {
        /// <summary>
        /// Returns next page url extracted from Link header, otherwise null
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string NextPageUrl(this IList<Parameter> headers)
        {
            var linkHeader = headers.FirstOrDefault(h => h.Name.ToLower() == "link");

            if (linkHeader == null) { return null; }

            var regex = new Regex("<([^>]+)>; rel=\"next\"", RegexOptions.IgnoreCase);

            var match = regex.Match(linkHeader.Value.ToString());

            if (match.Success) { return match.Groups[1].Value; }

            return null;
        }
    }
}
