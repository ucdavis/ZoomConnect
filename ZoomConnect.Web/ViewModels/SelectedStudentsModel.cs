using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoomConnect.Web.ViewModels
{
    public class SelectedStudentsModel
    {
        public SelectedStudentsModel()
        {
            Students = new List<StudentViewModel>();
        }

        public int SelectedStudents => Students.Where(s => s.IsSelected).Count();
        public int RemainingLicenses { get; set; }
        public string AlertClass => SelectedStudents > RemainingLicenses
            ? "alert-danger"
            : "alert-info";
        public string AlertMessage => SelectedStudents > RemainingLicenses
            ? $"You selected {SelectedStudents} students but have only {RemainingLicenses} remaining."
            : $"This action will use {SelectedStudents} of your remaining {RemainingLicenses} licenses.";

        public List<StudentViewModel> Students { get; set; }

        public int ConnectedCount => Students.Count(p => p.ZoomStatus == ZoomUserStatus.Connected);
        public int PendingCount => Students.Count(p => p.ZoomStatus == ZoomUserStatus.Pending);
        public int BasicCount => Students.Count(p => p.ZoomStatus == ZoomUserStatus.Basic);
        public int MissingCount => Students.Count(p => p.ZoomStatus == ZoomUserStatus.Missing);
        public int InactiveCount => Students.Count(p => p.ZoomStatus == ZoomUserStatus.Inactive);
    }
}
