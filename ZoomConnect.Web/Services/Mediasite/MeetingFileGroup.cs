using MediasiteUtil.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Mediasite
{
    public class MeetingFileGroup
    {
        public string meetingId { get; set; }
        public IEnumerable<FileInfo> files { get; set; }
        public ZoomMeetingCourseModel meetingModel { get; set; }
        public string termCrn { get; set; }
        public string msFolderId { get; set; }
        public PresentationFullRepresentation presentation { get; set; }
        public PresentationFileName parsedFileName { get; set; }
        public FileInfo selectedFile
        {
            get
            {
                return files.First();
            }
        }
    }
}
