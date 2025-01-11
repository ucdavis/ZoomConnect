using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.ViewModels
{
    public class SelectedMediasiteModel
    {
        public SelectedMediasiteModel()
        {
            Courses = new List<MediasiteCourseModel>();
        }

        public List<MediasiteCourseModel> Courses { get; set; }

        public int SelectedCourses => Courses.Where(c => c.IsSelected).Count();
    }
}
