using System;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Models
{
    /// <summary>
    /// Represents a single course assignment for a prof.
    /// Includes data from sirasgn and ssbsect rows.
    /// </summary>
    public class AssignmentModel
    {
        public AssignmentModel()
        {
        }

        public AssignmentModel(sirasgn assignment, ssbsect course)
        {
            term_code = assignment.term_code;
            crn = assignment.crn;
            pidm = assignment.pidm;
            primary_ind = assignment.primary_ind;

            subj_code = course.subj_code;
            crse_numb = course.crse_numb;
            seq_numb = course.seq_numb;
            crse_title = course.crse_title;
            enrl = course.enrl;
        }

        public string term_code { get; set; }
        public string crn { get; set; }
        public decimal pidm { get; set; }
        public string primary_ind { get; set; }

        public string subj_code { get; set; }
        public string crse_numb { get; set; }
        public string seq_numb { get; set; }
        public string crse_title { get; set; }
        public decimal enrl { get; set; }
    }
}
