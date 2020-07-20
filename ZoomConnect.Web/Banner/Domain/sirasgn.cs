using System;

namespace ZoomConnect.Web.Banner.Domain
{
    public class sirasgn : IBannerTable
    {
        public string term_code { get; set; }
        public string crn { get; set; }
        public decimal pidm { get; set; }
        public string primary_ind { get; set; }
    }
}
