using System;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.ViewModels
{
    public class StudentViewModel
    {
        public StudentViewModel()
        {
        }

        public StudentViewModel(StudentDataModel student)
        {
            Pidm = student.bannerPerson.pidm;
            Name = $"{student.bannerPerson.last_name}, {student.bannerPerson.first_name}";
            Email = student.bannerPerson.email == null ? "no email" : student.bannerPerson.email;
            ClassLevel = student.bannerPerson.classlevel;

            if (student.zoomUser == null || student.zoomUser.status == null)
            {
                ZoomStatus = ZoomUserStatus.Missing;
            }
            else if (student.zoomUser.status.ToLower() == "active")
            {
                ZoomStatus = ZoomUserStatus.Connected;
            }
            else if (student.zoomUser.status.ToLower() == "pending")
            {
                ZoomStatus = ZoomUserStatus.Pending;
            }
            else if (student.zoomUser.status.ToLower() == "inactive")
            {
                ZoomStatus = ZoomUserStatus.Inactive;
            }
            else
            {
                ZoomStatus = ZoomUserStatus.Missing;
            }

            if (ZoomStatus == ZoomUserStatus.Missing)
            {
                RowStatusClass = "";
                RowStatusIcon = "oi oi-x text-danger";
            }
            else if (ZoomStatus == ZoomUserStatus.Inactive)
            {
                RowStatusClass = "table-warning";
                RowStatusIcon = "oi oi-ban text-warning";
            }
            else if (ZoomStatus == ZoomUserStatus.Pending)
            {
                RowStatusClass = "table-warning";
                RowStatusIcon = "oi oi-clock text-warning";
            }
            else if (ZoomStatus == ZoomUserStatus.Basic)
            {
                RowStatusClass = "table-warning";
                RowStatusIcon = "oi oi-dollar text-warning";
            }
            else
            {
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
        /// Student name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Student email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Student Class Level
        /// </summary>
        public string ClassLevel { get; set; }

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
        /// Shows if this student can be licensed (has email and is Basic or not found)
        /// </summary>
        public bool CanBeLicensed =>
            (ZoomStatus == ZoomUserStatus.Missing || ZoomStatus == ZoomUserStatus.Basic) &&
            Email != "no email";
    }
}
