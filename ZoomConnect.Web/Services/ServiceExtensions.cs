using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            sb.Append("<table><tr><th>Name</th><th>Email</th><th>Duration (Minutes)</th><th>Date</th></tr>");

            // get report date for each row from first row
            var reportDate = participants.FirstOrDefault(p => !String.IsNullOrEmpty(p.join_time));
            if (reportDate == null) { return ""; }

            var reportDateParsed = DateTime.Parse(reportDate.join_time, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);

            // group participants by email address for aggregating minutes and names
            participants.Where(p => String.IsNullOrEmpty(p.user_email))
                .ToList()
                .ForEach(p =>
                {
                    sb.Append($"<tr><td>{p.name}</td><td>{p.user_email}</td><td>{Math.Ceiling(p.duration / 60.0)}</td><td>{reportDateParsed:MM/dd}</td></tr>");
                });
            participants.Where(p => !String.IsNullOrEmpty(p.user_email))
                .GroupBy(p => p.user_email, p => p)
                .ToList()
                .ForEach(p =>
                {
                    var names = String.Join(";", p.Select(n => n.name).Distinct().ToList());
                    var durationMinutes = Math.Ceiling(p.Sum(d => d.duration) / 60.0);
                    sb.Append($"<tr><td>{names}</td><td>{p.Key}</td><td>{durationMinutes}</td><td>{reportDateParsed:MM/dd}</td></tr>");
                });

            // table footer
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}
