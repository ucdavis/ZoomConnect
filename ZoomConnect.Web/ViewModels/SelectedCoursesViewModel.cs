using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoomConnect.Web.ViewModels
{
    public class SelectedCoursesViewModel
    {
        public SelectedCoursesViewModel()
        {
            Courses = new List<CourseViewModel>();
        }

        public List<CourseViewModel> Courses { get; set; }

        public int SelectedCourses => Courses.Where(c => c.IsSelected).Count();

        public bool IncludeCanvas { get; set; }
    }
}
