using CanvasClient.Domain;
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

        /// <summary>
        /// Checks whether this Canvas EnrollmentTerm matches the description of passed in Banner Term (from stvterm_desc)
        /// </summary>
        /// <param name="canvasTerm">Canvas Enrollment Term</param>
        /// <param name="bannerTerm">Banner Term description from stvterm_desc</param>
        /// <returns>true if matching on year and term name, false otherwise</returns>
        public static bool MatchesBannerTermDesc(this EnrollmentTerm canvasTerm, string bannerTermDesc)
        {
            // fail on any null or empty string
            if (canvasTerm == null || String.IsNullOrEmpty(canvasTerm.name) || String.IsNullOrEmpty(bannerTermDesc)) { return false; }

            var bannerYear = bannerTermDesc.Substring(bannerTermDesc.Length - 4);
            var bannerTerm = bannerTermDesc.Substring(0, bannerTermDesc.Length - 4).Trim();

            return canvasTerm.name.StartsWith(bannerTerm) && canvasTerm.name.EndsWith(bannerYear);
        }
    }
}
