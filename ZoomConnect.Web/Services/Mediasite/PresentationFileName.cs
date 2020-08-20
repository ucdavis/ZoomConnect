using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ZoomConnect.Web.Services.Mediasite
{
    public class PresentationFileName
    {
        private Regex startPattern = new Regex(@"^(\d{9,11})_(\d{4})(\d{2})(\d{2})_(\d{4})-(\d{4})_(LAW_\d{3}[A-Z]?[A-Z]?)_(\d{5})_", RegexOptions.IgnoreCase);
        private Regex endPattern = new Regex(@"(\d{4})_(\d{4})_in_([A-Z]+_\d+)_+([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}).(MP4)$", RegexOptions.IgnoreCase);
        private Match startMatch;
        private Match endMatch;
        private bool fileNameProvided = false;
        private bool datesAndTimesOk = false;

        public PresentationFileName(string fileName)
        {
            fileNameProvided = true;
            startMatch = startPattern.Match(fileName);
            endMatch = endPattern.Match(fileName);
            datesAndTimesOk = RecDateTime.HasValue && CourseDateTime.HasValue;
        }

        public bool isOk
        {
            get
            {
                return fileNameProvided &&
                    startMatch.Success &&
                    endMatch.Success &&
                    datesAndTimesOk;
            }
        }

        public string Term
        {
            get
            {
                if (!isOk) { return ""; }
                return RecYear + (RecMonth.CompareTo("08") < 0 ? "02" : "09");
            }
        }

        public string Folder
        {
            get
            {
                if (!isOk) { return ""; }
                return String.Format("{0}-{1}", Term, Crn);
            }
        }

        public string MeetingId
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[1].Value;
            }
        }

        public string RecYear
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[2].Value;
            }
        }

        public string RecMonth
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[3].Value;
            }
        }

        public string RecDay
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[4].Value;
            }
        }

        public DateTime? RecDateTime
        {
            get
            {
                if (!startMatch.Success) { return null; }
                DateTime testDate = DateTime.MinValue;
                if (DateTime.TryParseExact(startMatch.Groups[2].Value + startMatch.Groups[3].Value + startMatch.Groups[4].Value + startMatch.Groups[5].Value,
                    "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out testDate))
                {
                    return testDate;
                }
                return null;
            }
        }

        public string RecStart
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[5].Value;
            }
        }

        public string RecEnd
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[6].Value;
            }
        }

        public string Course
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[7].Value;
            }
        }

        public string Crn
        {
            get
            {
                if (!isOk) { return ""; }
                return startMatch.Groups[8].Value;
            }
        }

        public string CourseStart
        {
            get
            {
                if (!isOk) { return ""; }
                return endMatch.Groups[1].Value;
            }
        }

        public string CourseEnd
        {
            get
            {
                if (!isOk) { return ""; }
                return endMatch.Groups[2].Value;
            }
        }

        public DateTime? CourseDateTime
        {
            get
            {
                if (!startMatch.Success || !endMatch.Success) { return null; }
                DateTime testDate = DateTime.MinValue;
                if (DateTime.TryParseExact(startMatch.Groups[2].Value + startMatch.Groups[3].Value + startMatch.Groups[4].Value + endMatch.Groups[1].Value,
                    "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out testDate))
                {
                    return testDate;
                }
                return null;
            }
        }

        public string Room
        {
            get
            {
                if (!isOk) { return ""; }
                return endMatch.Groups[3].Value;
            }
        }

        public string RecordingGuid
        {
            get
            {
                if (!isOk) { return ""; }
                return endMatch.Groups[4].Value;
            }
        }

        public string FileExtension
        {
            get
            {
                if (!isOk) { return ""; }
                return endMatch.Groups[5].Value;
            }
        }

        public string GuidFileName
        {
            get
            {
                if (!isOk) { return ""; }
                return String.Format("{0}.{1}", RecordingGuid, FileExtension);
            }
        }
    }
}
