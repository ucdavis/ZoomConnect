using System;

namespace CanvasClient.Domain
{
    public class EnrollmentTerm
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
        public string workflow_state { get; set; }
        //public int grading_period_group_id { get; set; }
    }
}
