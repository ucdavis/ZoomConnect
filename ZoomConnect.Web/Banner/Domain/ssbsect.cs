using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZoomConnect.Web.Banner.Domain
{
    public class ssbsect : IBannerTable
    {
        public string term_code { get; set; }
        public string crn { get; set; }
        public string subj_code { get; set; }
        public string crse_numb { get; set; }
        public string seq_numb { get; set; }
        public string crse_title { get; set; }
        public decimal enrl { get; set; }
    }
}
