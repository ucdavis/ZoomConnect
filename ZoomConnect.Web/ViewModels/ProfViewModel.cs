using System;
using System.Linq;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.ViewModels
{
    public class ProfViewModel
    {
        public ProfViewModel()
        {
        }

        public ProfViewModel(ProfDataModel prof)
        {
            Pidm = prof.bannerPerson.pidm;
            Name = $"{prof.bannerPerson.last_name}, {prof.bannerPerson.first_name}";
            Email = prof.primaryEmail == null ? "no email" : prof.primaryEmail.email_address;
            AltEmailCount = prof.otherEmails.Count;
            AltEmails = String.Join(", ", prof.otherEmails.Select(e => e.email_address));
            CourseCount = prof.assignments.Count;
            Courses = String.Join(", ", prof.assignments
                .OrderBy(a => a.crse_numb)
                .Select(a => String.Format("{0} {1}", a.subj_code, a.crse_numb)));
            ZoomStatus = prof.ZoomStatus();
            if (ZoomStatus == ZoomUserStatus.Missing)
            {
                RowStatusClass = "";
                RowStatusIcon = "oi oi-x text-danger";
            }
            else if (ZoomStatus == ZoomUserStatus.Pending)
            {
                ZoomUserId = prof.zoomUser.id;
                RowStatusClass = "table-warning";
                RowStatusIcon = "oi oi-clock text-warning";
            }
            else if (ZoomStatus == ZoomUserStatus.Basic)
            {
                ZoomUserId = prof.zoomUser.id;
                ZoomUserEmail = prof.zoomUser.email;
                RowStatusClass = "table-warning";
                RowStatusIcon = "oi oi-dollar text-warning";
            }
            else
            {
                ZoomUserId = prof.zoomUser.id;
                ZoomUserEmail = prof.zoomUser.email;
                RowStatusClass = "table-success";
                RowStatusIcon = "oi oi-check text-success";
            }
        }

        /// <summary>
        /// Row is selected by user
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Banner person identifier, also the id for this row in a display list
        /// </summary>
        public decimal Pidm { get; set; }

        /// <summary>
        /// Banner person name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Prof primary email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Prof alt email count
        /// </summary>
        public int AltEmailCount { get; set; }

        /// <summary>
        /// Prof alt emails comma-separated
        /// </summary>
        public string AltEmails { get; set; }

        /// <summary>
        /// Prof course assignment count
        /// </summary>
        public int CourseCount { get; set; }

        /// <summary>
        /// Prof couse assignments by course number comma-separated
        /// </summary>
        public string Courses { get; set; }

        /// <summary>
        /// Prof is connected to a zoom account
        /// </summary>
        public ZoomUserStatus ZoomStatus { get; set; }

        /// <summary>
        /// bootstrap class for status of row based on ZoomStatus
        /// </summary>
        public string RowStatusClass { get; set; }

        /// <summary>
        /// OpenIconic class for icon to show in row based on ZoomStatus
        /// </summary>
        public string RowStatusIcon { get; set; }

        /// <summary>
        /// Prof zoom user email
        /// </summary>
        public string ZoomUserEmail { get; set; }

        /// <summary>
        /// Prof zoom user id
        /// </summary>
        public string ZoomUserId { get; set; }
    }
}
