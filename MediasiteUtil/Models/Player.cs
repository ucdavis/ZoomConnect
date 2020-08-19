using System;

namespace MediasiteUtil.Models
{
    public class Player
    {
        public String Id { get; set; }
        public String Owner { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }
        public String ParentFolderId { get; set; }
        public bool Recycled { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String LayoutId { get; set; }
        public String LayoutOptions { get; set; }
        public String PlayerUrl { get; set; }
        public String AlternateName { get; set; }
    }
}