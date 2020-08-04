using System;

namespace CanvasClient.Domain
{
    public class Course
    {
        public int id { get; set; }
        public string name { get; set; }
        public int account_id { get; set; }
        public string uuid { get; set; }
        public DateTime start_at { get; set; }
        public string course_code { get; set; }
        public string default_view { get; set; }
        public int root_account_id { get; set; }
        public int enrollment_term_id { get; set; }
        public DateTime? end_at { get; set; }
        public string time_zone { get; set; }
        /// <summary>
        /// Use this for linking from Banner, it's formatted as [TERM_CODE]-[SUBJ_CODE]-[CRSE_NUM]-[SEQ_NUM]-[CRN]
        /// </summary>
        public string sis_course_id { get; set; }
        public string integration_id { get; set; }
        public string workflow_state { get; set; }
    }
}


/*
  {
    "id": 400328,
    "name": "LAW 284 001 SSEM 2020",
    "account_id": 1214,
    "uuid": "sx8jrMSDrxFLqSbKZVNMI7Pgbzf4MTURm25Lm1fZ",
    "start_at": "2020-01-14T20:16:36Z",
    "course_code": "LAW 284 001 SSEM 2020",
    "default_view": "wiki",
    "root_account_id": 1,
    "enrollment_term_id": 125,
    "end_at": null,
    "time_zone": "America/Los_Angeles",
    "sis_course_id": "202002-LAW-284-001-20203",
    "workflow_state": "available",
  }

// unused:
    "grading_standard_id": 68,
    "is_public": null,
    "created_at": "2019-09-07T00:07:27Z",
    "license": null,
    "grade_passback_setting": null,
    "public_syllabus": false,
    "public_syllabus_to_auth": false,
    "storage_quota_mb": 2000,
    "is_public_to_auth_users": false,
    "apply_assignment_group_weights": false,
    "blueprint": false,
    "hide_final_grades": false,
    "restrict_enrollments_to_course_dates": false,
    "calendar": {
      "ics": "https://canvas.ucdavis.edu/feeds/calendars/course_sx8jrMSDrxFLqSbKZVNMI7Pgbzf4MTURm25Lm1fZ.ics"
    },
    "sis_import_id": 293315,
    "integration_id": null,
  },

 */
