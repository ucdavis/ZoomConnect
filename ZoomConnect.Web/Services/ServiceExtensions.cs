using System;
using System.Collections.Generic;
using System.Text;
using ZoomClient.Domain;

namespace ZoomConnect.Web.Services
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Gets an HTML report for a list of Meeting Participants, suitable for emailing.
        /// </summary>
        /// <param name="participants"></param>
        /// <returns></returns>
        public static string HtmlReport (this List<Participant> participants)
        {
            if (participants == null || participants.Count == 0)
            {
                return "";
            }

            var sb = new StringBuilder(participants.Count * 100);

            // table header
            sb.Append("<table><tr><th>Name</th><th>Email</th><th>Duration (Minutes)</th></tr>");

            // rows
            participants.ForEach(p =>
            {
                sb.Append($"<tr><td>{p.name}</td><td>{p.user_email}</td><td>{Math.Ceiling(p.duration / 60.0)}</td></tr>");
            });

            // table footer
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}
