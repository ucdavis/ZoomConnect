using System;

namespace CanvasClient.Domain
{
    public class EventRequestData
    {
        /// <summary>
        /// Context code of the course/group/user whose calendar this event should be added to.
        /// Courses are referenced in the form "course_[course_id]"
        /// </summary>
        public string context_code { get; set; }
        public string title { get; set; }
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
        public string description { get; set; }
        public string comments { get; set; }
        public string location_name { get; set; }
        public EventRecurrence duplicate { get; set; }
    }
}
/*
public string location_address { get; set; }
public bool all_day { get; set; }
*/

