using System;
using System.Collections.Generic;

namespace ZoomConnect.Web.ViewModels
{
    public class SelectedCoursesViewModel
    {
        public SelectedCoursesViewModel()
        {
            Courses = new List<CourseViewModel>();
        }

        public List<CourseViewModel> Courses { get; set; }
    }
}
