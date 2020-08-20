using System;

namespace ZoomConnect.Web.Services.Mediasite
{
    public class MediasiteJob
    {
        public string App { get; set; }
        public string JobId { get; set; }
        public string ExternalId { get; set; }
        public string FileName { get; set; }
        public string PresentationName { get; set; }
        public string Status { get; set; }
        public int Tries { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
