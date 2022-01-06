using System;
using System.Collections.Generic;

namespace ZoomConnect.Web.ViewModels
{
    public class EmailListModel
    {
        public EmailListModel()
        {
            EmailList = new List<String>();
        }

        public List<String> EmailList { get; set; }
        public String emails { get; set; }
    }
}
