using System;

namespace ZoomConnect.Web.Banner.Domain
{
    public class goremal : IBannerTable
    {
        public decimal pidm { get; set; }
        public string emal_code { get; set; }
        public string email_address { get; set; }
        public string status_ind { get; set; }
        public string preferred_ind { get; set; }
    }
}
