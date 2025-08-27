using MediasiteUtil;
using MediasiteUtil.Models;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Web.Banner.Domain;
using static NuGet.Packaging.PackagingConstants;

namespace ZoomConnect.Web.Models
{
    public class MediasiteCourseModel
    {
        public MediasiteCourseModel(ssrmeet bannerMeeting, ssbsect bannerCourse, ProfDataModel primaryProf, List<Folder> mediasiteCourseFolders,
            MediasiteClient msClient)
        {
            this.bannerMeeting = bannerMeeting;
            this.bannerCourse = bannerCourse;
            this.primaryProf = primaryProf;
            this.mediasiteCourseFolders = mediasiteCourseFolders;
            this.mediasiteClient = msClient;

            MediasiteFolderId = mediasiteCourseFolders.FirstOrDefault(f => f.Name == MediasiteFolderName)?.Id ?? "";
            MediasiteSchedule = msClient.FindSchedule(MediasiteScheduleName, MediasiteFolderId);
        }

        public ssrmeet bannerMeeting { get; set; }
        public ssbsect bannerCourse { get; set; }

        public ProfDataModel primaryProf { get; set; }
        public List<ProfDataModel> otherProfs { get; set; }

        public List<Folder> mediasiteCourseFolders { get; set; }

        private MediasiteClient mediasiteClient;

        /// <summary>
        /// Row is selected by user
        /// </summary>
        public bool IsSelected { get; set; }

        public decimal MeetingId { get; set; }

        public string MediasiteFolderId { get; set; }// => mediasiteFolders?.Id;
        public Schedule MediasiteSchedule { get; } = null;

        public string MediasiteFolderName => $"{bannerMeeting.term_code}-{bannerMeeting.crn}";
        public string MediasiteScheduleName => $"{primaryProf.bannerPerson.last_name} - Law {bannerCourse.crse_numb} - {bannerCourse.crse_title} | {bannerCourse.crn}";

        public string MediasiteFolderCssClass => MediasiteFolderId == null ? "oi oi-x text-danger" : "oi oi-folder text-success";

        public bool ShowCheckbox =>
            bannerMeeting != null &&
            bannerCourse != null &&
            bannerMeeting.bldg_code == "KING" &&
            ((bannerMeeting.room_code ?? "") != "") &&
            ((bannerMeeting.begin_time ?? "") != "") &&
            MediasiteSchedule == null;
    }
}
