﻿using System;

namespace ZoomConnect.Web.Banner.Domain
{
    /// <summary>
    /// Banner Terms
    /// </summary>
    public class stvterm : IBannerTable
    {
        public string code { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}
