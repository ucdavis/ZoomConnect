using System;

namespace ZoomConnect.Web.Banner.Domain
{
    /// <summary>
    /// Banner course meetings
    /// </summary>
    public class ssrmeet : IBannerTable
    {
        public decimal surrogate_id { get; set; }
        public string term_code { get; set; }
        public string crn { get; set; }
        public string begin_time { get; set; }
        public string end_time { get; set; }
        public string bldg_code { get; set; }
        public string room_code { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string catagory { get; set; }
        public string sun_day { get; set; }
        public string mon_day { get; set; }
        public string tue_day { get; set; }
        public string wed_day { get; set; }
        public string thu_day { get; set; }
        public string fri_day { get; set; }
        public string sat_day { get; set; }
        public string schd_code { get; set; }
        public string over_ride { get; set; }
        public string meet_no { get; set; }
        public string hrs_week { get; set; }
    }
}
