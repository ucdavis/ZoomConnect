using System;

namespace ZoomConnect.Web.Banner.Domain
{
    /// <summary>
    /// Banner students
    /// </summary>
    public class spriden_student : IBannerTable
    {
        public decimal pidm { get; set; }
        public string id { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string email { get; set; }
        public string degc { get; set; }
        public string classlevel { get; set; }
    }
}
