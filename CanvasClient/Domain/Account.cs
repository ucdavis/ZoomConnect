using System;

namespace CanvasClient.Domain
{
    public class Account
    {
        public int id { get; set; }
        public string name { get; set; }
        public string uuid { get; set; }
        public int? parent_account_id { get; set; }
        public int? root_account_id { get; set; }
        public string workflow_state { get; set; }

        /*
        public int default_storage_quota_mb { get; set; }
        public int default_user_storage_quota_mb { get; set; }
        public int default_group_storage_quota_mb { get; set; }
        public string default_time_zone { get; set; }
        */
    }
}
