using System;

namespace ZoomConnect.Web.Models
{
    public class WeekdayRecurrence
    {
        public WeekdayRecurrence(DateTime firstOccurrence, int dayOfWeek, DateTime termEnd)
        {
            int offset = 0;
            if (((int)firstOccurrence.DayOfWeek) <= dayOfWeek)
            {
                offset = dayOfWeek - (int)firstOccurrence.DayOfWeek;
            }
            else
            {
                offset = 7 - (int)firstOccurrence.DayOfWeek + dayOfWeek;
            }

            startDateTime = firstOccurrence.AddDays(offset);
            occurrences = (int)Math.Floor(termEnd.Subtract(startDateTime).TotalDays / 7);
        }

        /// <summary>
        /// Start Date and Time of the given day of the week for this course
        /// </summary>
        public DateTime startDateTime { get; set; }
        /// <summary>
        /// Number of occurrences of this weekday for the course
        /// </summary>
        public int occurrences { get; set; }
    }
}
