using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HolidayStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HolidayEnd { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? VarianceDate { get; set; }
        public DayOfWeek? VarianceDayOfWeek { get; set; }
    }
}
